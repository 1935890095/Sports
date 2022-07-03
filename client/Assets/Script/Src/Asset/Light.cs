using UnityEngine;
using XFX.Core.Render;

namespace XFX.Asset {
    public interface ILight : IRenderObject {
        float intensity { get; set; }
        Color color { get; set; }
        int cullingMask { get; set; }
        int shadowType { get; set; }
        void Show(bool show);
    }

    public class Light : RenderObject, ILight {

        private UnityEngine.Light light;
        private bool show = true;

        public static ILight Create(string name, IRenderObject parent) {
            ILight light = RenderInstance.Create<Light>("empty", parent);
            light.name = name;
            return light;
        }

        protected override void OnCreate(IRenderResource resource) {
            this.gameObject = new GameObject(this.name);
            this.light = this.gameObject.AddComponent<UnityEngine.Light>();
            this.light.type = LightType.Directional; // 默认为方向光，后续有需求开放接口
            if (!this.show) {
                this.light.enabled = false;
            }
        }

        protected override void OnDestroy() {
            if (this.gameObject != null) {
                RenderObject.DestroyObject(this.gameObject);
                this.gameObject = null;
                this.light = null;
            }
        }

        public float intensity {
            get { return light.intensity; }
            set { light.intensity = value; }
        }
        public Color color {
            get { return light.color; }
            set { light.color = value; }
        }
        public int cullingMask {
            get { return light.cullingMask; }
            set { light.cullingMask = value; }
        }

        public int shadowType {
            get { return (int) light.shadows; }
            set { light.shadows = (LightShadows) value; }
        }

        public void Show(bool show) {
            if (this.show != show) {
                this.show = show;
                if (this.light) {
                    this.light.enabled = show;
                }
            }
        }
    }
}