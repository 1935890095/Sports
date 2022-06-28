namespace ZF.Asset.Properties
{
	public class ViewProperty : DependsProperty, IAssetProperty
	{
		public bool Validate(IAssetValidator validator)
		{
			if (validator != null)
			{
				return validator.Validate(this);
			}
			Collect(DependFlags.Texture | DependFlags.Font);
			return false;
		}
	}
}
