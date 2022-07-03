using UnityEditor;
using UnityEngine;

namespace XFX.UI.Editor
{
	internal class EditorTools
	{
		public class Settings
		{
			public static bool minimalisticLook
			{
				get
				{
					return GetBool("Minimalistic", defaultValue: false);
				}
				set
				{
					SetBool("Minimalistic", value);
				}
			}

			public static bool GetBool(string name, bool defaultValue)
			{
				return EditorPrefs.GetBool(name, defaultValue);
			}

			public static int GetInt(string name, int defaultValue)
			{
				return EditorPrefs.GetInt(name, defaultValue);
			}

			public static float GetFloat(string name, float defaultValue)
			{
				return EditorPrefs.GetFloat(name, defaultValue);
			}

			public static string GetString(string name, string defaultValue)
			{
				return EditorPrefs.GetString(name, defaultValue);
			}

			public static void SetBool(string name, bool val)
			{
				EditorPrefs.SetBool(name, val);
			}

			public static void SetInt(string name, int val)
			{
				EditorPrefs.SetInt(name, val);
			}

			public static void SetFloat(string name, float val)
			{
				EditorPrefs.SetFloat(name, val);
			}

			public static void SetString(string name, string val)
			{
				EditorPrefs.SetString(name, val);
			}
		}

		private static bool mEndHorizontal = false;

		public static bool DrawHeader(string text)
		{
			return DrawHeader(text, text, forceOn: false, Settings.minimalisticLook);
		}

		public static bool DrawHeader(string text, string key)
		{
			return DrawHeader(text, key, forceOn: false, Settings.minimalisticLook);
		}

		public static bool DrawHeader(string text, bool detailed)
		{
			return DrawHeader(text, text, detailed, !detailed);
		}

		public static bool DrawHeader(string text, string key, bool forceOn, bool minimalistic)
		{
			bool flag = EditorPrefs.GetBool(key, true);
			if (!minimalistic)
			{
				GUILayout.Space(3f);
			}
			if (!forceOn && !flag)
			{
				GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
			}
			GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
			GUI.changed = false;
			if (minimalistic)
			{
				text = ((!flag) ? ("►" + '\u200a' + text) : ("▼" + '\u200a' + text));
				GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
				GUI.contentColor = (!EditorGUIUtility.isProSkin ? new Color(0f, 0f, 0f, 0.7f) : new Color(1f, 1f, 1f, 0.7f));
				if (!GUILayout.Toggle(true, text, new GUIStyle("PreToolbar2"), (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.MinWidth(20f) }))
				{
					flag = !flag;
				}
				GUI.contentColor = Color.white;
				GUILayout.EndHorizontal();
			}
			else
			{
				text = "<b><size=11>" + text + "</size></b>";
				text = ((!flag) ? ("► " + text) : ("▼ " + text));
				if (!GUILayout.Toggle(true, text, new GUIStyle("dragtab"), (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.MinWidth(20f) }))
				{
					flag = !flag;
				}
			}
			if (GUI.changed)
			{
				EditorPrefs.SetBool(key, flag);
			}
			if (!minimalistic)
			{
				GUILayout.Space(2f);
			}
			GUILayout.EndHorizontal();
			GUI.backgroundColor = Color.white;
			if (!forceOn && !flag)
			{
				GUILayout.Space(3f);
			}
			return flag;
		}

		public static void BeginContents()
		{
			BeginContents(Settings.minimalisticLook);
		}

		public static void BeginContents(bool minimalistic)
		{
			if (!minimalistic)
			{
				mEndHorizontal = true;
				GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
				EditorGUILayout.BeginHorizontal(new GUIStyle("TextArea"), (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.MinHeight(10f) });
			}
			else
			{
				mEndHorizontal = false;
				EditorGUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.MinHeight(10f) });
				GUILayout.Space(10f);
			}
			GUILayout.BeginVertical((GUILayoutOption[])(object)new GUILayoutOption[0]);
			GUILayout.Space(2f);
		}

		public static void EndContents()
		{
			GUILayout.Space(3f);
			GUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();
			if (mEndHorizontal)
			{
				GUILayout.Space(3f);
				GUILayout.EndHorizontal();
			}
			GUILayout.Space(3f);
		}
	}
}
