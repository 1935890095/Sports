using UnityEngine;
using XFX.Core.Render;

namespace XFX.Asset
{
	public class Empty : RenderObject
	{
		public void Set(GameObject go)
		{
			base.gameObject = go;
		}
	}
}
