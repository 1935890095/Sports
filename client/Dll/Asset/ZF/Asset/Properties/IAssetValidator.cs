using UnityEngine;

namespace ZF.Asset.Properties
{
	public interface IAssetValidator
	{
		[ExecuteInEditMode]
		bool Validate(IAssetProperty property);
	}
}
