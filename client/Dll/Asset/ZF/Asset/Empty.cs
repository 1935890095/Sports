using UnityEngine;
using ZF.Core.Render;

namespace ZF.Asset
{
	public class Empty : RenderObject
	{
		public void Set(GameObject go)
		{
			base.gameObject = go;
		}
	}
}
