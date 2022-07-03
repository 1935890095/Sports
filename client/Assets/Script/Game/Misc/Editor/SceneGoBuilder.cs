
namespace XFX.Misc.Editor {
    using UnityEditor;
    using UnityEngine;
    using XFX.Asset.Properties;
    using System.Collections.Generic;

    public class SceneGoBuilder : AssetBuilder {
        public SceneGoBuilder() {
            this.outputPath = "Assets/StreamingAssets/res/sg";
            this.compress = true;
            this.bundleVariant = "sg";
        }

        public override void Build(Object asset) {
            if (!asset) return;
            
            if (asset is GameObject) {
                string assetpath = AssetDatabase.GetAssetPath(asset);
                var go = asset as GameObject;
                var property =  go.GetComponent<DependsProperty>();
                if (property == null) property = go.AddComponent<DependsProperty>();
                property.Collect(DependFlags.Shader);

                List<AssetBundleBuild> buildmap = new List<AssetBundleBuild>();
                for (int i = 0; i < property.dependencies.Length; ++i) {
                    var d = property.dependencies[i];
                    if(d.dependence is Texture) {
                        string depassetpath =  AssetDatabase.GetAssetPath(d.dependence) ;
                        string depname = d.dependence.name.ToLower();
                        AssetBundleBuild abb = new AssetBundleBuild();
                        abb.assetBundleName = "tex/" + depname;
                        abb.assetBundleVariant = "tex";
                        abb.assetNames = new string[] { depassetpath};
                        buildmap.Add(abb);

                        d.path = "res/sg/tex/" + depname + ".tex";
                        UnityEngine.Debug.Log("** collect dep: " + d.path);
                    }
                    if(d.dependence is Shader) {
                        string depassetpath =  AssetDatabase.GetAssetPath(d.dependence) ;
                        string depname = d.dependence.name.Replace("/", "_").ToLower();
                        AssetBundleBuild abb = new AssetBundleBuild();
                        abb.assetBundleName = "../shader/" + depname;
                        abb.assetBundleVariant = "sd";
                        abb.assetNames = new string[] { depassetpath};
                        buildmap.Add(abb);

                        d.path = "res/shader/" + depname + ".sd";
                        UnityEngine.Debug.Log("** collect dep: " + d.path);
                    }
                }

                EditorUtility.SetDirty(asset);
                AssetDatabase.SaveAssets();

                {
                    AssetBundleBuild abb = new AssetBundleBuild();
                    abb.assetBundleName = asset.name;
                    abb.assetBundleVariant = this.bundleVariant;
                    abb.assetNames = new string[] { assetpath };
                    buildmap.Add(abb);
                }


                Build(buildmap.ToArray(), asset.name, this.bundleVariant, this.outputPath, this.compress);
                return;
            }
            string assetPath = AssetDatabase.GetAssetPath(asset);
            Build(assetPath);
        }
    }
}