using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using CSObjectWrapEditor;
using UnityEditor;
using UnityEngine;
using XLua;

namespace Assets.XLua.Src.Editor.LuaCofig {
    public class LuaConfigGen : ScriptableObject {
        public TextAsset Template;
        static string FILE_NAME;
        static string JSON;

        public static void GenLuaTableFile (string filename, string json) {
            FILE_NAME = filename;
            JSON = json;
            var d = ScriptableObject.CreateInstance<LuaConfigGen> ();
            Generator.CustomGen (d.Template.text, GetTasks);
        }

        public static IEnumerable<CustomGenTask> GetTasks (LuaEnv lua_env, UserConfig user_cfg) {
            DirectoryInfo dirinfo = new DirectoryInfo (Application.streamingAssetsPath);
            string dir = dirinfo.FullName + "/res/resource/config/";
            if (!Directory.Exists (dir)) {
                Directory.CreateDirectory (dir);
            }

            LuaTable data = lua_env.NewTable ();
            data.Set ("dbname", FILE_NAME);
            data.Set ("jsonstr", JSON);
            yield return new CustomGenTask () {
                Data = data,
                Output = new StreamWriter (string.Concat (dir, FILE_NAME, ".lua"), false, new UTF8Encoding(false))
            };
            FILE_NAME = string.Empty;
            JSON = string.Empty;
            AssetDatabase.Refresh ();
        }
    }
}