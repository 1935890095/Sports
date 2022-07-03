
namespace XFX.Misc.Editor {
    using System.Collections.Generic;
    using System.IO;
    using System;
    using UnityEditor;
    using UnityEngine;
    using Object = UnityEngine.Object;
    using XFX.Misc;

    public class LuaBuilder : AssetBuilder {
        // static bool encrpt = false;
        static string source = "Lua";
        public string temp = "LuaTemp";

        public LuaBuilder() {
            this.outputPath = "Assets/StreamingAssets/res/lua";
            this.compress = true;
            this.bundleName = "lua";
            this.bundleVariant = "bin";
        }

        public void Build() {
            string sourcePath = Path.Combine("Assets", source);
            string tempPath = Path.Combine("Assets", temp);

            Selection.activeObject = AssetDatabase.LoadAssetAtPath(sourcePath, typeof(Object));
            Directory.CreateDirectory(tempPath);

            try {
                List<string> assetPaths = new List<string>();
                Debug.Log(sourcePath);
                string[] files = Directory.GetFiles(sourcePath, "*.lua", SearchOption.AllDirectories);
                for (int i = 0; i < files.Length; ++i) {
                    string fileName = files[i].Replace(sourcePath + Path.DirectorySeparatorChar, "");
                    string tempName = fileName.Replace(Path.DirectorySeparatorChar, '.').Replace(".lua", ".bytes");
                    string tempFilePath = Path.Combine(tempPath, tempName);
                    using (var tempFile = File.Create(tempFilePath)) {
                        Debug.Log(tempName);
                    }
                    File.WriteAllBytes(tempFilePath, Crypto.DesEncrypt(File.ReadAllBytes(files[i])));

                    // FileUtil.CopyFileOrDirectory(files[i], tempFilePath);

                    EditorUtility.DisplayProgressBar("Build Lua", string.Format("[{0}/{1}] {2}", i, files.Length, files[i]), i * 1.0f / files.Length);

                    assetPaths.Add(tempFilePath);
                }
                AssetDatabase.Refresh();

                this.Build(assetPaths.ToArray());

                EditorUtility.ClearProgressBar();
                AssetDatabase.DeleteAsset(tempPath);
                Debug.Log("Build lua assets success");
            } catch (Exception e) {
                EditorUtility.ClearProgressBar();
                AssetDatabase.DeleteAsset(tempPath);
                AssetDatabase.Refresh();
                Debug.LogError("Build lua assets error");
                Debug.LogError(e.Message);
            }
        }
    }
}