using UnityEngine;
using ZF.Core.Render;

namespace ZF.Asset
{
	public class Root : RenderObject, IRoot, IRenderObject
	{
		protected override void OnCreate(IRenderResource _)
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			if (string.IsNullOrEmpty(base.name))
			{
				base.name = "root";
			}
			base.gameObject = (GameObject)(((object)base.gameObject) ?? ((object)new GameObject(base.name)));
		}

		protected override void OnDestroy()
		{
			if ((Object)(object)base.gameObject != (Object)null)
			{
				RenderObject.DestroyObject(base.gameObject);
				base.gameObject = null;
			}
		}

		public static IRoot Create(GameObject go, IRenderObject parent)
		{
			IRoot root = null;
			if ((Object)(object)go != (Object)null)
			{
				Root root2 = new Root();
				root2.gameObject = go;
				root2.parent = parent;
				root = root2;
				root.Create(null);
			}
			else
			{
				root = RenderInstance.Create<Root>("empty", parent, 0, string.Empty);
			}
			root.destroyChildrenOnDestroy = true;
			root.dontDestroyOnLoad = true;
			root.oncmd = true;
			return root;
		}

		public static IRoot Create<T>(GameObject go, IRenderObject parent) where T : Root, new()
		{
			IRoot root = null;
			if ((Object)(object)go != (Object)null)
			{
				root = new T
				{
					gameObject = go,
					parent = parent
				};
				root.Create(null);
			}
			else
			{
				root = RenderInstance.Create<T>("empty", parent, 0, string.Empty);
			}
			root.destroyChildrenOnDestroy = true;
			root.dontDestroyOnLoad = true;
			root.oncmd = true;
			return root;
		}
	}
}
