namespace ZF.Asset.Attributes
{
	public class StringPopupAttribute : DrawerAttribute
	{
		public string[] displayOptions { get; private set; }

		public string[] optionValues { get; private set; }

		public StringPopupAttribute(string label, string[] displayOptions, string[] optionValues)
		{
			this.displayOptions = displayOptions;
			this.optionValues = optionValues;
		}
	}
}
