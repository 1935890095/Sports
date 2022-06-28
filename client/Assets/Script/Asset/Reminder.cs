namespace ZF.Asset {
    using UnityEngine;
    using UnityEngine.UI;
    using ZF.Core.Render;

    public interface IReminder { 
        int anchor { get; set; }
        Vector2 anchoredPosition { get; set; }
        string res { get; set; }
    }

    public class Reminder : RenderObject, IReminder {
        public int anchor { 
            get { return _anchor; }
            set {
                anchorChanged = true;
                _anchor = value;
            }
        }
        public Vector2 anchoredPosition {
            get {
                return _anchoredPosition; 
            }
            set {
                anchorChanged = true;
                _anchoredPosition = value;
            }
        }
        public string res {
            get { return _res; }
            set {
                _res = value;
                SetSprite();
            }
        }

        private bool anchorChanged = true;
        private int _anchor = 1;
        private Vector2 _anchoredPosition = Vector2.zero;
        private string _res = null;

        static readonly string atlasPath = "res/ui/atlas";
        static readonly string atlasVariant = "atlas";

        private RectTransform rectTransform;
        private Image image;

        protected override void OnCreate(IRenderResource resource) {
            this.gameObject = GameObject.Instantiate(resource.asset) as GameObject;
            this.rectTransform = this.gameObject.GetComponent<RectTransform>();
            this.image = this.gameObject.GetComponent<Image>();
            if (!this.rectTransform) this.rectTransform = this.gameObject.AddComponent<RectTransform>();
            if (!this.image) this.image = this.gameObject.AddComponent<Image>();

            SetSprite();
        }

        protected override void OnUpdate() {
            base.OnUpdate();
            if (anchorChanged && this.complete && this.active) { // anchor、anchoredPosition设置要在显示时才有效
                anchorChanged = false;
                SetAnchor();
                SetAnchoredPosition();
            }
        }

        private void SetAnchor() { 
            if (!this.rectTransform) return;
            if (_anchor == 1) { // topright
                this.rectTransform.anchorMin = Vector2.one;
                this.rectTransform.anchorMax = Vector2.one;
            } else if (_anchor == 2) { // topleft
                var ac = new Vector2(0, 1);
                this.rectTransform.anchorMin = ac;
                this.rectTransform.anchorMax = ac;
            }
        }

        private void SetAnchoredPosition() { 
            if (!this.rectTransform) return;
            this.rectTransform.anchoredPosition = _anchoredPosition;
        }

        private void SetSprite() { 
            if (!this.image) return;
            if (null != _res) {
                var path = _res.Split('/');
                if (path.Length == 2) { 
                    var atlasAsset = RenderInstance.Create<ZF.Asset.SpriteAtlas>(string.Format("{0}/{1}.{2}", atlasPath, path[0], atlasVariant));
                    atlasAsset.SetSprite(this.image, path[1], true);
                    atlasAsset.Destroy();
                }
            }
        }

        protected override void OnDestroy() {
            if (this.gameObject != null) {
                RenderObject.DestroyObject(this.gameObject);
                this.gameObject = null;
            }
        }
    }
}