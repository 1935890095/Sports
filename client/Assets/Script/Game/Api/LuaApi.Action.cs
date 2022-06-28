/********************************************************
    Id: LuaApi.Action.cs
    Desc: 定义给lua层使用的API——Action
    Author: figo
    Date: 2020-07-15 11:31:56
*********************************************************/

namespace ZF.Game {
    using System.Collections.Generic;
    using System;
    using UnityEngine;
    using XLua;
    using ZF.Asset;
    using ZF.Core.Render;
    using ActionAsset = ZF.Asset.Action;

    public partial class LuaApi {

        public static class LuaApiConfigAction {
            [LuaCallCSharp]
            private static List<System.Type> LuaCallCSharp = new List<System.Type>() {
                typeof(IAction),
                // typeof (IActionContext),
            };

            [CSharpCallLua]
            public static List<System.Type> CSharpCallLua = new List<System.Type>() {
                // typeof (System.Action<IRenderObject>), // IRenderObject.onComplete(not test)
                typeof(System.Func<string, IRole>),
                typeof(System.Func<string, IRole[]>),
            };
        }

        [BlackList]
        private static Dictionary<int, GameObject> planes = new Dictionary<int, GameObject>();

        [LuaCallCSharp]
        public static class Action {
            public static IAction Play(string name, IRoot root, System.Func<string, IRole[]> onFire, System.Func<string, IRole[]> onHit, float speed = 1) {
                var action = RenderInstance.Create<ActionAsset>(name, root);
                action.speed = speed;
                var context = new ActionContext(onFire, onHit);
                action.Play(context);
                return action;
            }

            public static void Stop(IAction action) {
                action.Stop();
            }

            public static void ShowArea(IRenderObject renderObject, int sn, float distance, float angle) {
                GameObject plane;
                MeshRenderer renderer = null;
                if (!planes.TryGetValue(sn, out plane)) {
                    plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                    plane.transform.SetParent(renderObject.gameObject.transform);
                    planes[sn] = plane;

                    renderer = plane.GetComponent<MeshRenderer>();
                    renderer.sharedMaterial = Resources.Load<Material>("Material/sector");
                } else {
                    renderer = plane.GetComponent<MeshRenderer>();
                }
                renderer.sharedMaterial.SetFloat("_Angle", angle);
                renderer.sharedMaterial.SetFloat("_Distance", distance);
                int factor = Mathf.CeilToInt(distance / 0.5f);
                plane.transform.localScale = new Vector3(factor, factor, factor) / 10;
                plane.transform.localPosition = new Vector3(0, 0.2f, 0);
                plane.transform.localEulerAngles = new Vector3(0, 0, 0);
                renderer.sharedMaterial.SetInt("_Size", factor);
                plane.SetActive(true);
            }

            public static void HideArea(int sn) {
                if (planes.TryGetValue(sn, out var plane)) {
                    if (plane) {
                        plane.SetActive(false);
                    } else {
                        planes[sn] = null;
                    }
                }
            }

            public static void RemoveArea(int sn) {
                planes[sn] = null;
            }
        }

        class ActionContext : IActionContext {
            private System.Func<string, IRole[]> onFire;
            private System.Func<string, IRole[]> onHit;
            public ActionContext(System.Func<string, IRole[]> onFire, System.Func<string, IRole[]> onHit) {
                this.onFire = onFire;
                this.onHit = onHit;
            }

            public IRole[] OnSing(string script) { return null; }

            public IRole[] OnFire(string script) {
                if (this.onFire != null)
                    return this.onFire(script);
                else
                    return null;
            }

            public IRole[] OnHit(string script) {
                if (this.onHit != null)
                    return this.onHit(script);
                else
                    return null;
            }
        }
    }
}