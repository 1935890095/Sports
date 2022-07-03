using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace XFX.Asset.Properties
{
	[Serializable]
	public class Dependence
	{
		public string name = string.Empty;

		public string path = string.Empty;

		public Object dependence;

		public Object[] assets = Array.Empty<Object>();
	}
}
