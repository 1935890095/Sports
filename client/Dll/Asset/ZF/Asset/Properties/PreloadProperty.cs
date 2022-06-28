using System;
using UnityEngine;

namespace ZF.Asset.Properties
{
	[CreateAssetMenu]
	public class PreloadProperty : ScriptableObject, IAssetProperty
	{
		public string[] shaders = Array.Empty<string>();

		public PreloadProperty()
			: this()
		{
		}

		public bool Validate(IAssetValidator validator)
		{
			return validator?.Validate(this) ?? false;
		}
	}
}
