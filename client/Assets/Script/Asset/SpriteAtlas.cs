
namespace ZF.Asset {
    using System.Collections.Generic;
    using ZF.Core.Render;

    public interface ISpriteAtlas : IRenderObject
    {
        UnityEngine.U2D.SpriteAtlas atlas { get; }
        UnityEngine.Sprite GetSprite(string name);
        void SetSprite(UnityEngine.UI.Image img, string sprite, bool nativeSize = false);
    }

    public class SpriteAtlas : RenderObject, ISpriteAtlas, IRenderObject
    {
        public UnityEngine.U2D.SpriteAtlas atlas { get { return _atlas; } }
        private UnityEngine.U2D.SpriteAtlas _atlas;
        private Queue<KeyValuePair<UnityEngine.UI.Image, string>> queue;
        private Queue<bool> native;

        protected override void OnCreate(IRenderResource resource)
        {
            _atlas = resource.asset as UnityEngine.U2D.SpriteAtlas;

            if (null != queue) {
                while (queue.Count > 0) {
                    var kv = queue.Dequeue();
                    SetSprite(kv.Key, kv.Value, native.Dequeue());
                }
                queue = null;
                native = null;
            }
        }

        public UnityEngine.Sprite GetSprite(string name)
        {
            if (null == _atlas)
                return null;
            return _atlas.GetSprite(name);
        }

        public void SetSprite(UnityEngine.UI.Image img, string name, bool nativeSize = false) {
            if (null == img || string.IsNullOrEmpty(name)) return;

            if (null == _atlas) {
                if (null == queue) queue = new Queue<KeyValuePair<UnityEngine.UI.Image, string>>();
                if (null == native) native = new Queue<bool>();
                queue.Enqueue(new KeyValuePair<UnityEngine.UI.Image, string>(img, name));
                native.Enqueue(nativeSize);
            } else {
                var sprite = GetSprite(name);
                img.sprite = sprite;
                if (nativeSize) img.SetNativeSize();
            }
        }
    }
}
