
namespace XFX.Misc.Editor {
    using UnityEngine;
    using UnityEditor;

    public class AudioBuilder : AssetBuilder {
        public AudioBuilder() {
            this.outputPath = "Assets/StreamingAssets/res/audio";
            this.compress = true;
            this.bundleVariant = "audio";
        }
    }
}
