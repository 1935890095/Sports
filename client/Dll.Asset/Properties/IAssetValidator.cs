using UnityEngine;

namespace XFX.Asset.Properties
{
	public interface IAssetValidator
	{
		[ExecuteInEditMode]
		bool Validate(IAssetProperty property);
	}
}
