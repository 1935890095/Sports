using System;
using UnityEngine;

namespace XFX.Asset.Properties
{
	[CreateAssetMenu]
	public class PreloadProperty : ScriptableObject, IAssetProperty
	{
		public string[] shaders = Array.Empty<string>();

		public PreloadProperty()
		{
		}

		public bool Validate(IAssetValidator validator)
		{
			return validator?.Validate(this) ?? false;
		}
	}
}
