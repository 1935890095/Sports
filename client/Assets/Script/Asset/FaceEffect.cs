using UnityEngine;
using ZF.Core.Render;
using ZF.Game;

namespace ZF.Asset {
    public interface IFaceEffect : IEffect {
        UnityEngine.Texture texture { get; set; }
        string sortingLayerName { set; get; }

        void Show();
    }

    public class FaceEffect : Effect, IFaceEffect {
        private UnityEngine.Texture _tex;
        private string _sortingLayerName;

        protected override void OnCreate(IRenderResource resource) {
            base.OnCreate(resource);
            SetTexture();
            SetSortingLayer();
        }

        public void Show() {
            base.Play();
        }

        public UnityEngine.Texture texture {
            get { return _tex; }
            set {
                _tex = value;
                SetTexture();
            }
        }

        public string sortingLayerName {
            get { return _sortingLayerName; }
            set {
                _sortingLayerName = value;
                SetSortingLayer();
            }
        }

        private void SetTexture() {
            if (!gameObject || !_tex) {
                return;
            }

            var renderer = gameObject.GetComponentInChildren<ParticleSystemRenderer>();
            renderer.material.mainTexture = _tex;
        }

        private void SetSortingLayer() {
            if (!gameObject || string.IsNullOrWhiteSpace(_sortingLayerName)) {
                return;
            }
            var systems = this.gameObject.GetComponentsInChildren<ParticleSystem>();
            foreach (var sys in systems) {
                var renderer = sys.gameObject.GetComponentInChildren<ParticleSystemRenderer>();
                renderer.sortingLayerID = SortingLayer.NameToID(sortingLayerName);
            }
        }
    }
}