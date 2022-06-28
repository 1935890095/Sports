using UnityEngine;
using ZF.Core.Render;

namespace ZF.Asset
{
	public class Texture : RenderObject, ITexture, IRenderObject
	{
		public Texture texture { get; private set; }

		protected override void OnCreate(IRenderResource resource)
		{
			Object asset = resource.asset;
			texture = (Texture)(object)((asset is Texture) ? asset : null);
		}
	}
}
