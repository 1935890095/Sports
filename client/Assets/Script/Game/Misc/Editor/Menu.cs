
using UnityEngine;
using UnityEditor;
using XFX.UI;
using XLua;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using UnityEngine.U2D;
using System.Text.RegularExpressions;

namespace XFX.Misc.Editor.Menu {
    // ui menu
    public class UI {
        [MenuItem("XFX/UI/添加Export %e", false)]
        // [MenuItem("Assets/XFX/UI/添加Export %e", false)]
        public static void AddExportNode() {
            GameObject go = Selection.activeGameObject;
            if (go != null) {
                if (go.GetComponent<ExportNode>() == null) {
                    go.AddComponent<ExportNode>();
                } else {
                    Debug.LogWarning("already has ExportNode");
                }
            }
        }

        [MenuItem("XFX/UI/添加Export组 %#e", false)]
        // [MenuItem("Assets/XFX/UI/添加Export组 %#e", false)]
        public static void AddExportGroup() {
            GameObject go = Selection.activeGameObject;
            if (go != null) {
                if (go.GetComponent<ExportGroup>() == null) {
                    go.AddComponent<ExportGroup>();
                } else {
                    Debug.LogWarning("already has ExportGroup");
                }
            }
        }

        [MenuItem("XFX/UI/导出Cs资源", false)]
        // [MenuItem("Assets/XFX/UI/导出Cs资源", false)]
        public static void ExportCsCode() {
            XFX.UI.Editor.CodeExport.ExportCSCode();
        }

        [MenuItem("XFX/UI/导出Lua资源", false)]
        // [MenuItem("Assets/XFX/UI/导出Lua资源", false)]
        public static void ExportLuaCode() {
            XFX.UI.Editor.CodeExport.ExportLuaCode();
        }

        [MenuItem("XFX/UI/打印路径", false)]
        public static void LogPath()
        {
            Transform transform = Selection.activeGameObject.transform;
            string str = transform.name;
            transform = transform.parent;
            while(transform) {
                str = transform.name + "/" + str;
                transform = transform.parent;
            }
            Debug.Log(str);
        }
    }

    public class Build {
        // Lua menu
        [MenuItem("XFX/Build/Build Lua")]
        [MenuItem("Assets/XFX/Build/Build Lua")]
        public static void BuildLua() {
            var builder = new LuaBuilder();
            builder.Build();
        }

        [MenuItem("XFX/Build/Build Lua Res")]
        [MenuItem("Assets/XFX/Build/Build Lua Res")]
        public static void BuildLuaRes() {
            var builder = new LuaResourceBuilder();
            builder.Build();
        }

        [MenuItem("XFX/Build/Build UI")]
        [MenuItem("Assets/XFX/Build/Build UI")]
        public static void BuildUI() {
            var builder = new UIBuilder();
            foreach (var asset in Selection.objects) {
                GameObject go = asset as GameObject;
                if (go == null) continue;
                builder.Build(go);
            }
        }

        [MenuItem("XFX/Build/Build Audio", false)]
        [MenuItem("Assets/XFX/Build/Build Audio", false)]
        public static void BuildAudio() {
            var builder = new AudioBuilder();
            foreach (var asset in Selection.objects) {
                builder.Build(asset);
            }
        }

        [MenuItem("XFX/Build/Build Video", false)]
        [MenuItem("Assets/XFX/Build/Build Video", false)]
        public static void BuildVideo() {
            var builder = new VideoBuilder();
            foreach (var asset in Selection.objects) {
                builder.Build(asset);
            }
        }

        [MenuItem("XFX/Build/Build Texture", false)]
        [MenuItem("Assets/XFX/Build/Build Texture", false)]
        public static void BuildTexture() {
            var builder = new TextureBuilder();
            foreach (var asset in Selection.objects) {
                builder.Build(asset);   
            }
        }

        [MenuItem("XFX/Build/Build Scene", false)]
        [MenuItem("Assets/XFX/Build/Build Scene", false)]
        public static void BuildScene() {
            var builder = ScriptableObject.CreateInstance("SceneBuilder") as SceneBuilder;
            foreach (var asset in Selection.objects) {
                builder.Build(asset);   
            }
        }


        [MenuItem("XFX/Build/Build PreloadAssets", false)]
        [MenuItem("Assets/XFX/Build/Build PreloadAssets", false)]
        public static void BuildPreloadAssets()
        {
            var builder = new PreloadAssetsBuilder();
            foreach (var asset in Selection.objects) {
                builder.Build(asset);
            }
        }
        
        [MenuItem("XFX/Build/Build Scene Go", false)]
        [MenuItem("Assets/XFX/Build/Build Scene Go", false)]
        public static void BuildSceneGo() {
            var builder = ScriptableObject.CreateInstance("SceneGoBuilder") as SceneGoBuilder;
            foreach (var asset in Selection.objects) {
                builder.Build(asset);   
            }
        }

        [MenuItem("XFX/Build/Build Atlas", false)]
        [MenuItem("Assets/XFX/Build/Build Atlas", false)]
        public static void BuildAtlas()
        {
            var builder = ScriptableObject.CreateInstance("SpriteAtlasBuilder") as SpriteAtlasBuilder;
            foreach (var asset in Selection.objects) {
                builder.Build(asset);
            }
        }

        [MenuItem("XFX/Build/Build Protobuf", false)]
        [MenuItem("Assets/XFX/Build/Build Protobuf", false)]
        public static void BuildProtobuf()
        {
            var builder = new ProtobufBuilder();
            foreach (var asset in Selection.objects) {
                builder.Build(asset);
            }
        }
    }

    public class PlayerPrefs { 
        [MenuItem("XFX/清除缓存", false)]
        public static void Clear()
        {
            UnityEngine.PlayerPrefs.DeleteAll();
            System.IO.Directory.Delete(XFX.Core.Util.PathExt.MakeCachePath("/model"), true);
        }
    }
}
