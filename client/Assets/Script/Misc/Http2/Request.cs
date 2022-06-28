namespace ZF.Misc.HTTP2 {
    using UnityEngine;
    using System;
    using System.IO;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Net.Security;
    using System.Globalization;
    using System.Threading;
    using System.Security.Authentication;
    using System.Security.Cryptography.X509Certificates;

    using ZF.Core.Logging;
    using ILogger = ZF.Core.Logging.ILogger;

    public class HTTPException : Exception {
        public HTTPException( string message )
            : base( message ) {
        }
    }

    public enum RequestState {
        Waiting, Reading, Done
    }
 
    // Dns process
    public delegate string HttpDnsProcess(string uri, out string oridnal_host);

    public class Request {
        public static bool LogAllRequests = false;
        public static bool VerboseLogging = false;
        public static string unityVersion = Application.unityVersion;
        public static string operatingSystem = SystemInfo.operatingSystem;
        public static ILogger logger = NullLogger.Instance;
        public static HttpDnsProcess dnsProcess = null;

        public CookieJar cookieJar = CookieJar.Instance;
        public string method = "GET";
        // public string protocol = "HTTP/1.1";
        // public static byte[] EOL = { (byte)'\r', (byte)'\n' };
        public Uri uri;
        public Response response = null;
        public bool isDone = false;
        public int maximumRetryCount = 5;
        public bool acceptGzip = true;
        public bool useCache = false;
        public Exception exception = null;
        public volatile RequestState state = RequestState.Waiting;
        public long responseTime = 0; // in milliseconds
        public bool synchronous = false;
        // Use to save the stream when downloading.
        private Stream byteStream;
        public Stream outputStream;

        public Action<Request> completedCallback = null;

        Dictionary<string, List<string>> headers = new Dictionary<string, List<string>>();
        static Dictionary<string, string> etags = new Dictionary<string, string>();

        // The ordinal host
        private string host = "";

        static Request() {
            // support for https
            ServicePointManager.ServerCertificateValidationCallback += ValidateServerCertificate;
        }

        public Request(string method, string uri) {
            if (dnsProcess != null) uri = dnsProcess(uri, out host);
            this.method = method.ToUpper();
            this.uri = new Uri( uri );
        }

        public Request(string method, string uri, bool useCache) 
            : this(method, uri) {
            this.useCache = useCache;
        }

        public Request(string method, string uri, byte[] bytes)
            : this(method, uri) {
            this.byteStream = new MemoryStream(bytes);
        }

        public Request(string method, string uri, string text)
            : this(method, uri) {
            this.byteStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(text));
        }

        public Request(string method, string uri, StreamedWWWForm form)
            : this(method, uri) {
            this.byteStream = form.stream;
            foreach ( DictionaryEntry entry in form.headers ) {
                this.AddHeader( (string)entry.Key, (string)entry.Value );
            }
        }

        public Request(string method, string uri, WWWForm form)
            : this(method, uri) {
            this.byteStream = new MemoryStream(form.data);

            foreach (KeyValuePair<string, string> entry in form.headers) {
                this.AddHeader(entry.Key, entry.Value );
            }
        }

        public Request(string method, string uri, Hashtable data)
            : this( method, uri ) {
            this.byteStream = new MemoryStream( Encoding.UTF8.GetBytes( JSON.JsonEncode( data ) ) );
            this.AddHeader( "Content-Type", "application/json" );
        }

        public void AddHeader( string name, string value ) {
            name = name.ToLower().Trim();
            value = value.Trim();
            if ( !headers.ContainsKey( name ) )
                headers[name] = new List<string>();
            headers[name].Add( value );
        }

        public string GetHeader( string name ) {
            name = name.ToLower().Trim();
            if ( !headers.ContainsKey( name ) )
                return "";
            return headers[name][0];
        }

        public List<string> GetHeaders() {
            List<string> result = new List<string>();
            foreach ( string name in headers.Keys ) {
                foreach ( string value in headers[name] ) {
                    result.Add( name + ": " + value );
                }
            }

            return result;
        }

        public List<string> GetHeaders( string name ) {
            name = name.ToLower().Trim();
            if ( !headers.ContainsKey( name ) )
                headers[name] = new List<string>();
            return headers[name];
        }

        public void SetHeader( string name, string value ) {
            name = name.ToLower().Trim();
            value = value.Trim();
            if ( !headers.ContainsKey( name ) )
                headers[name] = new List<string>();
            headers[name].Clear();
            headers[name].TrimExcess();
            headers[name].Add( value );
        }

        private void GetResponse() {
            System.Diagnostics.Stopwatch curcall = new System.Diagnostics.Stopwatch();
            curcall.Start();
            try {
                var retry = 0;
                while ( retry++ < maximumRetryCount ) {
                    if ( useCache ) {
                        string etag = "";
                        if ( etags.TryGetValue( uri.AbsoluteUri, out etag ) ) {
                            SetHeader( "If-None-Match", etag );
                        }
                    }

                    var request = (HttpWebRequest)WebRequest.Create(uri);
                    // logger.Info("=> protocol version: {0}, method: {1}", request.ProtocolVersion.ToString(), this.method);
                    request.Method = this.method;
                    request.Timeout = request.ReadWriteTimeout = 3000;
                    request.AllowAutoRedirect = true;

                    // add headers
                    foreach (string name in headers.Keys) {
                        string value = string.Join(";", headers[name].ToArray());
                        // logger.Info("===> add request header {0}: {1}", name, value);
                        // https://docs.microsoft.com/en-us/dotnet/api/system.net.httpwebrequest.headers?view=netframework-4.8#System_Net_HttpWebRequest_Headers
                        switch ( name) {
                            case "accept": request.Accept = value; break;
                            case "connection": request.Connection = value; break;
                            case "content-length": request.ContentLength = long.Parse(value); break;
                            case "content-type": request.ContentType = value; break;
                            case "expect": request.Expect = value; break;
                            case "date": break;
                            case "host": break; 
                            case "if-modified-since": request.IfModifiedSince = DateTime.Parse(value); break;
                            case "range": request.AddRange(int.Parse(value)); break;
                            case "referer": request.Referer = value; break;
                            case "transfer-encoding": request.TransferEncoding = value; break;
                            case "user-agent":request.UserAgent = value; break;
                            default:
                                request.Headers.Add(name, value);
                                break;
                        }
                    }

                    // upload data
                    if (byteStream != null) {
                        long numBytesToWrite = byteStream.Length;
                        const int BufferSize = 4096;
                        byte[] buffer = new byte[BufferSize];
                        using (var istream = request.GetRequestStream()) {
                            while (numBytesToWrite> 0) {
                                int readed = byteStream.Read(buffer, 0, buffer.Length);
                                istream.Write(buffer, 0, readed);
                                numBytesToWrite -= readed;
                            }
                        }
                    }

                    response = new Response((HttpWebResponse)request.GetResponse(), cookieJar);
                    state = RequestState.Reading;
                    response.Read2Stream(this.outputStream);

                    if (response.status == 200) {
                        break;
                    } else {
                        logger.Warn("=> response status: {0}", response.status);
                    }
                }
                if (useCache) {
                    string etag = response.GetHeader( "etag" );
                    if (etag.Length > 0) etags[uri.AbsoluteUri] = etag;
                }

            } catch ( Exception e ) {
                logger.Warn("Unhandled Exception: {0}, aborting request, uri: {1}.", e.Message, uri.AbsoluteUri);
                exception = e;
                response = null;
            }

            state = RequestState.Done;
            isDone = true;
            responseTime = curcall.ElapsedMilliseconds;

            if (byteStream != null) { byteStream.Close(); }
            if (outputStream != null) { outputStream.Close(); }

            if ( completedCallback != null ) {
                if ( synchronous ) {
                    completedCallback( this );
                } else {
                    // we have to use this dispatcher to avoid executing the callback inside this worker thread
                    ResponseCallbackDispatcher.Singleton.requests.Enqueue( this );
                }
            }

            if ( LogAllRequests ) {
#if !UNITY_EDITOR
                logger.Info("NET: " + InfoString(VerboseLogging));
#else
                if ( response != null && response.status >= 200 && response.status < 300 ) {
                    logger.Info( InfoString( VerboseLogging ) );
                } else if ( response != null && response.status >= 400 ) {
                    logger.Info( InfoString( VerboseLogging ) );
                } else {
                    logger.Info( InfoString( VerboseLogging ) );
                }
#endif
            }
        }

        public void Send( Action<Request> callback = null ) {
            if ( !synchronous && callback != null && ResponseCallbackDispatcher.Singleton == null ) {
                ResponseCallbackDispatcher.Init();
            }

            completedCallback = callback;

            isDone = false;
            state = RequestState.Waiting;
            if ( acceptGzip ) {
                SetHeader( "Accept-Encoding", "gzip" );
            }

            if ( this.cookieJar != null ) {
                List<Cookie> cookies = this.cookieJar.GetCookies( new CookieAccessInfo( uri.Host, uri.AbsolutePath ) );
                string cookieString = this.GetHeader( "cookie" );
                for ( int cookieIndex = 0; cookieIndex < cookies.Count; ++cookieIndex ) {
                    if ( cookieString.Length > 0 && cookieString[cookieString.Length - 1] != ';' ) {
                        cookieString += ';';
                    }
                    cookieString += cookies[cookieIndex].name + '=' + cookies[cookieIndex].value + ';';
                } 
                SetHeader( "cookie", cookieString );
            }

            if ( byteStream != null && byteStream.Length > 0 && GetHeader( "Content-Length" ) == "" ) {
                SetHeader( "Content-Length", byteStream.Length.ToString() );
            }

            if ( GetHeader( "User-Agent" ) == "" ) {
                try {
                    SetHeader( "User-Agent", "UnityWeb/1.0 (Unity " + Request.unityVersion + "; " + Request.operatingSystem + ")" );
                } catch ( Exception ) {
                    SetHeader( "User-Agent", "UnityWeb/1.0" );
                }
            }

            // if ( GetHeader( "Connection" ) == "" ) {
            //     SetHeader( "Connection", "close" );
            // }

            // Basic Authorization
            if ( !String.IsNullOrEmpty( uri.UserInfo ) ) {
                SetHeader( "Authorization", "Basic " + System.Convert.ToBase64String( System.Text.ASCIIEncoding.ASCII.GetBytes( uri.UserInfo ) ) );
            }

            if ( synchronous ) {
                GetResponse();
            } else {
                ThreadPool.QueueUserWorkItem( new WaitCallback( delegate( object t ) {
                    GetResponse();
                } ) );
            }
        }

        public bool HasError { get { return (this.response == null || this.response.status != 200); } }

        public static bool ValidateServerCertificate( object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors ) {
#if !UNITY_EDITOR
            logger.Warn("NET: SSL Cert: " + sslPolicyErrors.ToString());
#else
            logger.Warn("SSL Cert Error: " + sslPolicyErrors.ToString());
#endif
            return true;
        }

        private static string[] sizes = { "B", "KB", "MB", "GB" };
        public string InfoString( bool verbose ) {
            string status = isDone && response != null ? response.status.ToString() : "---";
            string message = isDone && response != null ? response.message : "Unknown";
            double size = isDone && response != null ? response.length : 0.0f;

            int order = 0;
            while ( size >= 1024.0f && order + 1 < sizes.Length ) {
                ++order;
                size /= 1024.0f;
            }

            string sizeString = String.Format( "{0:0.##}{1}", size, sizes[order] );
            string result = uri.ToString() + " [ " + method.ToUpper() + " ] [ " + status + " " + message + " ] [ " + sizeString + " ] [ " + responseTime + "ms ]";

            if ( verbose && response != null ) {
                result += "\n\nRequest Headers:\n\n" + String.Join( "\n", GetHeaders().ToArray() );
                result += "\n\nResponse Headers:\n\n" + String.Join( "\n", response.GetHeaders().ToArray() );
                if ( response.Text != null ) { result += "\n\nResponse Body:\n" + response.Text; }
            }

            return result;
        }
    }
}
