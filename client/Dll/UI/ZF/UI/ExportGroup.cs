using System.ComponentModel;

namespace ZF.UI
{
	public class ExportGroup : ExportNode
	{
		[Description("子节点")]
		public ExportNode[] children;
	}
}
