using UnityEngine;
using XFX.Core.Render;

namespace XFX.Asset
{
	public interface ITexture : IRenderObject
	{
		UnityEngine.Texture texture { get; }
	}
}
