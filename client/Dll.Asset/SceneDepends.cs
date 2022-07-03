using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using XFX.Asset.Properties;
using XFX.Core.Render;
using Object = UnityEngine.Object;

namespace XFX.Asset
{
	internal class SceneDepends : Depends, IRenderResource
	{
		private SceneProperty property;

		public int priority { get; }

		public bool loading { get; }

		public Object asset { get; private set; }

		public string text { get; }

		protected override IEnumerator OnCreateAs(IRenderResource _)
		{
			string scenename = base.parent.name;
			UnityEngine.SceneManagement.Scene scene;
			while (true)
			{
				scene = SceneManager.GetActiveScene();
				if (scene.name == scenename)
				{
					break;
				}
				yield return null;
			}
			GameObject[] roots = scene.GetRootGameObjects();
			for (int i = 0; i < roots.Length; i++)
			{
				property = roots[i].GetComponent<SceneProperty>();
				if ((Object)(object)property != (Object)null)
				{
					asset = (Object)(object)roots[i];
					break;
				}
			}
			if ((Object)(object)property != (Object)null)
			{
				base.renderers = property.renderers;
			}
			IEnumerator itr = base.OnCreateAs((IRenderResource)this);
			while (itr.MoveNext())
			{
				yield return null;
			}
		}

		public IEnumerator Load()
		{
			return null;
		}

		public IRenderObject CreateInstance(Type type, IRenderObject parent)
		{
			return null;
		}

		public void RemoveInstance(IRenderObject obj)
		{
		}

		string IRenderResource.name
		{
			get{
			return base.name;
			}
		}

		bool IRenderResource.complete
		{
			get{
				return base.complete;
			}
		}

		void IRenderResource.Destroy()
		{
			Destroy();
		}
	}
}
