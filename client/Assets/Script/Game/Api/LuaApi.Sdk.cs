/********************************************************
    Id: LuaApi.Sdk.cs
    Desc: 定义给lua层使用的API——Sdk
    Author: figo
    Date: 2020-04-28 15:33:06
*********************************************************/
using System;
using UnityEngine;
using XLua;

namespace ZF.Game {

    public partial class LuaApi {

        [LuaCallCSharp]
        public static class Sdk {
            static CallBack callback;
            [BlackList]
            public static void Setup() {
                if (callback == null) {
                    callback = CallBack.Create();
                }
            }

            public static string Invoke (string name, string method, string args) {
                string result = "";
                switch(Application.platform) {
                case RuntimePlatform.Android: 
                    result = InvokeAndroid(name, method, args);
                    break;
                case RuntimePlatform.IPhonePlayer: 
                    result = InvokeiOS(name, method, args);
                    break;
                default: 
                    result = InvokeWindows(name, method, args);
                    break;
                }

                // default value
                if (string.IsNullOrEmpty(result)) {
                    result = null;
                    switch(method)  {
                        /*
                        case "getDeviceId": result = SystemInfo.deviceUniqueIdentifier; break;
                        case "getDeviceName": {
                            result = SystemInfo.deviceName; 
                            if (string.IsNullOrWhiteSpace(result) || result.Equals("<unknown>"))
                                result = System.Environment.MachineName;
                        }
                        */
                        case "getDeviceId": result = ""; break;
                        case "getDeviceName": result = System.Environment.MachineName;
                        break;
                        case "getOs":
                            result = SystemInfo.operatingSystem;
                        break;
                        default: break;
                    }
                }
                return result;
            }

            public static void Reported(string method, string eventName, string args) {
                Log.Debug("[Android Plugins Sdk] call method: " + method + ":" + eventName + ":" + args);
                switch (Application.platform) {
                    case RuntimePlatform.Android:
                        Log.Debug("[Android Plugins Sdk] call method: " + method + ":" +eventName + ":" + args);
                        // plugins.Call(method, eventName, args);
                        Invoke("flurry", eventName, args);
                        break;
                } 
            }

            #region [android]
            static string InvokeAndroid (string name, string method,  string args) {
                Log.Debug("[AndroidSdk] call method: " + method);
                if (android != null) {
                    return android.CallStatic<string>("invoke", name, method, args);
                } else {
                    return null;
                }
            }

            static AndroidJavaObject android_;
            static AndroidJavaObject android {
                get {
                    if (android_ == null) {
                        try {
                            android_= new AndroidJavaClass("com.zfc.sdk.SdkManager");
                        } catch (Exception e) {
                            Log.Error("[AndroidSdk] get sdk error: {0}", e.Message);
                        }
                    }
                    return android_;
                }
            }
            #endregion

            #region [iOS]
            static string InvokeiOS(string name, string method, string args) {
                return "";
            }
            #endregion

            #region [Windows]
            static string InvokeWindows(string name, string method, string args) {
                Log.Debug("invoke windows {0}", method);
                switch (method) {
                    case "login":
                        callback.LateCall("login", "");
                        break;
                    case "logout":
                        callback.LateCall("logout", "");
                        break;
                }
                return "";
            }
            #endregion

            class CallBack : MonoBehaviour {
                public static CallBack Create() {
                    GameObject go = new GameObject("SdkCallBack");
                    var c = go.AddComponent<CallBack>();
                    UnityEngine.Object.DontDestroyOnLoad(go);
                    return c;
                }

                void call(string json) {
                    string[] ss = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(json);
                    context.invoke("sdk", ss[0], ss[1]);
                }

                // 该接口用于非sdk环境 
                public void LateCall(string method, string args) {
                    StartCoroutine(CallCoroutine(method, args));
                }

                private System.Collections.IEnumerator CallCoroutine(string method, string args) {
                    yield return null;
                    args = Newtonsoft.Json.JsonConvert.SerializeObject(new string[] { method, args });
                    Log.Debug(args);
                    SendMessage("call", args);
                }
            }
        }
    }
}
