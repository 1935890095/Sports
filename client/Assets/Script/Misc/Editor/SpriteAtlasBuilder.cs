/********************************************************
    id: SpriteAtlasBuilder.cs
    Desc: 图集构建器
*********************************************************/

namespace ZF.Misc.Editor {
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.U2D;
    using UnityEngine.U2D;

    public class SpriteAtlasBuilder : AssetBuilder {
        static readonly string spriteVariant = "sp";

        public SpriteAtlasBuilder() {
            this.outputPath = "Assets/StreamingAssets/res/ui/atlas";
            this.compress = true;
            this.bundleVariant = "atlas";
        }

        public override void Build(Object asset)
        {
            if (!asset) return;

            var spriteAtlas = asset as SpriteAtlas;
            if (null == spriteAtlas) return;

            SpriteAtlasUtility.PackAtlases(new SpriteAtlas[] { spriteAtlas }, EditorUserBuildSettings.activeBuildTarget);

            base.Build(asset);
        }
    }
}
