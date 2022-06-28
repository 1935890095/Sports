using UnityEditor;
using UnityEngine;

namespace ZF.UI.Editor
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
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			bool flag = EditorPrefs.GetBool(key, true);
			if (!minimalistic)
			{
				GUILayout.Space(3f);
			}
			if (!forceOn && !flag)
			{
				GUI.set_backgroundColor(new Color(0.8f, 0.8f, 0.8f));
			}
			GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
			GUI.set_changed(false);
			if (minimalistic)
			{
				text = ((!flag) ? ("►" + '\u200a' + text) : ("▼" + '\u200a' + text));
				GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
				GUI.set_contentColor((!EditorGUIUtility.get_isProSkin()) ? new Color(0f, 0f, 0f, 0.7f) : new Color(1f, 1f, 1f, 0.7f));
				if (!GUILayout.Toggle(true, text, GUIStyle.op_Implicit("PreToolbar2"), (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.MinWidth(20f) }))
				{
					flag = !flag;
				}
				GUI.set_contentColor(Color.get_white());
				GUILayout.EndHorizontal();
			}
			else
			{
				text = "<b><size=11>" + text + "</size></b>";
				text = ((!flag) ? ("► " + text) : ("▼ " + text));
				if (!GUILayout.Toggle(true, text, GUIStyle.op_Implicit("dragtab"), (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.MinWidth(20f) }))
				{
					flag = !flag;
				}
			}
			if (GUI.get_changed())
			{
				EditorPrefs.SetBool(key, flag);
			}
			if (!minimalistic)
			{
				GUILayout.Space(2f);
			}
			GUILayout.EndHorizontal();
			GUI.set_backgroundColor(Color.get_white());
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
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			if (!minimalistic)
			{
				mEndHorizontal = true;
				GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
				EditorGUILayout.BeginHorizontal(GUIStyle.op_Implicit("TextArea"), (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.MinHeight(10f) });
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
