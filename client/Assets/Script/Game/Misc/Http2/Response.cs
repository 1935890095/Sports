
namespace XFX.Misc.HTTP2 {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Globalization;
    using System.Net;
    using System.Linq;

    public class Response {
        // default value: 200
        public int status { get; private set; }
        // default value: OK
        public string message { get; private set; }
        public byte[] bytes { get; private set; }
        public int length { get;  private set; }

        Dictionary<string, List<string>> headers = new Dictionary<string, List<string>>();
        public long ContentLength { get { return this.response.ContentLength; } }
        public string ContentType { get { return this.response.ContentType; } }
        private WebResponse response;

        internal Response(HttpWebResponse response, CookieJar cookieJar) {
            this.response = response;
            this.status = (int)response.StatusCode;
            this.message = response.StatusCode.ToString();

            foreach (var name in response.Headers.AllKeys) {
                var value = response.Headers.Get(name);
                var values = value.Split(';');
                Array.ForEach(values, (v) => AddHeader(name, v));
            }

            if (cookieJar != null) {
                List<string> cookies = GetHeaders("set-cookie");
                for (int cookieIndex = 0; cookieIndex < cookies.Count; ++cookieIndex) {
                    string cookieString = cookies[cookieIndex];
                    if (cookieString.IndexOf("domain=", StringComparison.CurrentCultureIgnoreCase) == -1) {
                        cookieString += "; domain=" + response.ResponseUri.Host;
                    }
                    if (cookieString.IndexOf("path=", StringComparison.CurrentCultureIgnoreCase) == -1) {
                        cookieString += "; path=" + response.ResponseUri.AbsolutePath;
                    }
                    cookieJar.SetCookie(new Cookie(cookieString));
                }
            }
        }

        public string Text { get { return bytes != null ? System.Text.UTF8Encoding.UTF8.GetString( bytes ): ""; } }

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
            if (!headers.ContainsKey(name))
                return new List<string>();
            return headers[name];
        }

        public string GetHeader( string name ) {
            name = name.ToLower().Trim();
            if (!headers.ContainsKey(name))
                return string.Empty;
            return headers[name][headers[name].Count - 1];
        }

        void AddHeader(string name, string value) {
            name = name.ToLower().Trim();
            value = value.Trim();
            if (!headers.ContainsKey(name))
                headers[name] = new List<string>();
            headers[name].Add( value );
        }

        internal void Read2Stream(Stream stream) {
            using (var ostream = response.GetResponseStream())  {
                bool useMemory = false;
                if (stream == null) {
                    // MemoryStream is a disposable
                    // http://stackoverflow.com/questions/234059/is-a-memory-leak-created-if-a-memorystream-in-net-is-not-closed
                    stream = new MemoryStream();
                    useMemory = true;
                }

                const int BufferSize = 4096;
                byte[] buffer = new byte[BufferSize];
                do {
                    int len = ostream.Read(buffer, 0, buffer.Length);
                    if (len > 0) {
                        this.length += len;
                        stream.Write(buffer, 0, len);
                        continue;
                    }
                    break;
                } while (true);

                if (ContentLength > 0 && this.length != ContentLength) {
                    throw new HTTPException("Response length does not match content length");
                }

                if (useMemory) {
                    var memory = stream as MemoryStream;
                    bytes = memory.ToArray();
                    memory.Dispose();
                }

                ostream.Close();
            }
        }
    }
}
