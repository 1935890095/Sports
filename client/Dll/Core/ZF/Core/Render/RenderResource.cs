using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZF.Core.Util;

namespace ZF.Core.Render
{
	internal class RenderResource : IRenderResource
	{
		private IRenderFactory factory;

		private List<IRenderObject> insts;

		private AssetBundle asset_bundle;

		private Object[] assets;

		public string name { get; private set; }

		public bool complete { get; private set; }

		public bool loading { get; private set; }

		public int priority { get; private set; }

		public Object asset => assets[0];

		public string text => null;

		public RenderResource(string name, IRenderFactory factory, int priority)
		{
			this.name = name;
			this.factory = factory;
			insts = new List<IRenderObject>();
			this.priority = priority;
		}

		public IRenderObject CreateInstance(Type type, IRenderObject parent)
		{
			IRenderObject renderObject = AllocInstance(type);
			renderObject.parent = parent;
			if (complete)
			{
				renderObject.Create(this);
			}
			insts.Add(renderObject);
			return renderObject;
		}

		public void RemoveInstance(IRenderObject inst)
		{
			if (insts.Remove(inst) && insts.Count == 0)
			{
				factory.RemoveResource(this);
			}
		}

		public IEnumerator Load()
		{
			loading = true;
			AssetBundleCreateRequest createrequest = AssetBundle.LoadFromFileAsync(PathExt.MakeLoadPath(name));
			((AsyncOperation)createrequest).set_priority(priority);
			while (!((AsyncOperation)createrequest).get_isDone())
			{
				yield return null;
			}
			asset_bundle = createrequest.get_assetBundle();
			if ((Object)(object)asset_bundle == (Object)null)
			{
				Debug.LogError((object)("[RenderResource] error: " + name));
				loading = false;
				yield break;
			}
			if (!asset_bundle.get_isStreamedSceneAssetBundle())
			{
				if (priority > 0)
				{
					assets = asset_bundle.LoadAllAssets();
				}
				else
				{
					AssetBundleRequest request = asset_bundle.LoadAllAssetsAsync();
					while (!((AsyncOperation)request).get_isDone())
					{
						yield return null;
					}
					assets = request.get_allAssets();
				}
			}
			if (assets == null || assets.Length == 0)
			{
				assets = (Object[])(object)new Object[1];
			}
			loading = false;
			Create();
		}

		private void Create()
		{
			OnCreate(asset_bundle);
			complete = true;
			int count = insts.Count;
			int num = 0;
			while (num < count)
			{
				IRenderObject renderObject = insts[num];
				renderObject.Create(this);
				if (count != insts.Count)
				{
					count = insts.Count;
				}
				else
				{
					num++;
				}
			}
		}

		public void Destroy()
		{
			if (loading)
			{
				throw new Exception(string.Format("attempt to destroy loading resource ", name));
			}
			OnDestroy();
			if ((Object)(object)asset_bundle != (Object)null)
			{
				asset_bundle.Unload(true);
				asset_bundle = null;
			}
			complete = false;
			loading = false;
		}

		protected virtual void OnCreate(AssetBundle asset_bundle)
		{
		}

		protected virtual void OnDestroy()
		{
		}

		internal static IRenderObject AllocInstance(Type type)
		{
			return (IRenderObject)Activator.CreateInstance(type);
		}
	}
}
