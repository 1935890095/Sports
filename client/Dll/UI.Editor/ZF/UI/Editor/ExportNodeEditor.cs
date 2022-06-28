using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ZF.UI.Editor
{
	[CustomEditor(typeof(ExportNode))]
	public class ExportNodeEditor : Editor
	{
		protected SerializedProperty prop_name;

		protected SerializedProperty prop_type;

		protected SerializedProperty prop_desc;

		protected SerializedProperty prop_path;

		protected ExportNode node;

		private int select_index;

		public ExportNodeEditor()
			: this()
		{
		}

		public virtual void Awake()
		{
			node = ((Editor)this).get_target() as ExportNode;
			if (string.IsNullOrEmpty(node.Name))
			{
				node.Name = ((Object)((Component)node).get_gameObject()).get_name();
			}
		}

		public virtual void OnEnable()
		{
			prop_name = ((Editor)this).get_serializedObject().FindProperty("Name");
			prop_type = ((Editor)this).get_serializedObject().FindProperty("type");
			prop_desc = ((Editor)this).get_serializedObject().FindProperty("desc");
			prop_path = ((Editor)this).get_serializedObject().FindProperty("path");
		}

		public override void OnInspectorGUI()
		{
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			((Editor)this).get_serializedObject().Update();
			List<string> list = (from c in ((Component)node).get_gameObject().GetComponents<Component>()
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
			EditorGUILayout.LabelField("Type", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(EditorGUIUtility.get_labelWidth()) });
			select_index = EditorGUILayout.Popup(select_index, list.ToArray(), (GUILayoutOption[])(object)new GUILayoutOption[0]);
			prop_type.set_stringValue(list[select_index]);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.PropertyField(prop_desc, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Height(EditorGUIUtility.get_singleLineHeight() * 3f) });
			EditorGUILayout.LabelField("Path", prop_path.get_stringValue(), (GUILayoutOption[])(object)new GUILayoutOption[0]);
			if (GUI.get_changed())
			{
				EditorUtility.SetDirty(((Editor)this).get_target());
			}
			((Editor)this).get_serializedObject().ApplyModifiedProperties();
		}
	}
}
