using System.ComponentModel;
using UnityEngine;

namespace ZF.UI
{
	public class ExportNode : MonoBehaviour
	{
		[Description("节点的名称")]
		public string Name;

		[Description("节点的类型")]
		public string type;

		[Description("节点的描述")]
		public string desc;

		[Description("节点的路径")]
		public string path;

		public ExportNode()
			: this()
		{
		}
	}
}
