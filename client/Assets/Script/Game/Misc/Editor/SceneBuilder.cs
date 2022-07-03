namespace XFX.Misc.Editor {
    public class SceneBuilder : AssetBuilder {
        public SceneBuilder() {
            this.outputPath = "Assets/StreamingAssets/res/scene";
            this.compress = true;
            this.bundleVariant = "scene";
        }
    }
}