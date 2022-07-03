using System;
using XFX.Core.Logging;
using ILogger = XFX.Core.Logging.ILogger;

namespace XFX.Game {
    public static class Log {
        //public static ILogger logger { get; private set; }
        private static ILogger logger;

        public static void Init() {
            // 根据配置初始日志器
            logger = new UnityLogger("Game", LoggerLevel.Trace);
        }

        public static ILogger ForkChild(string name) {
            return logger.ForkChild(name);
        }

        public static void Trace(string message, params object[] args) {
            logger.Trace(message, args);
        }
        public static void Trace(Exception exception, string message, params object[] args) {
            logger.Trace(exception, message, args);
        }

        public static void Debug(string message, params object[] args) {
            logger.Debug(message, args);
        }
        public static void Debug(Exception exception, string message, params object[] args) {
            logger.Debug(exception, message, args);
        }

        public static void Info(string message, params object[] args) {
            logger.Info(message, args);
        }
        public static void Info(Exception exception, string message, params object[] args) {
            logger.Info(exception, message, args);
        }

        public static void Warn(string message, params object[] args) {
            logger.Warn(message, args);
        }
        public static void Warn(Exception exception, string message, params object[] args) {
            logger.Warn(exception, message, args);
        }

        public static void Error(string message, params object[] args) {
            logger.Error(message, args);
        }
        public static void Error(Exception exception, string message, params object[] args) {
            logger.Error(exception, message, args);
        }

        public static void Fatal(string message, params object[] args) {
            logger.Fatal(message, args);
        }
        public static void Fatal(Exception exception, string message, params object[] args) {
            logger.Fatal(exception, message, args);
        }

        public static string GetStackTrace() {
            System.Text.StringBuilder stackTraceBuilder = new System.Text.StringBuilder("");

            // System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace (e, true);
            System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace();
            int count = stackTrace.FrameCount;
            for (int i = 0; i < count; i++) {
                System.Diagnostics.StackFrame frame = stackTrace.GetFrame(i);

                stackTraceBuilder.AppendFormat("{0}.{1}", frame.GetMethod().DeclaringType.Name, frame.GetMethod().Name);

                System.Reflection.ParameterInfo[] parameters = frame.GetMethod().GetParameters();
                if (parameters == null || parameters.Length == 0) {
                    stackTraceBuilder.Append(" () ");
                } else {
                    stackTraceBuilder.Append(" (");

                    int pcount = parameters.Length;

                    System.Reflection.ParameterInfo param = null;
                    for (int p = 0; p < pcount; p++) {
                        param = parameters[p];
                        stackTraceBuilder.AppendFormat("{0} {1}", param.ParameterType.Name, param.Name);

                        if (p != pcount - 1) {
                            stackTraceBuilder.Append(", ");
                        }
                    }
                    param = null;

                    stackTraceBuilder.Append(") ");
                }

                string fileName = frame.GetFileName();
                if (!string.IsNullOrEmpty(fileName) && !fileName.ToLower().Equals("unknown")) {
                    fileName = fileName.Replace("\\", "/");

                    int loc = fileName.ToLower().IndexOf("/assets/");
                    if (loc < 0) {
                        loc = fileName.ToLower().IndexOf("assets/");
                    }

                    if (loc > 0) {
                        fileName = fileName.Substring(loc);
                    }

                    stackTraceBuilder.AppendFormat("(at {0}:{1})", fileName, frame.GetFileLineNumber());
                }
                stackTraceBuilder.AppendLine();
            }
            return stackTraceBuilder.ToString();
        }
    }

#if false
    //屏幕输出
    public class OutLog : MonoBehaviour {
        static FixedQueue<string> line_queue = new FixedQueue<string>(32);
        static FixedQueue<string> write_queue = new FixedQueue<string>(64);
        private string outpath;
        private GUIStyle guistyle;

        public static void Create() {
            if (Application.platform == RuntimePlatform.WindowsEditor)
                return;
            var go = new GameObject("screenlog");
            UnityEngine.Object.DontDestroyOnLoad(go);
            go.AddComponent<OutLog>();
            // CategorySettings.Attach(go.transform, "_debug/", false);
        }

        void Start() {
            GUIStyleState guistate = new GUIStyleState();
            guistate.textColor = Color.red;
            guistyle = new GUIStyle();
            guistyle.fontSize = 36;
            guistyle.normal = guistate;

            this.outpath = PathExt.MakeCachePath("outlog.txt");
            if (System.IO.File.Exists(this.outpath))
                System.IO.File.Delete(this.outpath);

            Application.logMessageReceived += HandleLog;
        }

        void OnDestroy() {
            Application.logMessageReceived -= HandleLog;
        }

        void HandleLog(string log, string stacktrace, LogType type) {
            string writestr = string.Format("[{0}] {1}\n{2}", type, log, stacktrace);
            write_queue.Enqueue(writestr);

            if (type == LogType.Assert || type == LogType.Error || type == LogType.Exception) {
                Log(log);
                Log(stacktrace);
            }
        }

        public static void Log(params object[] objs) {
            string text = "";
            for (int i = 0; i < objs.Length; ++i) {
                if (i == 0)
                    text = objs[i].ToString();
                else
                    text += ", " + objs[i].ToString();
            }
            if (Application.isPlaying) {
                if (line_queue.Count >= 20)
                    line_queue.Dequeue();
                line_queue.Enqueue(text);
            }
        }

        void Update() {
            if (write_queue.Count > 0) {
                using(System.IO.StreamWriter writer = new System.IO.StreamWriter(this.outpath, true, System.Text.Encoding.UTF8)) {
                    foreach (string str in write_queue.GetEnumerator()) {
                        writer.WriteLine(str);
                    }
                }
                write_queue.Clear();
            }
        }

        void OnGUI() {
            GUI.color = Color.red;
            foreach (var str in line_queue.GetEnumerator()) {
                GUILayout.Label(str);
            }
        }
    }
#endif

}