using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using XLua;
using XFX.Core.Util;
using XFX.Misc;

namespace XFX.Game {
    class Lua : Instance {
        private const string LUA_AB_NAME = @"lua.bin";
        private const string LUA_RES_AB_NAME = @"resource.bin";
        private const string LUA_PATH = @"/lua";
        private const string LUA_PATH_LIB = @"/lua/lib";
        private const string RESOURCE_PREFIX = @"resource";

        private readonly Dictionary<string, byte[]> caches = new Dictionary<string, byte[]>();
        private LuaEnv env;
        private LuaContext context;
        public LuaEnv Env { get { return this.env; } }
        public LuaContext Context { get { return this.context; } }

        protected override void OnInit() {
            this.router.On<string>(GameEvent.UNITY_LEVEL_WAS_LOADED, OnLevelWasLoaded);
            this.router.On<bool>(GameEvent.UNITY_APPLICATION_PAUSE, OnApplicationPause);
            this.router.On(GameEvent.UNITY_APPLICATION_QUIT, OnApplicationQuit);
            this.router.On(GameEvent.RELOAD, Reload);
            this.router.On<int, byte[]>(GameEvent.FIRE_PROTO, OnFireProto);
            this.router.On(GameEvent.ON_CONNECTED, OnConnected);
            this.router.On(GameEvent.ON_CONNECT_FAILED, OnConnectFailed);
            this.router.On(GameEvent.ON_DISCONNECTED, OnDisconnected);
        }

        private void Reload() {
            Log.Warn("*** reload lua");
            this.game.StartCoroutine(ReloadAync());
        }

        private IEnumerator ReloadAync() {
            yield return null;
            ReleaseEnv();
            yield return new WaitForSeconds(1);
            Load();
        }

        public void Load() {
            if (Application.isMobilePlatform) {
                this.game.StartCoroutine(LoadResourceAB());
                this.game.StartCoroutine(LoadAB(() => InitEnv()));
            } else {
                LoadResource();
                if (Directory.Exists(Path.Combine(Application.dataPath, "Lua"))) {
                    LoadLua();
                    InitEnv();
                } else {
                    this.game.StartCoroutine(LoadAB(() => InitEnv()));
                }
            }
        }

        protected override void OnDestroy() {
            ReleaseEnv();
        }

        protected override void OnUpdate() {
            try {
                if (((int) Time.realtimeSinceStartup % 3) == 0) {
                    if (this.env != null)
                        this.env.Tick();
                }
                if (this.context != null) {
                    this.context.loop(Time.deltaTime,
                        Time.unscaledDeltaTime,
                        Time.realtimeSinceStartup);
                }
            } catch (Exception e) {
                Log.Error(e, "lua tick exception");
            }
        }

        void InitEnv() {
            LuaApi.Sdk.Invoke("flurry", "InitEnv", "");
            Log.Debug("*** init lua env");
            this.env = new LuaEnv();
            this.env.AddLoader(Loader);
            // this.env.AddBuildin("protobuf.c", XLua.LuaDLL.Lua.LoadProtobuf);
            this.env.AddBuildin("pb", XLua.LuaDLL.Lua.LoadLuaProtobuf);

            this.env.DoString("require 'Context'");
            // 获取lua上下文
            this.context = this.env.Global.Get<LuaContext>("Context");
            LuaApi.Setup(this.game, this.context);
            this.context.load();
        }

        void ReleaseEnv() {
            try {
                if (this.context != null)
                    this.context.unload();
                if (this.env != null)
                    this.env.Dispose();
                this.context = null;
                this.env = null;
                this.caches.Clear();
            } catch (Exception e) {
                Log.Error("*** release lua env error: {0}", e.Message);
                if (this.context != null)
                    this.context.print_func_ref_by_csharp();
            }
            Log.Debug("*** release lua env");
        }

        private void LoadLua() {
            string path = Path.Combine(Application.dataPath, "Lua");
            int start = path.IndexOf("Lua") + 4;
            string[] files = Directory.GetFiles(path, "*.lua", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++) {
                string fileName = files[i];
                byte[] content = File.ReadAllBytes(fileName);
                string name = fileName.Substring(start)
                    .Replace(Path.DirectorySeparatorChar, '.')
                    .Replace("lib.", "")
                    .Replace(".lua", "");
                caches[name] = content;
            }
        }

        private void LoadResource() {
            string path = Path.Combine(Application.streamingAssetsPath, "res/resource");
            int start = path.IndexOf("res") + 4;
            string[] files = Directory.GetFiles(path, "*.lua", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++) {
                string fileName = files[i];
                byte[] content = File.ReadAllBytes(fileName);
                string name = fileName.Substring(start)
                    .Replace(Path.DirectorySeparatorChar, '.')
                    .Replace(".lua", "");
                caches[name] = content;
            }
        }

        IEnumerator LoadAB(Action cb) {
            AssetBundle ab = null;
            string asset_path = string.Concat("res", LUA_PATH, "/", LUA_AB_NAME);
            ab = AssetBundle.LoadFromFile(PathExt.MakeLoadPath(asset_path));
            if (ab != null) {
                var request = ab.LoadAllAssetsAsync(typeof(TextAsset));
                yield return request;
                UnityEngine.Object[] assets = request.allAssets;
                Log.Info("===== lua.assets size = {0} =====", assets.Length);
                for (int i = 0; i < assets.Length; ++i) {
                    TextAsset text = assets[i] as TextAsset;
                    string name = text.name.Replace("lib.", "").Replace(".lua", "");
                    byte[] bytes = Crypto.DesDecrypt(text.bytes);
                    caches[name] = bytes;
                    Log.Info(">>> load lua {0}", name);
                }
                // release ab
                ab.Unload(true);
            }
            if (cb != null) cb();
        }

        IEnumerator LoadResourceAB() {
            AssetBundle ab = null;
            string asset_path = string.Concat("res", LUA_PATH, "/", LUA_RES_AB_NAME);
            ab = AssetBundle.LoadFromFile(PathExt.MakeLoadPath(asset_path));
            if (ab != null) {
                var request = ab.LoadAllAssetsAsync(typeof(TextAsset));
                yield return request;
                UnityEngine.Object[] assets = request.allAssets;
                Log.Info("===== lua.assets size = {0} =====", assets.Length);
                for (int i = 0; i < assets.Length; ++i) {
                    TextAsset text = assets[i] as TextAsset;
                    string name = text.name;
                    byte[] bytes = Crypto.DesDecrypt(text.bytes);
                    caches[name] = bytes;
                    Log.Info(">>> load lua {0}", name);
                }
                // release ab
                ab.Unload(true);
            }
        }

        byte[] Loader(ref string name) {
            if (caches.TryGetValue(name, out var content)) {
                return content;
            }

            name = string.Concat(name.Replace(".", "/"), ".lua");

            // 策划配置
            if (name.StartsWith(RESOURCE_PREFIX, StringComparison.Ordinal)) {
                name = string.Concat("res/", name);
                if (Application.isMobilePlatform) {
                    string filename = PathExt.MakeCachePath(name);
                    if (File.Exists(filename)) content = File.ReadAllBytes(filename);
                } else {
                    string filename = PathExt.MakeLoadPath(name);
                    if (File.Exists(filename)) { content = File.ReadAllBytes(filename); }
                }
                return content;
            }

            //TODO
            //#if DEV 
            if (!Application.isMobilePlatform) {
                string filename = string.Concat(Application.dataPath, LUA_PATH, "/", name);
                if (File.Exists(filename)) {
                    content = File.ReadAllBytes(filename);
                } else {
                    filename = string.Concat(Application.dataPath, LUA_PATH_LIB, "/", name);
                    if (File.Exists(filename)) { content = File.ReadAllBytes(filename); }
                }
            }
            //#endif
            return content;
        }

        void OnLevelWasLoaded(string scene) { if (this.context != null) this.context.invoke("game", "unity", "LevelWasLoaded", scene); }
        void OnApplicationQuit() { if (this.context != null) this.context.invoke("game", "unity", "ApplicationQuit", ""); }
        void OnApplicationPause(bool status) { if (this.context != null) this.context.invoke("game", "unity", "ApplicationPause", status); }
        void OnFireProto(int pid, byte[] data) { if (this.context != null) this.context.invoke("game", "net", "FireProto", pid, data); }

        void OnConnected() { if (this.context != null) this.context.invoke("game", "net", "OnConnected"); }
        void OnConnectFailed() { if (this.context != null) this.context.invoke("game", "net", "OnConnectFailed"); }
        void OnDisconnected() { if (this.context != null) this.context.invoke("game", "net", "OnDisconnected"); }
    }
}

namespace XLua.LuaDLL {
    public static partial class Lua {

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_pb(IntPtr ptr);

        [MonoPInvokeCallback(typeof(lua_CSFunction))]
        public static int LoadLuaProtobuf(IntPtr ptr) {
            return luaopen_pb(ptr);
        }
    }
}