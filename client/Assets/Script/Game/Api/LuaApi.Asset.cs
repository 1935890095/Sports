
namespace XFX.Game {
    using System.Collections.Generic;
    using XLua;
    using XFX.Asset;
    using XFX.Core.Render;

    public static class LuaApiConfigAsset {
        [LuaCallCSharp]
        private static List<System.Type> LuaCallCSharp = new List<System.Type>() {
            typeof(IRenderObject),
            typeof(IRenderComponent),
            typeof(IAudio),
            typeof(IVideoClip),
            typeof(IVideoPlayer),
            typeof(IEffect),
            typeof(ITexture),
            typeof(IRenderTexture),
            typeof(IRole),
            // typeof(ITextMeshPro),
            typeof(IPrimitive3D),
            typeof(ICamera),
            typeof(ILight),
            typeof(IRoot),
            typeof(IReminder),
            typeof(IFaceEffect),
            typeof(ICanvas),
        };

        [CSharpCallLua]
        public static List<System.Type> CSharpCallLua = new List<System.Type>() {
            typeof(System.Action<IRenderObject>), // IRenderObject.onComplete(not test)
        };
    }

    public partial class LuaApi {
        [LuaCallCSharp]
        public static class Asset {
            public static IRoot root { get { return game.root; } }
            public static IRoot viewroot { get { return game.viewroot; } }
            // 创建一个root节点
            public static IRoot CreateRoot(string name, IRenderObject parent) {
                var root = Root.Create(null, parent);
                root.name = name;
                return root;
            }

            public static IAudio CreateAudio(string filename, IRenderObject parent) { return RenderInstance.Create<Audio>(filename, parent); }
            public static IEffect CreateEffect(string filename, IRenderObject parent) { return RenderInstance.Create<Effect>(filename, parent); }
            public static ITexture CreateTexture(string filename, IRenderObject parent) { return RenderInstance.Create<Texture>(filename, parent); }
            public static IText CreateText(string filename) { return RenderInstance.Create<Text>(filename); }
            public static ICanvas CreateCanvas(string filename, IRenderObject parent) { return RenderInstance.Create<FaceCanvas>(filename, parent);}

            // public static ITextMeshPro CreateTMP(string filename, IRenderObject parent) {
            //     return RenderInstance.Create<TextMeshPro>(filename, parent);
            // }

            public static IPrimitive3D CreatePrimitive3D(string filename, IRenderObject parent) {
                return RenderInstance.Create<Primitive3D>(filename, parent);
            }

            public static ICamera CreateCamera(string name, IRenderObject parent) {
                ICamera camera = XFX.Asset.Camera.Create(name, parent);
                // 相机默认设置
                var component = camera.gameObject.GetComponent<UnityEngine.Camera>();
                component.clearFlags = UnityEngine.CameraClearFlags.Depth;
                component.allowHDR = false;
                component.allowMSAA = false;
                component.allowDynamicResolution = true;
                camera.name = name;
                return camera;
            }

            public static ILight CreateLight(string name, IRenderObject parent) {
                return XFX.Asset.Light.Create(name, parent);
            }
        }
    }
}