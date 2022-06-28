using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZF.Core.Util;

namespace ZF.Core.Scene
{
	public class Scene : IScene
	{
		protected AssetBundle asset_bundle;

		public string name { get; private set; }

		public ISceneOption option { get; set; }

		public SceneState state { get; set; }

		public Scene(string name, ISceneOption option)
		{
			this.name = name;
			this.option = option;
		}

		public IEnumerator Load(string dir, string ext)
		{
			option.OnInvoke(this, load: true, done: false, 0f);
			if (!option.builtin)
			{
				string asset_path = dir + "/" + name + "." + ext;
				AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(PathExt.MakeLoadPath(asset_path));
				float progress = 0f;
				while (!((AsyncOperation)request).get_isDone())
				{
					if (progress != ((AsyncOperation)request).get_progress())
					{
						progress = ((AsyncOperation)request).get_progress();
						option.OnInvoke(this, load: true, done: false, progress);
					}
					yield return null;
				}
				asset_bundle = request.get_assetBundle();
			}
			option.OnInvoke(this, load: true, done: true, 1f);
		}

		public IEnumerator Unload()
		{
			option.OnInvoke(this, load: false, done: false, 0f);
			while (true)
			{
				Scene activeScene = SceneManager.GetActiveScene();
				if (!(((Scene)(ref activeScene)).get_name() == name))
				{
					break;
				}
				yield return null;
			}
			int count = SceneManager.get_sceneCount();
			if (count > 1)
			{
				bool find = false;
				for (int i = 0; i < count; i++)
				{
					string text = name;
					Scene sceneAt = SceneManager.GetSceneAt(i);
					if (text.Equals(((Scene)(ref sceneAt)).get_name()))
					{
						find = true;
						break;
					}
				}
				if (find)
				{
					AsyncOperation request = SceneManager.UnloadSceneAsync(name);
					float progress = 0f;
					while (request != null && !request.get_isDone())
					{
						if (progress != request.get_progress())
						{
							progress = request.get_progress();
							option.OnInvoke(this, load: false, done: false, progress);
						}
						yield return null;
					}
				}
			}
			if ((Object)(object)asset_bundle != (Object)null)
			{
				asset_bundle.Unload(true);
				asset_bundle = null;
			}
			option.OnInvoke(this, load: false, done: true, 1f);
		}
	}
}
