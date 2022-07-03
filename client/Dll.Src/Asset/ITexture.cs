using UnityEngine;
using XFX.Core.Render;

namespace XFX.Asset
{
	public interface ITexture : IRenderObject
	{
		Texture texture { get; }
	}
}
