using System.Collections;
using XFX.Core.Render;
using XFX.Core.Scene;

namespace XFX.Asset
{
	public class Scene : IScene
	{
		private IScene scene;

		private IRenderObject root;

		public string name { get; private set; }

		public ISceneOption option
		{
			get
			{
				return scene.option;
			}
			set
			{
				scene.option = value;
			}
		}

		public SceneState state
		{
			get
			{
				return scene.state;
			}
			set
			{
				if (value == SceneState.Instantiated)
				{
					OnComplete();
				}
				scene.state = value;
			}
		}

		public Scene(string name, ISceneOption option, IRenderObject root)
		{
			this.root = RenderInstance.Create<Empty>("empty", root, 0, string.Empty);
			this.root.destroyChildrenOnDestroy = true;
			this.root.name = name;
			scene = new XFX.Core.Scene.Scene(name, option);
			this.name = name;
		}

		public IEnumerator Load(string dir, string ext)
		{
			return scene.Load(dir, ext);
		}

		public IEnumerator Unload()
		{
			IEnumerator itr = scene.Unload();
			while (itr.MoveNext())
			{
				yield return null;
			}
			if (root != null)
			{
				root.Destroy();
				root = null;
			}
		}

		protected void OnComplete()
		{
			RenderInstance.Create<SceneDepends>("empty", root, 0, string.Empty);
		}
	}
}
