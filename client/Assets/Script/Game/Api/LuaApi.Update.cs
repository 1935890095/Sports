/********************************************************
    Id: LuaApi.Update.cs
    Desc: 定义给lua层使用的API——Update
    Author: figo
    Date: 2020-04-29 10:08:35
*********************************************************/

namespace ZF.Game {
    using System.Collections.Generic;
    using System.IO;
    using System;
    using UnityEngine;
    using XLua;
    using ZF.Misc;

    public partial class LuaApi {

        public static class LuaApiConfigUpdate {
            [CSharpCallLua]
            public static List<Type> CSharpCallLua = new List<Type>() {
                typeof(Action<bool>),
                typeof(Action<float>),
                typeof(Action<int, string>)
            };
        }

        [LuaCallCSharp]
        public static class Update {
            static Updater updater = null;

            public static void Begin(string url, string version, string upversion,
                System.Action<bool> onComplete, System.Action<float> onProgress, System.Action<int, string> onError) {
                if (string.IsNullOrWhiteSpace(url)) {
                    var asset = Resources.Load("url") as TextAsset;
                    url = asset.text.Trim('\n', '\r', ' ');
                    Resources.UnloadAsset(asset);
                }

                if (string.IsNullOrWhiteSpace(version)) {
                    var asset = Resources.Load("version") as TextAsset;
                    version = asset.text.Trim('\n', '\r', ' ');
                    Resources.UnloadAsset(asset);
                }

                string manifest = PathExt.MakeCachePath("version.manifest");
                if (File.Exists(manifest)) {
                    var mversion = File.ReadAllText(manifest).Trim('\n', '\r', ' ');
                    if (Misc.Version.Parse(mversion) < Misc.Version.Parse(version)) {
                        // clear cache
                        Directory.Delete(PathExt.MakeCachePath("res"), true);
                        File.Delete(manifest);
                    } else {
                        version = mversion;
                    }
                }

                Log.Info("* current version: {0}", version);

                updater = new Updater(version, url);
                updater.logger = ZF.Game.Log.ForkChild("update");
                if (onComplete != null) updater.onComplete = onComplete;
                if (onProgress != null) updater.onProgress = onProgress;
                if (onError != null) updater.onError = onError;

                if (!string.IsNullOrEmpty(upversion))
                    updater.Begin(upversion, url);
                else 
                    updater.Begin();
            }


            public static object Get(string what) {
                if (updater == null) return null;
                switch (what) {
                    case "totalSize":
                        return updater.totalSize;
                    case "downloadSize":
                        return updater.downloadSize;
                    case "downloadSpeed":
                        return updater.downloadSpeed;
                    case "progress":
                        return updater.progress;
                    case "complete":
                        return updater.complete;
                    default:
                        return null;
                }
            }

            public static void Continue() {
                if (updater != null) updater.Continue();
            }

            public static void End() {
                if (updater != null) {
                    updater.onComplete = null;
                    updater.onError = null;
                    updater.onProgress = null;
                    updater.End();
                    updater = null;
                }
            }

            public static bool Run() {
                if (updater != null) return updater.Update();
                else return false;
            }
        }
    }
}