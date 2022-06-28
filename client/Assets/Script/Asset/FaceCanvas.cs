using UnityEngine;
using UnityEngine.UI;
using ZF.Core.Render;

namespace ZF.Asset {
    public interface ICanvas : IRenderObject { 
        Canvas canvas { get; }
        int sortingOrder { set; get; }
        string sortingLayerName{ set; get; }
    }

    public class FaceCanvas : RenderObject, ICanvas {
        public static ICanvas Create(string name, IRenderObject parent) {
            ICanvas guideCanvas = RenderInstance.Create<FaceCanvas>("empty", parent);
            guideCanvas.name = name;
            return guideCanvas;
        }

        public Canvas canvas { private set; get; }
        public int sortingOrder {
            set {
                canvas.overrideSorting = true;
                canvas.sortingOrder = value;
            }
            get {
                return canvas.sortingOrder;
            }
        }
        public string sortingLayerName
        {
            set{
                canvas.overrideSorting = true;
                canvas.sortingLayerName = value;
            }
            get{
                return canvas.sortingLayerName;
            }
        }
        private GraphicRaycaster raycaster;
        private bool remove = false;
        private int oldOrder = 0;
        private string oldSortName = "TopMost";

        protected override void OnCreate(IRenderResource resource) {
            canvas = this.parent.gameObject.GetComponent<Canvas>();
            if (null == canvas) {
                remove = true;
                canvas = this.parent.gameObject.AddComponent<Canvas>();
                raycaster = this.parent.gameObject.AddComponent<GraphicRaycaster>();
                raycaster.SetBlockingMask(ZF.Misc.Defines.Layer.UI);
            } else {
                oldOrder = canvas.sortingOrder;
                oldSortName = canvas.sortingLayerName;
            }
        }
        
        protected override void OnDestroy() {
            base.OnDestroy();
            if (remove) {
                UnityEngine.GameObject.DestroyImmediate(raycaster);
                UnityEngine.GameObject.DestroyImmediate(canvas);
            } else { 
                canvas.sortingOrder = oldOrder;
                canvas.sortingLayerName = oldSortName;
            }
        }
    }
}