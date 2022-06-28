/********************************************************
    id: Menu.cs
    desc: 菜单
    author: figo
    date: 2019/03/05 21:05:04

    Copyright (C) 2019 zwwx Ltd. All rights reserved.
*********************************************************/

using UnityEngine;
using UnityEditor;
using ZF.UI;
using XLua;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using UnityEngine.U2D;
using System.Text.RegularExpressions;

namespace ZF.Misc.Editor.Menu {
    // ui menu
    public class UI {
        [MenuItem("ZF/UI/添加Export %e", false)]
        // [MenuItem("Assets/ZF/UI/添加Export %e", false)]
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

        [MenuItem("ZF/UI/添加Export组 %#e", false)]
        // [MenuItem("Assets/ZF/UI/添加Export组 %#e", false)]
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

        [MenuItem("ZF/UI/导出Cs资源", false)]
        // [MenuItem("Assets/ZF/UI/导出Cs资源", false)]
        public static void ExportCsCode() {
            ZF.UI.Editor.CodeExport.ExportCSCode();
        }

        [MenuItem("ZF/UI/导出Lua资源", false)]
        // [MenuItem("Assets/ZF/UI/导出Lua资源", false)]
        public static void ExportLuaCode() {
            ZF.UI.Editor.CodeExport.ExportLuaCode();
        }

        [MenuItem("ZF/UI/打印路径", false)]
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
        [MenuItem("ZF/Build/Build Lua")]
        [MenuItem("Assets/ZF/Build/Build Lua")]
        public static void BuildLua() {
            var builder = new LuaBuilder();
            builder.Build();
        }

        [MenuItem("ZF/Build/Build Lua Res")]
        [MenuItem("Assets/ZF/Build/Build Lua Res")]
        public static void BuildLuaRes() {
            var builder = new LuaResourceBuilder();
            builder.Build();
        }

        [MenuItem("ZF/Build/Build UI")]
        [MenuItem("Assets/ZF/Build/Build UI")]
        public static void BuildUI() {
            var builder = new UIBuilder();
            foreach (var asset in Selection.objects) {
                GameObject go = asset as GameObject;
                if (go == null) continue;
                builder.Build(go);
            }
        }

        [MenuItem("ZF/Build/Build Audio", false)]
        [MenuItem("Assets/ZF/Build/Build Audio", false)]
        public static void BuildAudio() {
            var builder = new AudioBuilder();
            foreach (var asset in Selection.objects) {
                builder.Build(asset);
            }
        }

        [MenuItem("ZF/Build/Build Video", false)]
        [MenuItem("Assets/ZF/Build/Build Video", false)]
        public static void BuildVideo() {
            var builder = new VideoBuilder();
            foreach (var asset in Selection.objects) {
                builder.Build(asset);
            }
        }

        [MenuItem("ZF/Build/Build Texture", false)]
        [MenuItem("Assets/ZF/Build/Build Texture", false)]
        public static void BuildTexture() {
            var builder = new TextureBuilder();
            foreach (var asset in Selection.objects) {
                builder.Build(asset);   
            }
        }

        [MenuItem("ZF/Build/Build Scene", false)]
        [MenuItem("Assets/ZF/Build/Build Scene", false)]
        public static void BuildScene() {
            var builder = ScriptableObject.CreateInstance("SceneBuilder") as SceneBuilder;
            foreach (var asset in Selection.objects) {
                builder.Build(asset);   
            }
        }


        [MenuItem("ZF/Build/Build PreloadAssets", false)]
        [MenuItem("Assets/ZF/Build/Build PreloadAssets", false)]
        public static void BuildPreloadAssets()
        {
            var builder = new PreloadAssetsBuilder();
            foreach (var asset in Selection.objects) {
                builder.Build(asset);
            }
        }
        
        [MenuItem("ZF/Build/Build Scene Go", false)]
        [MenuItem("Assets/ZF/Build/Build Scene Go", false)]
        public static void BuildSceneGo() {
            var builder = ScriptableObject.CreateInstance("SceneGoBuilder") as SceneGoBuilder;
            foreach (var asset in Selection.objects) {
                builder.Build(asset);   
            }
        }

        [MenuItem("ZF/Build/Build Atlas", false)]
        [MenuItem("Assets/ZF/Build/Build Atlas", false)]
        public static void BuildAtlas()
        {
            var builder = ScriptableObject.CreateInstance("SpriteAtlasBuilder") as SpriteAtlasBuilder;
            foreach (var asset in Selection.objects) {
                builder.Build(asset);
            }
        }

        [MenuItem("ZF/Build/Build Protobuf", false)]
        [MenuItem("Assets/ZF/Build/Build Protobuf", false)]
        public static void BuildProtobuf()
        {
            var builder = new ProtobufBuilder();
            foreach (var asset in Selection.objects) {
                builder.Build(asset);
            }
        }
    }

    public class PlayerPrefs { 
        [MenuItem("ZF/清除缓存", false)]
        public static void Clear()
        {
            UnityEngine.PlayerPrefs.DeleteAll();
            System.IO.Directory.Delete(ZF.Core.Util.PathExt.MakeCachePath("/model"), true);
        }
    }
}
