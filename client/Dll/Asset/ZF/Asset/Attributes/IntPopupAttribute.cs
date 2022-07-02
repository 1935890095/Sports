namespace ZF.Asset.Attributes
{
	public class IntPopupAttribute : DrawerAttribute
	{
		public string[] displayOptions { get; private set; }

		public int[] optionValues { get; private set; }

		public IntPopupAttribute(string label, string[] displayOptions, int[] optionValues)
		{
			this.displayOptions = displayOptions;
			this.optionValues = optionValues;
		}
	}
}
