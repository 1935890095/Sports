/********************************************************
    Id: UIBuilder.cs
    Desc: UI构建器
    Author: figo
    Date: 2020-04-27 11:40:44
*********************************************************/

namespace ZF.Misc.Editor {
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.U2D;
    using UnityEngine.UI;
    using ZF.Asset.Properties;

    public class UIBuilder : AssetBuilder {
        private const string AtlasDataPath = "/UI/Atlas";
        private const string AtlasAtlasFindRegex = "*.spriteatlas";
        private const string AtlasRelativePath = "Assets/UI/Atlas";
        private const string AtlasBundleRelativePath = "atlas/";
        private const string AtlasBundleVariant = "atlas";

        private const string ShaderDataPath = "/Shader/custom";
        private const string ShaderFindRegex = "*.shader";
        private const string ShaderRelativePath = "Assets/Shader/custom";
        private const string ShaderBundleRelativePath = "shader/";
        private const string ShaderBundleVariant = "sd";

        private const string TextureRelativePath = "Assets/UI/Texture";
        private const string TextureBundleRelativePath = "tex/";
        private const string TextureBundleVariant = "tex";
        private const string FontBundleRelativePath = "font/";
        private const string FontBundleVariant = "font";
        private const string ManifestExt = "manifest";
        private const string DependencyPathFormat = "res/ui/{0}{1}.{2}";
        private const string DependencyShaderPathFormat = "res/{0}{1}.{2}";

        public UIBuilder() {
            this.outputPath = "Assets/StreamingAssets/res/ui";
            this.compress = true;
            this.bundleVariant = "ui";
        }

        public override void Build(UnityEngine.Object viewAsset)
        {
            var view = viewAsset as UnityEngine.GameObject;
            if (null == view) return;

            // if (viewAsset.name.Equals("LaunchView")) {
            //     base.Build(viewAsset);
            //     return;
            // }

            List<AssetBundleBuild> buildList = new List<AssetBundleBuild>();
            List<Dependence> dependencies = new List<Dependence>();

            #region [图集]
            var imgs = view.GetComponentsInChildren<Image>(true);
            var atlasList = new List<SpriteAtlas>();
            var usedAtlasList = new List<SpriteAtlas>();

            DirectoryInfo folder = new DirectoryInfo(Application.dataPath + AtlasDataPath);
            var files = folder.GetFiles(AtlasAtlasFindRegex);
            for (int i = 0; i < files.Length; i++) {
                atlasList.Add(AssetDatabase.LoadAssetAtPath<SpriteAtlas>(AtlasRelativePath + "/" + files[i].Name));
            }

            for (int i = 0, max = atlasList.Count; i < max; i++) {
                var atlas = atlasList[i];
                if (null != atlas) {
                    for (int _i = 0, _max = imgs.Length; _i < _max; _i++) {
                        var img = imgs[_i];
                        var sprite = img.sprite;
                        if (null != sprite && atlas.CanBindTo(sprite)) {
                            usedAtlasList.Add(atlas);
                            buildList.Add(CreateAssetBundleBuild(atlas, AtlasBundleRelativePath, AtlasBundleVariant));

                            var dpath = string.Format(DependencyPathFormat, AtlasBundleRelativePath, atlas.name, AtlasBundleVariant);
                            var dep = new Dependence();
                            dep.path = dpath;
                            dependencies.Add(dep);
                            break;
                        }
                    }
                }
            }

            if (usedAtlasList.Count > 0) {
                for (int i = 0, max = usedAtlasList.Count; i < max; i++) {
                    UnityEditor.U2D.SpriteAtlasUtility.PackAtlases(new SpriteAtlas[] { usedAtlasList[i] }, EditorUserBuildSettings.activeBuildTarget);
                }
            }
            #endregion
           
            #region [shader]

            var graphics = view.GetComponentsInChildren<Graphic>(true);
            
            var shaderList = new List<Shader>();

            DirectoryInfo shaderFolder = new DirectoryInfo(Application.dataPath + ShaderDataPath);
            var shaderFiles = shaderFolder.GetFiles(ShaderFindRegex);
            for (int i = 0; i < shaderFiles.Length; i++)
            {
                shaderList.Add(AssetDatabase.LoadAssetAtPath<Shader>(ShaderRelativePath + "/" + shaderFiles[i].Name));
            }

            for (int i = 0, max = shaderList.Count; i < max; i++)
            {
                var shader = shaderList[i];
                if (null != shader)
                {
                    for (int _i = 0, _max = graphics.Length; _i < _max; _i++)
                    {
                        var graphic = graphics[_i];
                        Material material = graphic.material;
                        if (null != material.shader && material.shader == shader)
                        {

                            List<AssetBundleBuild> shaderBuildList = new List<AssetBundleBuild>();
                            var build = new AssetBundleBuild();
                            build.assetBundleName = ShaderBundleRelativePath +  graphic.material.shader.name.Replace('/','_');
                            build.assetBundleVariant = ShaderBundleVariant;
                            build.assetNames = new string[] { AssetDatabase.GetAssetPath(graphic.material.shader) };
                            shaderBuildList.Add(build);

                            var dpath = string.Format(DependencyShaderPathFormat, ShaderBundleRelativePath, graphic.material.shader.name.Replace('/','_'), ShaderBundleVariant);
                            Dependence dependence = new Dependence();
                            dependence.name = material.shader.name;
                            dependence.dependence = material.shader;
                            dependence.assets = new Object[] { material };
                            dependence.path = dpath.ToLower();
                            dependencies.Add(dependence);
                            
                            AssetBuilder.Build(shaderBuildList.ToArray(), view.name, this.bundleVariant, "Assets/StreamingAssets/res", this.compress);
                            break;
                        }
                    }
                }
            }

            #endregion

            #region [其他依赖项]
            // var viewPath = AssetDatabase.GetAssetPath(view);
            // var assets = AssetDatabase.GetDependencies(viewPath);
            // for (int i = 0, max = assets.Length; i < max; i++) {
            //     var asset = AssetDatabase.LoadAssetAtPath(assets[i], typeof(UnityEngine.Object));
            //     var path = Path.GetDirectoryName(assets[i]).Replace(@"\", "/");
            //     if (asset is Texture && path.Equals(TextureRelativePath)) {
            //         var dpath = string.Format(DependencyPathFormat, TextureBundleRelativePath, asset.name, TextureBundleVariant);
            //         if (dependencies.ContainsKey(dpath)) continue;
            //         buildList.Add(CreateAssetBundleBuild(asset, TextureBundleRelativePath, TextureBundleVariant));
            //         var dep = new Dependence();
            //         dep.path = dpath;
            //         dependencies.Add(dpath, dep);
            //     } else 
            //     if (asset is Font) {
            //         var dpath = string.Format(DependencyPathFormat, FontBundleRelativePath, asset.name, FontBundleVariant);
            //         if (dependencies.ContainsKey(dpath)) continue;
            //         buildList.Add(CreateAssetBundleBuild(asset, FontBundleRelativePath, FontBundleVariant));
            //         var dep = new Dependence();
            //         dep.path = dpath;
            //         dependencies.Add(dpath, dep);
            //     }
            // }
            #endregion

            var property = view.GetComponent<ZF.Asset.Properties.ViewProperty>();
            if (null == property)
                property = view.AddComponent<ZF.Asset.Properties.ViewProperty>();

            property.Validate(null);
            for (int i = 0, max = property.dependencies.Length; i < max; i++) {
                var dep = property.dependencies[i];
                dep.name = dep.name.ToLower();
                var asset = dep.dependence;
                if (asset is Texture) {
                    var dpath = string.Format(DependencyPathFormat, TextureBundleRelativePath, asset.name, TextureBundleVariant);
                    buildList.Add(CreateAssetBundleBuild(asset, TextureBundleRelativePath, TextureBundleVariant));
                    dep.path = dpath.ToLower();
                } else if (asset is Font) {
                    var dpath = string.Format(DependencyPathFormat, FontBundleRelativePath, asset.name, FontBundleVariant);
                    buildList.Add(CreateAssetBundleBuild(asset, FontBundleRelativePath, FontBundleVariant));
                    dep.path = dpath.ToLower();
                }
            }

            dependencies.AddRange(property.dependencies);
            property.dependencies = dependencies.ToArray();

            EditorUtility.SetDirty(viewAsset);
            AssetDatabase.SaveAssets();

            buildList.Add(CreateAssetBundleBuild(view, string.Empty, this.bundleVariant));
            AssetBuilder.Build(buildList.ToArray(), view.name, this.bundleVariant, this.outputPath, this.compress);

            for (int i = 0, max = buildList.Count; i < max; i++) {
                var build = buildList[i];
                if (build.assetBundleVariant.Equals(this.bundleVariant))
                    continue;

                var manifestPath = string.Format("{0}/{1}.{2}.{3}", this.outputPath, build.assetBundleName, build.assetBundleVariant, ManifestExt);
                AssetDatabase.DeleteAsset(manifestPath);
            }
        }

        private AssetBundleBuild CreateAssetBundleBuild(UnityEngine.Object obj, string relativePath, string variant)
        {
            var build = new AssetBundleBuild();
            build.assetBundleName = relativePath + obj.name;
            build.assetBundleVariant = variant;
            build.assetNames = new string[] { AssetDatabase.GetAssetPath(obj) };
            return build;
        }
    }
}