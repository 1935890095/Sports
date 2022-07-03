
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using XLua;
using XFX.Core.Scene;

namespace XFX.Game {
    public static class LuaApiConfigScene{
        [CSharpCallLua]
        public static List<System.Type> CSharpCallLua = new List<System.Type>() {
            typeof (System.Action<string, bool, bool, float>), // IRenderObject.onComplete(not test)
        };
    }

    public partial class LuaApi {
        [LuaCallCSharp]
        public static class Scene {
            [BlackList]
            static GameObject mapRoot;

            public static void Setup() {
                mapRoot = GameObject.FindWithTag(XFX.Misc.Defines.Tag.MapRoot);
            }

            public static object GetMapRootProperty(string componentName, string propName) {
                if (string.IsNullOrEmpty(componentName)) return null;
                if (string.IsNullOrEmpty(propName)) return null;
                if (null == mapRoot) return null;

                object result = null;
                if (componentName.Equals("gameobject")) { 

                } else if (componentName.Equals("transform")) {
                    var component = mapRoot.GetComponent<Transform>();
                    switch (propName) { 
                        case "position":
                            result = component.position;
                            break;
                    }
                }
                return result;
            }

            public static void LoadScene(string name, 
                bool synchronize = false, 
                bool additive = false,
                System.Action<string, bool, bool, float> onInvoke = null) {
                ISceneOption option = new SceneOption() {
                    synchronize = synchronize,
                    additive =  additive,
                    onInvoke = onInvoke
                };
                game.router.Event(GameEvent.SCENE_LOAD, name, option);
            }

            public static void UnloadScene(string name) {
                game.router.Event(GameEvent.SCENE_UNLOAD, name);
                Resources.UnloadUnusedAssets();
            }

            public static void UnloadAllScene() {
                game.router.Event(GameEvent.SCENE_UNLOADALL);
            }

            class SceneOption : ISceneOption {
                // 是否同步加载
                public bool synchronize { get; set; } 
                // 当异步加载时，指定是否禁止激活场景(no use)
                public bool denySceneActivation { get; set; }
                // 是否是内置场景
                public bool builtin { get; set; }
                // 是否叠加加载
                public bool additive { get; set; }

                public System.Action<string, bool, bool, float> onInvoke { get; set; }

                // 加载/卸载场景时会调用该接口
                public void OnInvoke(IScene scene, bool load, bool done, float progress ) {
                    Log.Info("===>scene '{0}' load:{1} done:{2} progress:{3}", scene.name, load, done, progress);
                    if (this.onInvoke != null) onInvoke(scene.name, load, done, progress);
                }
            }

            public static object Raycast(Vector3 origin, Vector3 dir, string func, float maxDistance = 1000) {
                object result = null;
                var ray = new Ray(origin, dir);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, maxDistance)) {
                    switch (func) { 
                        case "hit":
                            result = true;
                            break;
                        case "point":
                            result = hit.point;
                            break;
                        case "tag":
                            result = hit.collider.tag;
                            break;
                        case "name":
                            result = hit.collider.name;
                            break;
                    }
                } else { 
                    switch (func) { 
                        case "hit":
                            result = false;
                            break;
                    }
                }

                return result;
            }
        }
    }
}