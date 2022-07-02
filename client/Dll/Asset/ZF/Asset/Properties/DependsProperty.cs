using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Text = UnityEngine.UI.Text;

namespace ZF.Asset.Properties
{
	public class DependsProperty : MonoBehaviour
	{
		public Dependence[] dependencies = Array.Empty<Dependence>();

		public DependsProperty()
		{
		}

		public void Collect(GameObject[] roots, DependFlags flags)
		{
			Dictionary<Object, HashSet<Object>> dict = new Dictionary<Object, HashSet<Object>>();
			Array.ForEach(roots, delegate(GameObject root)
			{
				Renderer[] componentsInChildren = root.GetComponentsInChildren<Renderer>(true);
				foreach (Renderer val in componentsInChildren)
				{
					Material[] sharedMaterials = val.sharedMaterials;
					foreach (Material val2 in sharedMaterials)
					{
						if (val2)
						{
							if ((flags & DependFlags.Shader) != 0 && val2.shader)
							{
								if (!dict.ContainsKey(val2.shader))
								{
									dict.Add(val2.shader, new HashSet<Object>());
								}
								dict[val2.shader].Add(val2);
							}
							if ((flags & DependFlags.Texture) != 0 && (Object)(val2.mainTexture))
							{
								if (!dict.ContainsKey(val2.mainTexture))
								{
									dict.Add(val2.mainTexture, new HashSet<Object>());
								}
								dict[val2.mainTexture].Add(val2);
							}
						}
					}
				}
				if ((flags & DependFlags.Texture) != 0)
				{
					RawImage[] componentsInChildren2 = root.GetComponentsInChildren<RawImage>(true);
					RawImage[] array2 = componentsInChildren2;
					foreach (RawImage val3 in array2)
					{
						if ((Object)(val3.texture))
						{
							if (!dict.ContainsKey(val3.texture))
							{
								dict.Add(val3.texture, new HashSet<Object>());
							}
							dict[val3.texture].Add(val3);
						}
					}
				}
				if ((flags & DependFlags.Font) != 0)
				{
					Text[] componentsInChildren3 = root.GetComponentsInChildren<Text>(true);
					Text[] array3 = componentsInChildren3;
					foreach (Text val4 in array3)
					{
						if ((Object)(val4.font))
						{
							if (!dict.ContainsKey(val4.font))
							{
								dict.Add(val4.font, new HashSet<Object>());
							}
							dict[val4.font].Add(val4);
						}
					}
				}
			});
			if (dict.Count <= 0)
			{
				return;
			}
			List<Dependence> list = new List<Dependence>();
			foreach (KeyValuePair<Object, HashSet<Object>> item2 in dict)
			{
				Object[] array = (Object[])(object)new Object[item2.Value.Count];
				item2.Value.CopyTo(array);
				Dependence dependence = new Dependence();
				dependence.name = item2.Key.name;
				dependence.dependence = item2.Key;
				dependence.assets = array;
				Dependence item = dependence;
				list.Add(item);
			}
			dependencies = list.ToArray();
		}

		public void Collect(DependFlags flags = DependFlags.Default)
		{
			Collect((GameObject[])(object)new GameObject[1] { ((Component)this).gameObject }, flags);
		}
	}
}
