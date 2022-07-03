
namespace XFX.Misc {
    using System.IO;
    using UnityEngine;
    using UnityEditor;

    public class AssetBuilder : ScriptableObject {
        public string outputPath { get; set; }
        public bool compress { get; set; }
        public string bundleName { get; set; }
        public string bundleVariant { get; set; }
        public AssetBuilder(string variant = null) {
            this.bundleVariant = variant;
        }

        /// <summary>
        /// 对指定资源进行创建AssetBundle, 
        /// </summary>
        public virtual void Build(Object asset) {
            if (!asset) return;
            string assetPath = AssetDatabase.GetAssetPath(asset);
            Build(assetPath);
        }

        /// <summary>
        /// 对指定资源进行创建AssetBundle
        /// </summary>
        public void Build(string assetPath) {
            AssetImporter importer = AssetImporter.GetAtPath(assetPath);
            if (importer == null) {
                EditorUtility.DisplayDialog("Error", "资源信息错误", "OK");
                return;
            }

            var asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
            SetAssetBundleNameAndVariant(asset);

            if (string.IsNullOrEmpty(importer.assetBundleName) || string.IsNullOrEmpty(importer.assetBundleVariant)) {
                EditorUtility.DisplayDialog("Error", "资源的AssetBundle信息未设置", "OK");
                return;
            }

            if (!string.IsNullOrEmpty(this.bundleName) && !string.IsNullOrEmpty(this.bundleVariant)) {
                Build(assetPath, bundleName, bundleVariant, outputPath, compress);
            } else {
                Build(assetPath, importer.assetBundleName, importer.assetBundleVariant, outputPath, compress);
            }
        }

        public void Build(string[] assetPaths) {
            foreach(var assetPath in assetPaths) {
                var asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
                SetAssetBundleNameAndVariant(asset);
            }
            Build(assetPaths, this.bundleName, this.bundleVariant, this.outputPath, this.compress);
        }

        protected virtual void SetAssetBundleNameAndVariant(Object asset ) {
            SetAssetBundleNameAndVariant(asset, asset.name, this.bundleVariant);
        }

        /// <summary>
        /// 对指定资源进行创建AssetBundle
        /// </summary>
        public static void Build(string assetPath, string bundleName, string bundleVariant, string folder, bool compress) {
            Build(new string[] { assetPath }, bundleName, bundleVariant, folder, compress);
        }

        /// <summary>
        /// 对指定资源进行创建AssetBundle
        /// </summary>
        public static void Build(string[] assetPaths, string bundleName, string bundleVariant, string folder, bool compress) {
            if (string.IsNullOrEmpty(bundleName) || string.IsNullOrEmpty(bundleVariant)) {
                EditorUtility.DisplayDialog("Error", "资源的AssetBundle信息未设置", "OK");
                return;
            }

            AssetBundleBuild abb = new AssetBundleBuild();
            abb.assetBundleName = bundleName;
            abb.assetBundleVariant = bundleVariant;
            abb.assetNames = assetPaths;

            Build(new AssetBundleBuild[] { abb }, bundleName, bundleVariant, folder, compress);
        }

        public static void Build(AssetBundleBuild[] buildmap, string bundleName, string bundleVariant, string output, bool compress) {
            BuildAssetBundleOptions options = BuildAssetBundleOptions.None;
            if (compress) {
                // 压缩
                options |= BuildAssetBundleOptions.ChunkBasedCompression;
            }
            else {
                options |= BuildAssetBundleOptions.UncompressedAssetBundle | BuildAssetBundleOptions.ForceRebuildAssetBundle; 
            }
            if(!Directory.Exists(output)) {
                Directory.CreateDirectory(output);
            }

            BuildPipeline.BuildAssetBundles(output, buildmap, options, EditorUserBuildSettings.activeBuildTarget);

            AssetDatabase.Refresh();

            // 删除生成出来的manifest文件
            AssetDatabase.DeleteAsset(output + "/" + bundleName + "." + bundleVariant + ".manifest");
            string outputName  = output.Substring(output.LastIndexOf("/") + 1);
            AssetDatabase.DeleteAsset(output + "/" + outputName);
            AssetDatabase.DeleteAsset(output + "/" + outputName + ".manifest");

            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 设置预置体资源的AssetBundle信息(bundle name及bundle variant)
        /// </summary>
        protected static void SetAssetBundleNameAndVariant(Object asset, string bundleName, string bundleVariant) {
            SetAssetBundleNameAndVariant(AssetDatabase.GetAssetPath(asset), bundleName, bundleVariant);
        }

        /// <summary>
        /// 设置预置体资源的AssetBundle信息(bundle name及bundle variant)
        /// </summary>
        protected static void SetAssetBundleNameAndVariant(string assetPath, string bundleName, string bundleVariant) {
            AssetImporter importer = AssetImporter.GetAtPath(assetPath);
            if (importer == null)
                return;
            importer.SetAssetBundleNameAndVariant(bundleName, bundleVariant);
        }

        /// <summary>
        /// 创建一个Asset
        /// </summery>
        /// <param name="asset">asset模板对象</param>
        /// <param name="assetPath">asset路径</param>
        /// <returns>Asset路径</returns>
        public static string CreateAsset(Object asset, string assetPath) {
            assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
            AssetDatabase.CreateAsset(asset, assetPath);
            return assetPath;
        }
    }
}