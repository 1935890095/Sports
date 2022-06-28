using UnityEngine;

namespace ZF.Asset.Properties
{
	public class ObjectProperty : DependsProperty, IAssetProperty
	{
		public Object asset;

		public bool Validate(IAssetValidator validator)
		{
			if (validator != null)
			{
				return validator.Validate(this);
			}
			if (!Object.op_Implicit(asset))
			{
				return false;
			}
			return true;
		}
	}
}
