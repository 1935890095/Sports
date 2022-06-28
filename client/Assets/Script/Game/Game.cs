using System.Collections;
using UnityEngine;
using ZF.Asset;
using ZF.Core.Render;
using ZF.Core.Router;

namespace ZF.Game {
    public interface IGame {
        IRouter router { get; }
        Coroutine StartCoroutine(IEnumerator routine);
        IRoot root { get; }
        IRoot viewroot { get; }
        IHttpNetwork http { get; }
    }

    class Game : Core.Instance.InstanceManager, IGame, ILoop {
        private LoopKit _kit = null;
        public IRouter router { get; private set; }
        public IRoot root { get; private set; }
        public IRoot viewroot { get; private set; }
        public IHttpNetwork http { get; private set; }

        private Resource _resource;
        private AtlasMgr _atlas;
        private Scenes _scenes;
        private Lua _lua;
        private NetworkMgr _network;

        public Game() {
            GameObject root = new GameObject("Game");
            Object.DontDestroyOnLoad(root);
            this._kit = root.AddComponent<LoopKit>();
            this._kit.loop = this;

            Log.Init();
            Input.multiTouchEnabled = false;

            this.router = new Router();
            this.http = new HttpNetwork(Log.ForkChild("http"), 3);
            this._network = CreateSingle<NetworkMgr>();
            this._resource = CreateSingle<Resource>();
            this._atlas = CreateSingle<AtlasMgr>();
            this._scenes = CreateSingle<Scenes>();
            this._lua = CreateSingle<Lua>();

            this.root = Root.Create(null, null);
            this.viewroot = Root.Create(GameObject.FindWithTag("uiroot"), this.root);
            this.viewroot.position = new Vector3(0, 100, 0);
            this.viewroot.name = "[viewroot]";

            Log.Debug("game start");
        }

        public IEnumerator Start() {
            var preload = RenderInstance.Create<PreloadAssets>("res/preload/preload.preload", this.root);
            yield return preload;
            this._lua.Load();
        }

        public override void Init(Core.Instance.IInstance inst, string name) {
            if (inst is Instance m) {
                m.Init(this);
            }
            base.Init(inst, name);
        }

        public void Update() {
            this._resource.Update();
            this._scenes.Update();
            this._lua.Update();
            this._network.Update();
            this.root.Update();
            this.http.Update();
        }

        public void LateUpdate() {
            this._resource.LateUpdate();
            this._scenes.LateUpdate();
            this._lua.LateUpdate();
            this._network.LateUpdate();
            this.root.LateUpdate();
        }

        public void FixedUpdate() { }

        public void OnLevelWasLoaded(int level) {
            string active_scene_name = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            this.router.Event<string>(GameEvent.UNITY_LEVEL_WAS_LOADED, active_scene_name);
        }

        public void OnApplicationQuit() {
            this.router.Event(GameEvent.UNITY_APPLICATION_QUIT);
        }

        public void OnApplicationPause(bool status) {
            this.router.Event<bool>(GameEvent.UNITY_APPLICATION_PAUSE, status);
        }

        public Coroutine StartCoroutine(IEnumerator routine) {
            return this._kit.StartCoroutine(routine);
        }

        #region Loop Implement
        private class LoopKit : MonoBehaviour {
            public ILoop loop;
            private void Update() { if (loop != null) loop.Update(); }
            private void LateUpdate() { if (loop != null) loop.LateUpdate(); }
            private void FixedUpdate() { if (loop != null) loop.FixedUpdate(); }
            private void OnLevelWasLoaded(int level) { if (loop != null) loop.OnLevelWasLoaded(level); }
            private void OnApplicationQuit() { if (loop != null) loop.OnApplicationQuit(); }
            private void OnApplicationPause(bool status) { if (loop != null) loop.OnApplicationPause(status); }
        }
        #endregion
    }

    interface ILoop {
        void Update();
        void LateUpdate();
        void FixedUpdate();
        void OnLevelWasLoaded(int level);
        void OnApplicationQuit();
        void OnApplicationPause(bool status);
    }

    class Instance : Core.Instance.Instance {
        protected IGame game;
        protected IRouter router;

        public void Init(IGame game) {
            this.game = game;
            this.router = game.router;
        }

        public void Update() { this.OnUpdate(); }
        public void LateUpdate() { this.OnLateUpdate(); }

        protected virtual void OnUpdate() { }
        protected virtual void OnLateUpdate() { }
    }
}