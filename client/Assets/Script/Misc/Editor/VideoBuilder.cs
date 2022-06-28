/********************************************************
    id: AudioBuilder.cs
    Desc: 音效构建器
    Author: figo
    Date: 2020-03-30 11:21:10
*********************************************************/

namespace ZF.Misc.Editor {
    public class VideoBuilder : AssetBuilder {
        public VideoBuilder() {
            this.outputPath = "Assets/StreamingAssets/res/video";
            this.compress = false;
            this.bundleVariant = "video";
        }
    }
}
