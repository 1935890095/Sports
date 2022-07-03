using System;
using XFX.Core.Render;

namespace XFX.Game {

    class AtlasMgr : Instance {
        static readonly string atlasPath = "res/ui/atlas";
        static readonly string atlasVariant = "atlas";

        protected override void OnInit() {
            UnityEngine.U2D.SpriteAtlasManager.atlasRequested += OnAtlasRequested;
        }

        protected override void OnDestroy() {
            UnityEngine.U2D.SpriteAtlasManager.atlasRequested -= OnAtlasRequested;
        }

        private void OnAtlasRequested(string tag, Action<UnityEngine.U2D.SpriteAtlas> action) {
            var atlas = RenderInstance.Create<XFX.Asset.SpriteAtlas>(string.Format("{0}/{1}.{2}", atlasPath, tag, atlasVariant));
            if (atlas.complete) {
                action(atlas.atlas);
                atlas.Destroy();
            } else {
                atlas.onComplete = obj => {
                    action(((XFX.Asset.SpriteAtlas) obj).atlas);
                    atlas.Destroy();
                };
            }
        }
    }
}