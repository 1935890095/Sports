using System;

namespace ZF.Asset.Attributes
{
	public class AssetPathAttribute : DrawerAttribute
	{
		public string extension { get; private set; }
		public string tooltip { get; private set; }

		public Type type { get; private set; }
		public string label{ get; private set; }

		public AssetPathAttribute(string label, string tooltip, Type type)
		{
			this.type = type;
			this.label = label;
			this.tooltip = tooltip;
		}

		public AssetPathAttribute(string label, string tooltip, string extension)
		{
			this.extension = extension;
			this.label = label;
			this.tooltip = tooltip;
		}
	}
}
