/********************************************************
    Id: Scene.cs
    desc: Game层的场景管理器
    Author: figo
    Date: 2020-06-22 18:58:10
*********************************************************/

using ZF.Core.Scene;
using SceneManager = ZF.Core.Scene.SceneManager;

namespace ZF.Game {
    // class SceneOption : ISceneOption {
    //     // 是否同步加载
    //     public bool synchronize { get; set; }
    //     // 当异步加载时，指定是否禁止激活场景
    //     public bool denySceneActivation { get { return false; } }
    //     // 是否是内置场景
    //     public bool builtin { get; set; }
    //     // 是否叠加加载
    //     public bool additive { get; set; }
    //     // 加载/卸载场景时会调用该接口
    //     public void OnInvoke(IScene scene, bool load, bool done, float progress ) {
    //         Log.Info("===>scene:{0} load:{1} done:{2} progress:{3}", scene.name, load, done, progress);
    //     }
    // }

    class Scenes : Instance {
        private ISceneManager scenemgr = null;

        protected override void OnInit() {
            this.scenemgr = new SceneManager((name, option) => {
                return new Asset.Scene(name, option, this.game.root);
            }) {
                scene_dir = "res/scene",
                    scene_ext = "scene",
                    logger = Log.ForkChild("SceneManager")
            };

            this.router.On<string, ISceneOption>(GameEvent.SCENE_LOAD, OnLoadScene);
            this.router.On<string>(GameEvent.SCENE_UNLOAD, OnUnloadScene);
            this.router.On(GameEvent.SCENE_UNLOADALL, OnUnloadAllScene);
        }

        void OnLoadScene(string name, ISceneOption option) { this.scenemgr.LoadScene(name, option); }

        void OnUnloadScene(string name) { this.scenemgr.UnloadScene(name); }

        void OnUnloadAllScene() { this.scenemgr.UnloadAllScene(); }

        protected override void OnUpdate() { this.scenemgr.Loop(); }
    }
}