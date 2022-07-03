using System.ComponentModel;

namespace XFX.UI
{
	public class ExportGroup : ExportNode
	{
		[Description("子节点")]
		public ExportNode[] children;
	}
}
