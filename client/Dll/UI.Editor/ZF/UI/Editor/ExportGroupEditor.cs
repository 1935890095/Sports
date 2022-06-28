using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace ZF.UI.Editor
{
	[CustomEditor(typeof(ExportGroup))]
	public class ExportGroupEditor : ExportNodeEditor
	{
		private ExportGroup group = null;

		[HideInInspector]
		public string GroupClassName;

		[HideInInspector]
		public string GroupName;

		[HideInInspector]
		public List<ExportNode> ExportComponents = new List<ExportNode>();

		[HideInInspector]
		public List<ExportGroup> ExportGroups = new List<ExportGroup>();

		public override void Awake()
		{
			base.Awake();
			group = node as ExportGroup;
		}

		public override void OnEnable()
		{
			base.OnEnable();
			BuildExportTree(group);
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			if (group.children == null)
			{
				return;
			}
			EditorGUILayout.Space();
			if (!EditorTools.DrawHeader($"children({group.children.Length})"))
			{
				return;
			}
			EditorGUILayout.Space();
			for (int i = 0; i < group.children.Length; i++)
			{
				ExportNode exportNode = group.children[i];
				if (exportNode is ExportGroup)
				{
					DrawGroup(i, exportNode as ExportGroup);
				}
				else
				{
					DrawNode(i, exportNode);
				}
			}
		}

		private void DrawGroup(int index, ExportGroup group)
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			if (EditorTools.DrawHeader($"{index}.{((Object)group).get_name()}[g]"))
			{
				EditorTools.BeginContents();
				EditorGUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
				EditorGUILayout.LabelField("GameObject", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(EditorGUIUtility.get_labelWidth()) });
				EditorGUILayout.ObjectField((Object)(object)((Component)group).get_gameObject(), typeof(GameObject), true, (GUILayoutOption[])(object)new GUILayoutOption[0]);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
				EditorGUILayout.LabelField("Name：", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(EditorGUIUtility.get_labelWidth()) });
				GUI.set_color(Color.get_green());
				EditorGUILayout.LabelField(group.Name, (GUILayoutOption[])(object)new GUILayoutOption[0]);
				GUI.set_color(Color.get_white());
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
				EditorGUILayout.LabelField("Type：", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(EditorGUIUtility.get_labelWidth()) });
				GUI.set_color(Color.get_green());
				EditorGUILayout.LabelField(group.type, (GUILayoutOption[])(object)new GUILayoutOption[0]);
				GUI.set_color(Color.get_white());
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
				EditorGUILayout.LabelField("Desc：", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(EditorGUIUtility.get_labelWidth()) });
				GUI.set_color(Color.get_green());
				EditorGUILayout.LabelField(group.desc, (GUILayoutOption[])(object)new GUILayoutOption[0]);
				GUI.set_color(Color.get_white());
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
				EditorGUILayout.LabelField("Path：", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(EditorGUIUtility.get_labelWidth()) });
				GUI.set_color(Color.get_green());
				EditorGUILayout.LabelField(group.path, (GUILayoutOption[])(object)new GUILayoutOption[0]);
				GUI.set_color(Color.get_white());
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
				EditorGUILayout.LabelField("Children：", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(EditorGUIUtility.get_labelWidth()) });
				GUI.set_color(Color.get_green());
				EditorGUILayout.LabelField((group.children != null) ? group.children.Length.ToString() : "0", (GUILayoutOption[])(object)new GUILayoutOption[0]);
				GUI.set_color(Color.get_white());
				EditorGUILayout.EndHorizontal();
				EditorTools.EndContents();
			}
		}

		private void DrawNode(int index, ExportNode node)
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			if (EditorTools.DrawHeader($"{index}.{((Object)node).get_name()}"))
			{
				EditorTools.BeginContents();
				EditorGUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
				EditorGUILayout.LabelField("GameObject", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(EditorGUIUtility.get_labelWidth()) });
				EditorGUILayout.ObjectField((Object)(object)((Component)node).get_gameObject(), typeof(GameObject), true, (GUILayoutOption[])(object)new GUILayoutOption[0]);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
				EditorGUILayout.LabelField("Name：", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(EditorGUIUtility.get_labelWidth()) });
				GUI.set_color(Color.get_green());
				EditorGUILayout.LabelField(node.Name, (GUILayoutOption[])(object)new GUILayoutOption[0]);
				GUI.set_color(Color.get_white());
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
				EditorGUILayout.LabelField("Type：", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(EditorGUIUtility.get_labelWidth()) });
				GUI.set_color(Color.get_green());
				EditorGUILayout.LabelField(node.type, (GUILayoutOption[])(object)new GUILayoutOption[0]);
				GUI.set_color(Color.get_white());
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
				EditorGUILayout.LabelField("Desc：", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(EditorGUIUtility.get_labelWidth()) });
				GUI.set_color(Color.get_green());
				EditorGUILayout.LabelField(node.desc, (GUILayoutOption[])(object)new GUILayoutOption[0]);
				GUI.set_color(Color.get_white());
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
				EditorGUILayout.LabelField("Path：", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(EditorGUIUtility.get_labelWidth()) });
				GUI.set_color(Color.get_green());
				EditorGUILayout.LabelField(node.path, (GUILayoutOption[])(object)new GUILayoutOption[0]);
				GUI.set_color(Color.get_white());
				EditorGUILayout.EndHorizontal();
				EditorTools.EndContents();
			}
		}

		private void BuildExportTree(ExportGroup root)
		{
			root.children = null;
			List<ExportNode> list = new List<ExportNode>();
			root.path = GetPath(root, null);
			BuildExportTree(((Component)root).get_transform(), root, list);
			root.children = list.ToArray();
		}

		private void BuildExportTree(Transform parent, ExportGroup root, List<ExportNode> children)
		{
			for (int i = 0; i < parent.get_childCount(); i++)
			{
				Transform child = parent.GetChild(i);
				ExportNode component = ((Component)child).GetComponent<ExportNode>();
				if ((Object)(object)component != (Object)null)
				{
					component.path = GetPath(component, root);
					children.Add(component);
					ExportGroup exportGroup = component as ExportGroup;
					if ((Object)(object)exportGroup != (Object)null)
					{
						exportGroup.children = null;
						List<ExportNode> list = new List<ExportNode>();
						BuildExportTree(((Component)exportGroup).get_transform(), exportGroup, list);
						exportGroup.children = list.ToArray();
						continue;
					}
				}
				BuildExportTree(child, root, children);
			}
		}

		private string GetPath(ExportNode node, ExportGroup group)
		{
			int num = 0;
			StringBuilder stringBuilder = new StringBuilder();
			if ((Object)(object)group == (Object)null)
			{
				Transform transform = ((Component)node).get_transform();
				if ((Object)(object)transform.get_parent() != (Object)null)
				{
					stringBuilder.Insert(0, "/" + ((Object)transform).get_name());
					transform = transform.get_parent();
					while ((Object)(object)transform != (Object)null && !((Object)(object)((Component)transform).GetComponent<ExportGroup>() != (Object)null))
					{
						stringBuilder.Insert(0, "/" + ((Object)transform).get_name());
						if (++num == 100)
						{
							transform = null;
							break;
						}
						transform = transform.get_parent();
					}
					if ((Object)(object)transform == (Object)null)
					{
						return null;
					}
					stringBuilder.Remove(0, 1);
				}
			}
			else
			{
				Transform val = ((Component)node).get_transform();
				while ((Object)(object)val != (Object)null && !((Object)(object)val == (Object)(object)((Component)group).get_transform()))
				{
					stringBuilder.Insert(0, "/" + ((Object)val).get_name());
					if (++num == 100)
					{
						val = null;
						break;
					}
					val = val.get_parent();
				}
				if ((Object)(object)val == (Object)null)
				{
					return null;
				}
				stringBuilder.Remove(0, 1);
			}
			return stringBuilder.ToString();
		}
	}
}
