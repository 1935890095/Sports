using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using XLua;

namespace XFX.Game {
    public partial class LuaApi {
        public static class LuaApiConfigHttp {
            [CSharpCallLua]
            public static List<Type> CSharpCallLua = new List<Type>() {
                typeof(Action<byte[]>),
                typeof(Action<string>)
            };
        }

        public static class HttpMethods {
            public const string Get = "GET";
            public const string Post = "POST";
        }

        [LuaCallCSharp]
        public static class Http {
            static Http() {
                if (Application.isEditor) {
                    ServicePointManager.ServerCertificateValidationCallback += (sender, x509, chain, error) => true;
                }
            }

            public static void SetHeader(string name, string value) {
                game.http.SetHeader(name, value);
            }

            public static void SetTimeout(int timeout) {
                game.http.SetTimeout(timeout);
            }

            public static void SetHold(bool hold) {
                game.http.SetHold(hold);
            }

            public static void Get(string path, Action<byte[]> responseHandler, Action<string> errorHandler, bool now) {
                game.http.Send(path, HttpMethods.Get, null, responseHandler, errorHandler, now);
            }

            public static void Post(string path, byte[] body, Action<byte[]> responseHandler, Action<string> errorHandler, bool now) {
                game.http.Send(path, HttpMethods.Post, body, responseHandler, errorHandler, now);
            }

            public static void GetTextureByUrl(string url, bool isReadCache, bool isSaveCache ,string cacheName, int width, int height, Action<Texture2D> callback, Action<string> errorCallback = null) {
                string cachePath = PathExt.MakeCachePath("cache/tex");
                if ((isReadCache || isSaveCache) && !Directory.Exists(cachePath)) {
                    Directory.CreateDirectory(cachePath);
                }
                string dataPath = PathExt.MakeCachePath("cache/tex/" + cacheName);
                if (isReadCache && File.Exists(dataPath)) {
                    byte[] bytes = File.ReadAllBytes(dataPath);
                    Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
                    texture.LoadImage(bytes, false);
                    callback(texture);
                } else {
                    Get(url, (bytes) => {
                        Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
                        texture.LoadImage(bytes, false);
                        callback(texture);
                        if (isSaveCache) {
                            File.WriteAllBytes(dataPath, bytes);
                        }
                    }, (error) => {
                        if (errorCallback != null)
                            errorCallback(error);

                        Log.Warn("get url {0} error", url);
                    }, false);
                }
            }
        }
    }
}