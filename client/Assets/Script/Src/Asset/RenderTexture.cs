using UnityEngine;
using XFX.Core.Render;

namespace XFX.Asset {
    public interface IRenderTexture : IRenderObject {
        UnityEngine.RenderTexture renderTexture { get; }
    }

    public class RenderTexture : RenderObject, IRenderTexture {
        public static IRenderTexture Create(string name, IRenderObject parent) {
            IRenderTexture renderTexture = RenderInstance.Create<RenderTexture>("empty", parent);
            renderTexture.name = name;
            return renderTexture;
        }

        public UnityEngine.RenderTexture renderTexture { get; private set; }
        public int width {
            get { return _width; }
            set {
                _width = value;
                if (null != renderTexture) renderTexture.width = value;
            }
        }
        public int height {
            get { return _height; }
            set {
                _height = value;
                if (null != renderTexture) renderTexture.height = value;
            }
        }
        public int depth {
            get { return _depth; }
            set {
                _depth = value;
                if (null != renderTexture) renderTexture.depth = value;
            }
        }
        private int _width = 1080;
        private int _height = 2340;
        private int _depth = 24;

        protected override void OnCreate(IRenderResource resource) {
            renderTexture = new UnityEngine.RenderTexture(_width, _height, _depth);
        }

        protected override void OnDestroy() {
            GameObject.Destroy(renderTexture);
        }
    }
}