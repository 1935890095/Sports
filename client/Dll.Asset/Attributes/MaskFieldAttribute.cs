namespace XFX.Asset.Attributes
{
	public class MaskFieldAttribute : DrawerAttribute
	{
		public string[] displayOptions { get; private set; }

		public MaskFieldAttribute(string label, string[] displayOptions)
			: base(label, label)
		{
			this.displayOptions = displayOptions;
		}
	}
}
