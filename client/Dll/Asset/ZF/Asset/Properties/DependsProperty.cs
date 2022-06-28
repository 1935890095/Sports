using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ZF.Asset.Properties
{
	public class DependsProperty : MonoBehaviour
	{
		public Dependence[] dependencies = Array.Empty<Dependence>();

		public DependsProperty()
			: this()
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
					Material[] sharedMaterials = val.get_sharedMaterials();
					foreach (Material val2 in sharedMaterials)
					{
						if (Object.op_Implicit((Object)(object)val2))
						{
							if ((flags & DependFlags.Shader) != 0 && Object.op_Implicit((Object)(object)val2.get_shader()))
							{
								if (!dict.ContainsKey((Object)(object)val2.get_shader()))
								{
									dict.Add((Object)(object)val2.get_shader(), new HashSet<Object>());
								}
								dict[(Object)(object)val2.get_shader()].Add((Object)(object)val2);
							}
							if ((flags & DependFlags.Texture) != 0 && Object.op_Implicit((Object)(object)val2.get_mainTexture()))
							{
								if (!dict.ContainsKey((Object)(object)val2.get_mainTexture()))
								{
									dict.Add((Object)(object)val2.get_mainTexture(), new HashSet<Object>());
								}
								dict[(Object)(object)val2.get_mainTexture()].Add((Object)(object)val2);
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
						if (Object.op_Implicit((Object)(object)val3.get_texture()))
						{
							if (!dict.ContainsKey((Object)(object)val3.get_texture()))
							{
								dict.Add((Object)(object)val3.get_texture(), new HashSet<Object>());
							}
							dict[(Object)(object)val3.get_texture()].Add((Object)(object)val3);
						}
					}
				}
				if ((flags & DependFlags.Font) != 0)
				{
					Text[] componentsInChildren3 = root.GetComponentsInChildren<Text>(true);
					Text[] array3 = componentsInChildren3;
					foreach (Text val4 in array3)
					{
						if (Object.op_Implicit((Object)(object)val4.get_font()))
						{
							if (!dict.ContainsKey((Object)(object)val4.get_font()))
							{
								dict.Add((Object)(object)val4.get_font(), new HashSet<Object>());
							}
							dict[(Object)(object)val4.get_font()].Add((Object)(object)val4);
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
				dependence.name = item2.Key.get_name();
				dependence.dependence = item2.Key;
				dependence.assets = array;
				Dependence item = dependence;
				list.Add(item);
			}
			dependencies = list.ToArray();
		}

		public void Collect(DependFlags flags = DependFlags.Default)
		{
			Collect((GameObject[])(object)new GameObject[1] { ((Component)this).get_gameObject() }, flags);
		}
	}
}
