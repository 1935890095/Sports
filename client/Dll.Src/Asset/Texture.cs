using UnityEngine;
using XFX.Core.Render;

namespace XFX.Asset
{
	public class Texture : RenderObject, ITexture, IRenderObject
	{
		public UnityEngine.Texture texture { get; private set; }

		protected override void OnCreate(IRenderResource resource)
		{
			Object asset = resource.asset;
			texture = (UnityEngine.Texture)(object)((asset is UnityEngine.Texture) ? asset : null);
		}
	}
}
