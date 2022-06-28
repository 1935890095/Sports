namespace ZF.Asset.Attributes
{
	public class IntPopupAttribute : DrawerAttribute
	{
		public string[] displayOptions { get; private set; }

		public int[] optionValues { get; private set; }

		public IntPopupAttribute(string label, string[] displayOptions, int[] optionValues)
			: base(label, label)
		{
			this.displayOptions = displayOptions;
			this.optionValues = optionValues;
		}
	}
}
