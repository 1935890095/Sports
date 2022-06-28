using UnityEngine;

namespace ZF.Asset.Properties
{
	public interface IAssetProperty
	{
		[ExecuteInEditMode]
		bool Validate(IAssetValidator validater = null);
	}
}
