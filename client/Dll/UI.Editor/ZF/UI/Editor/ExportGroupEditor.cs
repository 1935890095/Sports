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
			if (EditorTools.DrawHeader($"{index}.{(group).name}[g]"))
			{
				EditorTools.BeginContents();
				EditorGUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
				EditorGUILayout.LabelField("GameObject", (GUILayoutOption[])new GUILayoutOption[1] { GUILayout.Width(EditorGUIUtility.labelWidth) });
				EditorGUILayout.ObjectField(((Component)group).gameObject, typeof(GameObject), true, (GUILayoutOption[])new GUILayoutOption[0]);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
				EditorGUILayout.LabelField("Name：", (GUILayoutOption[])new GUILayoutOption[1] { GUILayout.Width(EditorGUIUtility.labelWidth) });
				GUI.color = (Color.green);
				EditorGUILayout.LabelField(group.Name, (GUILayoutOption[])new GUILayoutOption[0]);
				GUI.color = (Color.white);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
				EditorGUILayout.LabelField("Type：", (GUILayoutOption[])new GUILayoutOption[1] { GUILayout.Width(EditorGUIUtility.labelWidth) });
				GUI.color = (Color.green);
				EditorGUILayout.LabelField(group.type, (GUILayoutOption[])new GUILayoutOption[0]);
				GUI.color = (Color.white);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
				EditorGUILayout.LabelField("Desc：", (GUILayoutOption[])new GUILayoutOption[1] { GUILayout.Width(EditorGUIUtility.labelWidth) });
				GUI.color = (Color.green);
				EditorGUILayout.LabelField(group.desc, (GUILayoutOption[])new GUILayoutOption[0]);
				GUI.color = (Color.white);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
				EditorGUILayout.LabelField("Path：", (GUILayoutOption[])new GUILayoutOption[1] { GUILayout.Width(EditorGUIUtility.labelWidth) });
				GUI.color = (Color.green);
				EditorGUILayout.LabelField(group.path, (GUILayoutOption[])new GUILayoutOption[0]);
				GUI.color = (Color.white);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
				EditorGUILayout.LabelField("Children：", (GUILayoutOption[])new GUILayoutOption[1] { GUILayout.Width(EditorGUIUtility.labelWidth) });
				GUI.color = (Color.green);
				EditorGUILayout.LabelField((group.children != null) ? group.children.Length.ToString() : "0", (GUILayoutOption[])new GUILayoutOption[0]);
				GUI.color = (Color.white);
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
			if (EditorTools.DrawHeader($"{index}.{(node).name}"))
			{
				EditorTools.BeginContents();
				EditorGUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
				EditorGUILayout.LabelField("GameObject", (GUILayoutOption[])new GUILayoutOption[1] { GUILayout.Width(EditorGUIUtility.labelWidth) });
				EditorGUILayout.ObjectField(((Component)node).gameObject, typeof(GameObject), true, (GUILayoutOption[])new GUILayoutOption[0]);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
				EditorGUILayout.LabelField("Name：", (GUILayoutOption[])new GUILayoutOption[1] { GUILayout.Width(EditorGUIUtility.labelWidth) });
				GUI.color = (Color.green);
				EditorGUILayout.LabelField(node.Name, (GUILayoutOption[])new GUILayoutOption[0]);
				GUI.color  = (Color.white);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
				EditorGUILayout.LabelField("Type：", (GUILayoutOption[])new GUILayoutOption[1] { GUILayout.Width(EditorGUIUtility.labelWidth) });
				GUI.color = (Color.green);
				EditorGUILayout.LabelField(node.type, (GUILayoutOption[])new GUILayoutOption[0]);
				GUI.color = (Color.white);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
				EditorGUILayout.LabelField("Desc：", (GUILayoutOption[])new GUILayoutOption[1] { GUILayout.Width(EditorGUIUtility.labelWidth) });
				GUI.color = (Color.green);
				EditorGUILayout.LabelField(node.desc, (GUILayoutOption[])new GUILayoutOption[0]);
				GUI.color = (Color.white);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
				EditorGUILayout.LabelField("Path：", (GUILayoutOption[])new GUILayoutOption[1] { GUILayout.Width(EditorGUIUtility.labelWidth) });
				GUI.color = (Color.green);
				EditorGUILayout.LabelField(node.path, (GUILayoutOption[])new GUILayoutOption[0]);
				GUI.color = (Color.white);
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
				if (component != null)
				{
					component.path = GetPath(component, root);
					children.Add(component);
					ExportGroup exportGroup = component as ExportGroup;
					if (exportGroup != null)
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
			if (group == null)
			{
				Transform transform = ((Component)node).transform;
				if (transform.parent != null)
				{
					stringBuilder.Insert(0, "/" + (transform).name);
					transform = transform.parent;
					while (transform != null && !(((Component)transform).GetComponent<ExportGroup>() != null))
					{
						stringBuilder.Insert(0, "/" + (transform).name);
						if (++num == 100)
						{
							transform = null;
							break;
						}
						transform = transform.parent;
					}
					if (transform == null)
					{
						return null;
					}
					stringBuilder.Remove(0, 1);
				}
			}
			else
			{
				Transform val = ((Component)node).transform;
				while (val != null && !(val == ((Component)group).transform))
				{
					stringBuilder.Insert(0, "/" + (val).name);
					if (++num == 100)
					{
						val = null;
						break;
					}
					val = val.parent;
				}
				if (val == null)
				{
					return null;
				}
				stringBuilder.Remove(0, 1);
			}
			return stringBuilder.ToString();
		}
	}
}
