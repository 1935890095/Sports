using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using ZF.Asset.Properties;
using ZF.Core.Render;

namespace ZF.Asset
{
	public abstract class Depends : RenderObject
	{
		private class Depend : RenderObject
		{
			private Object asset;

			private Object[] assets;

			protected override void OnCreate(IRenderResource resource)
			{
				asset = resource.asset;
				if (assets != null)
				{
					DoAttach(assets);
					assets = null;
				}
			}

			public void Attach(Object[] assets)
			{
				if (assets != null)
				{
					if (!base.complete)
					{
						this.assets = assets;
					}
					else
					{
						DoAttach(assets);
					}
				}
			}

			private void DoAttach(Object[] assets)
			{
				if (asset is Texture)
				{
					Object obj = asset;
					Texture val = (Texture)(object)((obj is Texture) ? obj : null);
					for (int i = 0; i < assets.Length; i++)
					{
						Object obj2 = assets[i];
						Material val2 = (Material)(object)((obj2 is Material) ? obj2 : null);
						if (val2)
						{
							val2.mainTexture = (val);
						}
						Object obj3 = assets[i];
						RawImage val3 = (RawImage)(object)((obj3 is RawImage) ? obj3 : null);
						if (val3)
						{
							((Behaviour)val3).enabled = (false);
							val3.texture = (val);
							((Behaviour)val3).enabled = (true);
						}
					}
				}
				if (asset is Shader)
				{
					Object obj4 = asset;
					Shader shader = (Shader)(object)((obj4 is Shader) ? obj4 : null);
					foreach (Object obj5 in assets)
					{
						Material val4 = (Material)(object)((obj5 is Material) ? obj5 : null);
						if (val4)
						{
							val4.shader = (shader);
						}
					}
				}
				if (!(asset is Font))
				{
					return;
				}
				Object obj6 = asset;
				Font font = (Font)(object)((obj6 is Font) ? obj6 : null);
				foreach (Object obj7 in assets)
				{
					Text val5 = (Text)(object)((obj7 is Text) ? obj7 : null);
					if (val5 != null)
					{
						val5.font = font;
					}
				}
			}
		}

		protected override IEnumerator OnCreateAs(IRenderResource resource)
		{
			if (!(resource.asset is GameObject))
			{
				Destroy();
				Debug.LogError((object)("create depends error, resource is not GameObject: " + resource.name));
				yield break;
			}
			Object asset = resource.asset;
			DependsProperty prop = ((GameObject)((asset is GameObject) ? asset : null)).GetComponent<DependsProperty>();
			if (!((Object)(object)prop != (Object)null) || prop.dependencies == null || prop.dependencies.Length <= 0)
			{
				yield break;
			}
			Dependence[] dependencies = prop.dependencies;
			int i = 0;
			int num = dependencies.Length;
			base.destroyChildrenOnDestroy = true;
			for (int j = 0; j < num; j++)
			{
				if (string.IsNullOrEmpty(dependencies[j].path))
				{
					i++;
					continue;
				}
				Depend depend = RenderInstance.Create<Depend>(dependencies[j].path, this, 0, string.Empty);
				depend.onComplete = delegate
				{
					i++;
				};
				if (!Object.op_Implicit(dependencies[j].dependence))
				{
					depend.Attach(dependencies[j].assets);
				}
			}
			while (i < num)
			{
				yield return null;
			}
		}
	}
}
