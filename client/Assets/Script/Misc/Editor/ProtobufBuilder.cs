/********************************************************
    id: ProtobufBuilder.cs
    Desc: 音效构建器
    Author: nizhenghua
    Date: 2020-10-29 11:21:10
*********************************************************/

namespace ZF.Misc.Editor {
    using UnityEngine;
    using UnityEditor;
    using System.IO;

    public class ProtobufBuilder : AssetBuilder {
        public ProtobufBuilder() {
            this.outputPath = "Assets/StreamingAssets/res/proto";
            this.compress = true;
            this.bundleVariant = "pb";
        }

        public override void Build(Object asset) {
            string path = AssetDatabase.GetAssetPath(asset);
            string cachePath = AssetDatabase.GenerateUniqueAssetPath(path);
            byte[] bytes = File.ReadAllBytes(path);
            byte[] datas = Crypto.DesEncrypt(bytes);
            File.WriteAllBytes(cachePath, datas);
            AssetDatabase.Refresh();
            Build(cachePath, Path.GetFileNameWithoutExtension(path), this.bundleVariant, this.outputPath, this.compress);
            AssetDatabase.DeleteAsset(cachePath);
            AssetDatabase.Refresh();
        }
    }
}
