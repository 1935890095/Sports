using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Object =UnityEngine.Object;
using ZF.Core.Util;

namespace ZF.Core.Render
{
	internal class TextureResource : IRenderResource
	{
		private IRenderFactory factory;

		private List<IRenderObject> insts;

		public string name { get; private set; }

		public bool complete { get; private set; }

		public int priority { get; private set; }

		public bool loading { get; private set; }

		public Object asset { get; private set; }

		public string text { get; private set; }

		public TextureResource(string name, IRenderFactory factory, int priority)
		{
			this.name = name;
			this.factory = factory;
			this.priority = priority;
			insts = new List<IRenderObject>();
		}

		public IRenderObject CreateInstance(Type type, IRenderObject parent)
		{
			IRenderObject renderObject = (IRenderObject)Activator.CreateInstance(type);
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
			UnityWebRequest request = UnityWebRequestTexture.GetTexture(PathExt.MakeWWWPath(name));
			try
			{
				yield return request.SendWebRequest();
				if (request.isNetworkError || request.isHttpError)
				{
					Debug.LogError((object)request.error);
				}
				else
				{
					asset = (Object)(object)DownloadHandlerTexture.GetContent(request);
				}
			}
			finally
			{
				
			}
			loading = false;
			complete = true;
			for (int i = 0; i < insts.Count; i++)
			{
				insts[i].Create(this);
			}
		}

		public void Destroy()
		{
			if (loading)
			{
				throw new Exception($"attempt to destroy loading resource {name}");
			}
			if (asset != (Object)null)
			{
				Object.Destroy(asset);
				asset = null;
			}
			text = null;
			complete = false;
			loading = false;
		}
	}
}
