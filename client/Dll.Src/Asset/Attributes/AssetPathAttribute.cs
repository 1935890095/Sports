using System;

namespace XFX.Asset.Attributes
{
	public class AssetPathAttribute : DrawerAttribute
	{
		public string extension { get; private set; }

		public Type type { get; private set; }

		public AssetPathAttribute(string label, string tooltip, Type type)
			: base(label, tooltip)
		{
			this.type = type;
		}

		public AssetPathAttribute(string label, string tooltip, string extension)
			: base(label, tooltip)
		{
			this.extension = extension;
		}
	}
}
