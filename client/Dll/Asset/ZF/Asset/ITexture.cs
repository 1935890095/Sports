using UnityEngine;
using ZF.Core.Render;

namespace ZF.Asset
{
	public interface ITexture : IRenderObject
	{
		Texture texture { get; }
	}
}
