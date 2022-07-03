
namespace XFX.Misc.Editor {
    public class VideoBuilder : AssetBuilder {
        public VideoBuilder() {
            this.outputPath = "Assets/StreamingAssets/res/video";
            this.compress = false;
            this.bundleVariant = "video";
        }
    }
}
