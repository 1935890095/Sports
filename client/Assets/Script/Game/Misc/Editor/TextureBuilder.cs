namespace XFX.Misc.Editor {
    public class TextureBuilder : AssetBuilder {
        public TextureBuilder() {
            this.outputPath = "Assets/StreamingAssets/res/ui/tex";
            this.compress = true;
            this.bundleVariant = "tex";
        }
    }

    public class CardTextureBuilder : AssetBuilder {
         public CardTextureBuilder() {
            this.outputPath = "Assets/StreamingAssets/res/card";
            this.compress = true;
            this.bundleVariant = "tex";
        }
    }

    public class HeadTextureBuilder : AssetBuilder {
         public HeadTextureBuilder() {
            this.outputPath = "Assets/StreamingAssets/res/head";
            this.compress = true;
            this.bundleVariant = "tex";
        }
    }

    public class FaceTextureBuilder : AssetBuilder {
         public FaceTextureBuilder() {
            this.outputPath = "Assets/StreamingAssets/res/ui/face";
            this.compress = true;
            this.bundleVariant = "tex";
        }
    }
}