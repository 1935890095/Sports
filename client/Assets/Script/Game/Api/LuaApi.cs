using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using XLua;
using XFX.Misc;

namespace XFX.Game {
    // 导出到lua层使用的组件
    public static class LuaApiConfig {
        [GCOptimize]
        public static List<Type> GCOptimize = new List<Type> {
            // XLua/Src/GenAttributes.cs中包含了一部份
            typeof(Color32),
            typeof(Rect),
            typeof(KeyCode),
            typeof(RectTransform),
            typeof(TouchPhase),
        };

        [LuaCallCSharp]
        private static List<Type> LuaCallCSharp = new List<Type>() {
            typeof(Vector2),
            typeof(Vector3),
            typeof(Vector4),
            typeof(Color),
            typeof(Quaternion),
            typeof(Ray),
            typeof(Bounds),
            typeof(Ray2D),

            typeof(Color32),
            typeof(Rect),

            typeof(RectTransform),
            typeof(Resources),
            typeof(TextAsset),
            typeof(Application),
            typeof(Screen),
            typeof(PlayerPrefs),
            typeof(Time),
            typeof(System.Action),
            // typeof(Input), // 很奇怪，导出Input后，Build Lua会报错，见鬼。
            typeof(Shader),
            typeof(KeyCode),
            typeof(TouchPhase),
            typeof(SystemLanguage),
            typeof(AudioListener),
            typeof(SortingLayer),
            typeof(MeshCollider),
            typeof(NetworkReachability)
        };

        [CSharpCallLua]
        public static List<Type> CSharpCallLua = new List<Type>() {
            typeof(System.Action<float, float>),
            typeof(System.Action<int, string>),
            typeof(System.Action<int, bool>), // ui toggle
            typeof(System.Action<int, int>)
        };
    }

    [LuaCallCSharp]
    public partial class LuaApi {
        [BlackList]
        internal static LuaContext context;
        [BlackList]
        internal static IGame game;
        [BlackList]
        public static void Setup(IGame game, LuaContext context) {
            LuaApi.game = game;
            LuaApi.context = context;
            Sdk.Setup();
        }

        public static void Quit() {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public static void Reload() {
            // Log.Error("reload is unsupported");
            game.router.Event(GameEvent.RELOAD);
        }

        // 设置帧率
        public static void SetFrameRate(int rate) {
            Application.targetFrameRate = rate;
        }
    }

    // PathExt
    public partial class LuaApi {
        [LuaCallCSharp]
        public class PathExt {
            public static string MakeLoadPath(string name) { return XFX.Core.Util.PathExt.MakeLoadPath(name); }
            public static string MakeWWWPath(string name) { return XFX.Core.Util.PathExt.MakeWWWPath(name); }
            public static string MakeCachePath(string name) { return XFX.Core.Util.PathExt.MakeCachePath(name); }
        }
    }

    public partial class LuaApi {
        [LuaCallCSharp]
        public static class Cache {
            public static string Get(string fileName) {
                string path = XFX.Core.Util.PathExt.MakeCachePath(fileName);
                if (!File.Exists(path)) {
                    return null;
                }
                using(StreamReader reader = File.OpenText(path)) {
                    return reader.ReadToEnd();
                }
            }

            public static void Save(string fileName, string content, bool cachePath = true) {
                string path = null;
                if (!Application.isMobilePlatform && !cachePath)
                    path = XFX.Core.Util.PathExt.MakeLoadPath(fileName);
                else
                    path = XFX.Core.Util.PathExt.MakeCachePath(fileName);

                var fileInfo = new FileInfo(path);
                if (!fileInfo.Directory.Exists)
                    fileInfo.Directory.Create();
                File.WriteAllText(path, content, new System.Text.UTF8Encoding(false));
            }

            public static bool Exists(string fileName) {
                string path = XFX.Core.Util.PathExt.MakeCachePath(fileName);
                return File.Exists(path);
            }

            public static byte[] LoadProtobuf(string fileName) {
                string path = PathExt.MakeLoadPath(fileName);
                AssetBundle ab = AssetBundle.LoadFromFile(path);
                TextAsset[] asset = ab.LoadAllAssets<TextAsset>();
                byte[] data = Crypto.DesDecrypt(asset[0].bytes);
                ab.Unload(true);
                return data;
            }

            public static string GetServerUrl() {
                var asset = Resources.Load("server") as TextAsset;
                if (asset == null) {
                    return "";
                }
                string url = asset.text.Replace("\r", "").Replace("\n", "").Trim();
                Resources.UnloadAsset(asset);
                return url;
            }

            public static string GetUserId() {
                var asset = Resources.Load("user") as TextAsset;
                if (asset == null) {
                    return "";
                }
                string id = asset.text.Replace("\r", "").Replace("\n", "").Trim();
                Resources.UnloadAsset(asset);
                return id;
            }

            public static string GetConfig() {
                var asset = Resources.Load("config") as TextAsset;
                if (asset == null) {
                    return "";
                }
                string config = asset.text;
                Resources.UnloadAsset(asset);
                return config;
            }

            public static string GetConfig(string name) {
                var asset = Resources.Load(name) as TextAsset;
                if (asset == null) {
                    return "";
                }
                string config = asset.text;
                Resources.UnloadAsset(asset);
                return config;
            }

            public static void SendMail(string dress, string title) {
                if (string.IsNullOrEmpty(dress) || string.IsNullOrEmpty(title)) {
                    return;
                }
                Uri uri = new Uri(string.Format("mailto:{0}?subject={1}", dress, title));
                Application.OpenURL(uri.AbsoluteUri);
            }

            public static void DeleteDirectory(string name) {
                string path = PathExt.MakeCachePath(name);
                if(Directory.Exists(path)) {
                    Directory.Delete(path, true);
                }
            }
        }
    }

    // Logger
    public partial class LuaApi {
        private static XFX.Core.Logging.ILogger logger = XFX.Game.Log.ForkChild("Lua");

        [LuaCallCSharp]
        public static class Log {
            public static void Trace(string message, params object[] args) {
                logger.Trace(message, args);
            }
            public static void Debug(string message, params object[] args) {
                logger.Debug(message, args);
            }
            public static void Info(string message, params object[] args) {
                logger.Info(message, args);
            }
            public static void Warn(string message, params object[] args) {
                logger.Warn(message, args);
            }
            public static void Error(string message, params object[] args) {
                logger.Error(message, args);
            }
            public static void Fatal(string message, params object[] args) {
                logger.Fatal(message, args);
            }
        }
    }

    // Debug
    public partial class LuaApi {
        [LuaCallCSharp]
        public static class Debug {
            static UnityEngine.Object console = null;
            public static void FpsStart(string type) {
                switch (type) {
                    case "screen":
                        XFX.Misc.Fps.StartScreen();
                        break;
                    case "record":
                        XFX.Misc.Fps.StartRecord();
                        break;
                    default:
                        XFX.Misc.Fps.StartScreen();
                        break;
                }
            }

            public static void FpsStop(string type) {
                switch (type) {
                    case "screen":
                        XFX.Misc.Fps.StopScreen();
                        break;
                    case "record":
                        XFX.Misc.Fps.StopRecord();
                        break;
                    default:
                        XFX.Misc.Fps.StopScreen();
                        break;
                }
            }

            public static void ShowConsole() {
                console = UnityEngine.Object.Instantiate(Resources.Load("console"));
            }

            public static void HideConsole() {
                UnityEngine.Object.Destroy(console);
                console = null;
            }
        }
    }

    public partial class LuaApi {
        [LuaCallCSharp]
        public static class Screen {
            public static float NotchHeight { get; set; }
        }
    }

    public partial class LuaApi {
        [LuaCallCSharp]
        public static class Util {
            const string chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            static char[] buff = new char[64];
            public static string EncodeID(Int64 id) {
                int n = 0;
                while (id > 0) {
                    buff[n++] = chars[(int)(id % 62)];
                    id = id / 62;
                }
                for (int left = 0, right = n -1; left < right; left++, right--) {
                    var tmp = buff[left];
                    buff[left] = buff[right];
                    buff[right] = tmp;
                }
                return new string(buff, 0, n);
            }
        }
    }

}