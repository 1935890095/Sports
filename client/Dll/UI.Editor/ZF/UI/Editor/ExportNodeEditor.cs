using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ZF.UI.Editor
{
	[CustomEditor(typeof(ExportNode))]
	public class ExportNodeEditor : UnityEditor.Editor
	{
		protected SerializedProperty prop_name;

		protected SerializedProperty prop_type;

		protected SerializedProperty prop_desc;

		protected SerializedProperty prop_path;

		protected ExportNode node;

		private int select_index;

		public ExportNodeEditor()
		{
		}

		public virtual void Awake()
		{
			node = (this).target as ExportNode;
			if (string.IsNullOrEmpty(node.Name))
			{
				node.Name = ((Object)((Component)node).gameObject).name;
			}
		}

		public virtual void OnEnable()
		{
			prop_name = (this).serializedObject.FindProperty("Name");
			prop_type = (this).serializedObject.FindProperty("type");
			prop_desc = (this).serializedObject.FindProperty("desc");
			prop_path = (this).serializedObject.FindProperty("path");
		}

		public override void OnInspectorGUI()
		{
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			(this).serializedObject.Update();
			List<string> list = (from c in ((Component)node).gameObject.GetComponents<Component>()
				where (Object)(object)c != (Object)null && ((object)c).GetType() != typeof(ExportNode)
				select c into x
				select ((object)x).GetType().Name).Distinct().ToList();
			list.Insert(0, "GameObject");
			list.Remove("CanvasRenderer");
			int num = list.IndexOf(node.type);
			if (num >= 0)
			{
				select_index = num;
			}
			else
			{
				select_index = ((list.Count > 2) ? 2 : 0);
			}
			EditorGUILayout.PropertyField(prop_name, (GUILayoutOption[])(object)new GUILayoutOption[0]);
			EditorGUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
			EditorGUILayout.LabelField("Type", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(EditorGUIUtility.labelWidth) });
			select_index = EditorGUILayout.Popup(select_index, list.ToArray(), (GUILayoutOption[])(object)new GUILayoutOption[0]);
			prop_type.stringValue = (list[select_index]);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.PropertyField(prop_desc, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Height(EditorGUIUtility.singleLineHeight * 3f) });
			EditorGUILayout.LabelField("Path", prop_path.stringValue, (GUILayoutOption[])(object)new GUILayoutOption[0]);
			if (GUI.changed)
			{
				EditorUtility.SetDirty((this).target);
			}
			(this).serializedObject.ApplyModifiedProperties();
		}
	}
}
