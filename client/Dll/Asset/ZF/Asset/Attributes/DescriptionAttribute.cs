namespace ZF.Asset.Attributes
{
	public class DescriptionAttribute : DrawerAttribute
	{
		public DescriptionAttribute(string label, string tooltip)
			: base(label, tooltip)
		{
		}

		public DescriptionAttribute(string label)
			: base(label, label)
		{
		}
	}
}
