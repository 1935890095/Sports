/********************************************************
    id: AudioBuilder.cs
    Desc: 音效构建器
    Author: figo
    Date: 2020-03-30 11:21:10
*********************************************************/

namespace ZF.Misc.Editor {
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
