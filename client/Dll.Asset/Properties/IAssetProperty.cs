using UnityEngine;

namespace XFX.Asset.Properties
{
	public interface IAssetProperty
	{
		[ExecuteInEditMode]
		bool Validate(IAssetValidator validater = null);
	}
}
