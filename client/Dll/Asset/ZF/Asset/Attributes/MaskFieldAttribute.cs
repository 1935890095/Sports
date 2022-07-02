namespace ZF.Asset.Attributes
{
	public class MaskFieldAttribute : DrawerAttribute
	{
		public string[] displayOptions { get; private set; }

		public MaskFieldAttribute(string label, string[] displayOptions)
		{
			this.displayOptions = displayOptions;
		}
	}
}
