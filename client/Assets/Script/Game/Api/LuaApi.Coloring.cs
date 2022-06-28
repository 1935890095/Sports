using System.Collections.Generic;
using UnityEngine;
using XLua;
using ZF.Asset.Properties;
using ZF.Core.Render;

namespace ZF.Game {
    public partial class LuaApi {
        [LuaCallCSharp]
        public static class Coloring {

            [BlackList]
            private static readonly Dictionary<string, Material> mats = new Dictionary<string, Material>();

            public static void SetColor(IRenderObject role, string key, int index) {
                Material mat;
                if (!mats.TryGetValue(key, out mat)) {
                    mat = new Material(Shader.Find("ZF/Coloring"));
                    mats[key] = mat;
                }
                ColoringComponent com = role.GetComponent<ColoringComponent>();
                if (com == null) {
                    com = role.AddComponent<ColoringComponent>();
                    com.mat = mat;
                    com.index = index;
                }
            }

            public static void Clear() {
                foreach (var mat in mats.Values) {
                    GameObject.Destroy(mat);
                }
                mats.Clear();
            }
        }
    }

    public class ColoringComponent : RenderComponent {

        public Material mat;
        public int index;

        protected override void OnCreate() {
            var prop = this.renderObject.gameObject.GetComponent<RoleProperty>();
            if (prop.renderers == null || prop.renderers.Length <= 0)
                return;
            if (prop.colors == null || prop.colors.Length <= 0)
                return;
            Color32 color = prop.colors[index];
            mat.SetColor("_Color", color);
            Renderer renderer = this.renderObject.gameObject.GetComponent<RoleProperty>().renderers[0];
            if (mat.mainTexture == null) {
                mat.mainTexture = renderer.sharedMaterial.mainTexture;
            }
            renderer.sharedMaterials = new Material[] { renderer.sharedMaterial, mat };
        }
    }
}