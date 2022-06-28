using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ZF.Core.Logging;

namespace ZF.Game {
    public class HttpRequest {
        public string Path { get; set; }
        public string Method { get; set; }
        public byte[] Body { get; set; }
    }

    public class HttpResponse {
        public byte[] Body { get; set; }
    }

    public class HttpContext {
        public HttpRequest Request { get; set; }
        public HttpResponse Response { get; set; }
        public HttpClient Client { get; set; }
        public bool IsDone { get; set; }
        public string Error { get; set; }
        public Action<byte[]> OnResponse { get; set; }
        public Action<string> OnError { get; set; }
    }

    public interface IHttpNetwork {
        void Update();
        void Close();
        void Clear();

        void SetTimeout(int timeout);
        void SetHeader(string name, string value);
        void SetHold(bool hold);

        void Send(string path, string method, byte[] body, Action<byte[]> responseHandler, Action<string> errorHandler, bool now);
    }

    public class HttpNetwork : IHttpNetwork {
        private readonly Queue<HttpContext> _contexts = new Queue<HttpContext>();
        private static readonly Dictionary<string, string> _headers = new Dictionary<string, string>();
        private readonly HttpClient _client = new HttpClient();
        private HttpContext _current;
        private bool _hold;
        private HttpContext _first;

        private readonly ILogger _logger;

        public HttpNetwork(ILogger logger, int defaultTimeout) {
            _logger = logger;
            _client.Timeout = TimeSpan.FromSeconds(defaultTimeout);
        }

        public void Update() {
            ProcessRequest();
            ProcessResponse();
        }

        public void Close() {
            Clear();
            _client.Dispose();
        }

        public void Clear() {
            _client.CancelPendingRequests();
            _current = null;
            _contexts.Clear();
        }

        public void Send(string path, string method, byte[] body, Action<byte[]> responseHandler, Action<string> errorHandler, bool now) {
            HttpContext context = new HttpContext() {
                Request = new HttpRequest() {
                Path = path,
                Method = method,
                Body = body
                },
                Response = new HttpResponse(),
                OnResponse = responseHandler,
                OnError = errorHandler
            };
            if (now) {
                if (_first != null) {
                    return;
                }
                _first = context;
            } else {
                _contexts.Enqueue(context);
            }
        }

        public void SetHeader(string name, string value) {
            if (_headers.ContainsKey(name)) {
                _client.DefaultRequestHeaders.Remove(name);
            }
            _headers[name] = value;
            _client.DefaultRequestHeaders.Add(name, value);
        }

        public void SetTimeout(int timeout) {
            _client.Timeout = TimeSpan.FromSeconds(timeout);
        }

        public void SetHold(bool hold) {
            _hold = hold;
        }

        private void ProcessRequest() {
            if (_hold) {
                return;
            }
            if (_current != null) {
                return;
            }
            if (_first != null) {
                _current = _first;
                _first = null;
                _current.Client = _client;
                HttpSender.Send(_current);
            } else {
                if (_contexts.Count <= 0) {
                    return;
                }
                _current = _contexts.Dequeue();
                _current.Client = _client;
                HttpSender.Send(_current);
            }
        }

        private void ProcessResponse() {
            if (_current == null) {
                return;
            }
            if (!_current.IsDone) {
                return;
            }
            if (string.IsNullOrWhiteSpace(_current.Error)) {
                try {
                    _current.OnResponse(_current.Response.Body);
                } catch (Exception ex) {
                    _logger.Error(ex.Message);
                }
            } else {
                _current.OnError(_current.Error);
            }
            _current = null;
        }

        private class HttpSender {
            private readonly HttpContext _context;

            private HttpSender(HttpContext context) {
                _context = context;
            }

            public static void Send(HttpContext context) {
                HttpSender sender = new HttpSender(context);
                sender.Send();
            }

            private void Send() {
                Task.Factory.StartNew(RequestAsync);
            }

            private async Task RequestAsync() {
                HttpRequestMessage request = new HttpRequestMessage {
                    RequestUri = new Uri(_context.Request.Path)
                };
                if (_context.Request.Method == "GET") {
                    request.Method = HttpMethod.Get;
                } else if (_context.Request.Method == "POST") {
                    request.Method = HttpMethod.Post;
                    request.Content = new ByteArrayContent(_context.Request.Body);
                    if (_headers.TryGetValue("ContentType", out string value)) {
                        request.Content.Headers.ContentType = new MediaTypeHeaderValue(value);
                    }
                } else {
                    throw new NotSupportedException($"The method {_context.Request.Method} is not supported.");
                }
                try {
                    HttpResponseMessage response = await _context.Client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    _context.Response.Body = await response.Content.ReadAsByteArrayAsync();;
                } catch (Exception ex) {
                    _context.Error = ex.Message + "\n" + ex.StackTrace;
                    if (ex.InnerException != null) {
                        _context.Error += "\n" + ex.InnerException.Message;
                    }
                }
                _context.IsDone = true;
            }
        }
    }
}