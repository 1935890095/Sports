using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace XFX.UI.Editor
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
			if (EditorTools.DrawHeader($"{index}.{((Object)group).name}[g]"))
			{
				EditorTools.BeginContents();
				EditorGUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
				EditorGUILayout.LabelField("GameObject", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(EditorGUIUtility.labelWidth) });
				EditorGUILayout.ObjectField((Object)(object)((Component)group).gameObject, typeof(GameObject), true, (GUILayoutOption[])(object)new GUILayoutOption[0]);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
				EditorGUILayout.LabelField("Name：", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(EditorGUIUtility.labelWidth) });
				GUI.color = Color.green;
				EditorGUILayout.LabelField(group.Name, (GUILayoutOption[])(object)new GUILayoutOption[0]);
				GUI.color = Color.white;
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
				EditorGUILayout.LabelField("Type：", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(EditorGUIUtility.labelWidth) });
				GUI.color = Color.green;
				EditorGUILayout.LabelField(group.type, (GUILayoutOption[])(object)new GUILayoutOption[0]);
				GUI.color = Color.white;
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
				EditorGUILayout.LabelField("Desc：", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(EditorGUIUtility.labelWidth) });
				GUI.color = Color.green;
				EditorGUILayout.LabelField(group.desc, (GUILayoutOption[])(object)new GUILayoutOption[0]);
				GUI.color = Color.white;
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
				EditorGUILayout.LabelField("Path：", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(EditorGUIUtility.labelWidth) });
				GUI.color = Color.green;
				EditorGUILayout.LabelField(group.path, (GUILayoutOption[])(object)new GUILayoutOption[0]);
				GUI.color = Color.white;
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
				EditorGUILayout.LabelField("Children：", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(EditorGUIUtility.labelWidth) });
				GUI.color = Color.green;
				EditorGUILayout.LabelField((group.children != null) ? group.children.Length.ToString() : "0", (GUILayoutOption[])(object)new GUILayoutOption[0]);
				GUI.color = Color.white;
				EditorGUILayout.EndHorizontal();
				EditorTools.EndContents();
			}
		}

		private void DrawNode(int index, ExportNode node)
		{
			if (EditorTools.DrawHeader($"{index}.{((Object)node).name}"))
			{
				EditorTools.BeginContents();
				EditorGUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
				EditorGUILayout.LabelField("GameObject", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(EditorGUIUtility.labelWidth) });
				EditorGUILayout.ObjectField((Object)(object)((Component)node).gameObject, typeof(GameObject), true, (GUILayoutOption[])(object)new GUILayoutOption[0]);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
				EditorGUILayout.LabelField("Name：", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(EditorGUIUtility.labelWidth) });
				GUI.color = Color.green;
				EditorGUILayout.LabelField(node.Name, (GUILayoutOption[])(object)new GUILayoutOption[0]);
				GUI.color = Color.white;
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
				EditorGUILayout.LabelField("Type：", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(EditorGUIUtility.labelWidth) });
				GUI.color = Color.green;
				EditorGUILayout.LabelField(node.type, (GUILayoutOption[])(object)new GUILayoutOption[0]);
				GUI.color = Color.white;
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
				EditorGUILayout.LabelField("Desc：", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(EditorGUIUtility.labelWidth) });
				GUI.color = Color.green;
				EditorGUILayout.LabelField(node.desc, (GUILayoutOption[])(object)new GUILayoutOption[0]);
				GUI.color = Color.white;
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
				EditorGUILayout.LabelField("Path：", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(EditorGUIUtility.labelWidth) });
				GUI.color = Color.green;
				EditorGUILayout.LabelField(node.path, (GUILayoutOption[])(object)new GUILayoutOption[0]);
				GUI.color = Color.white;
				EditorGUILayout.EndHorizontal();
				EditorTools.EndContents();
			}
		}

		private void BuildExportTree(ExportGroup root)
		{
			root.children = null;
			List<ExportNode> list = new List<ExportNode>();
			root.path = GetPath(root, null);
			BuildExportTree(((Component)root).transform, root, list);
			root.children = list.ToArray();
		}

		private void BuildExportTree(Transform parent, ExportGroup root, List<ExportNode> children)
		{
			for (int i = 0; i < parent.childCount; i++)
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
						BuildExportTree(((Component)exportGroup).transform, exportGroup, list);
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
				Transform transform = ((Component)node).transform;
				if ((Object)(object)transform.parent != (Object)null)
				{
					stringBuilder.Insert(0, "/" + ((Object)transform).name);
					transform = transform.parent;
					while ((Object)(object)transform != (Object)null && !((Object)(object)((Component)transform).GetComponent<ExportGroup>() != (Object)null))
					{
						stringBuilder.Insert(0, "/" + ((Object)transform).name);
						if (++num == 100)
						{
							transform = null;
							break;
						}
						transform = transform.parent;
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
				Transform val = ((Component)node).transform;
				while ((Object)(object)val != (Object)null && !((Object)(object)val == (Object)(object)((Component)group).transform))
				{
					stringBuilder.Insert(0, "/" + ((Object)val).name);
					if (++num == 100)
					{
						val = null;
						break;
					}
					val = val.parent;
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
