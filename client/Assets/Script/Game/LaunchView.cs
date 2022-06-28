using UnityEngine;
using ZF.Asset;
using ZF.Core.Render;

namespace ZF.Game {
    class LaunchView : RenderComponent, IView {
        public static LaunchView Create(IRenderObject parent) {
            View view = RenderInstance.Create<View>("res/ui/LaunchView.ui", parent);
            var c = view.AddComponent<LaunchView>();
            return c;
        }

        protected override void OnCreate() {
            var canvas = renderObject.gameObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = UnityEngine.Camera.main;
        }

        #region [IView]
        public bool isShow {
            get {
                if (this.renderObject != null) return this.renderObject.active;
                else return false;
            }
        }
        public void Destroy() { if (this.renderObject != null) this.renderObject.Destroy(); }
        public void Hide() { if (this.renderObject != null) this.renderObject.active = false; }
        public void Show() { if (this.renderObject != null) this.renderObject.active = true; }
        #endregion
    }
}