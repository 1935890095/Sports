using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using ZF.Core.Util;
using Object = UnityEngine.Object;

namespace ZF.Core.Render
{
	internal class TextResource : IRenderResource
	{
		private IRenderFactory factory;

		private List<IRenderObject> insts;

		public string name { get; private set; }

		public bool complete { get; private set; }

		public int priority { get; private set; }

		public bool loading { get; private set; }

		public Object asset { get; private set; }

		public string text { get; private set; }

		public TextResource(string name, IRenderFactory factory, int priority)
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
			UnityWebRequest request = UnityWebRequest.Get(PathExt.MakeWWWPath(name));
			try
			{
				yield return request.SendWebRequest();
				if (request.isHttpError || request.isNetworkError)
				{
					Debug.LogError((object)request.error);
				}
				else
				{
					text = request.downloadHandler.text;
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
