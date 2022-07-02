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
				while (!((AsyncOperation)request).isDone)
				{
					if (progress != ((AsyncOperation)request).progress)
					{
						progress = ((AsyncOperation)request).progress;
						option.OnInvoke(this, load: true, done: false, progress);
					}
					yield return null;
				}
				asset_bundle = request.assetBundle;
			}
			option.OnInvoke(this, load: true, done: true, 1f);
		}

		public IEnumerator Unload()
		{
			option.OnInvoke(this, load: false, done: false, 0f);
			while (true)
			{
				UnityEngine.SceneManagement.Scene activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
				if (!((activeScene).name == name))
				{
					break;
				}
				yield return null;
			}
			int count = UnityEngine.SceneManagement.SceneManager.sceneCount;
			if (count > 1)
			{
				bool find = false;
				for (int i = 0; i < count; i++)
				{
					string text = name;
					UnityEngine.SceneManagement.Scene sceneAt = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
					if (text.Equals((sceneAt).name))
					{
						find = true;
						break;
					}
				}
				if (find)
				{
					AsyncOperation request = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(name);
					float progress = 0f;
					while (request != null && !request.isDone)
					{
						if (progress != request.progress)
						{
							progress = request.progress;
							option.OnInvoke(this, load: false, done: false, progress);
						}
						yield return null;
					}
				}
			}
			if (asset_bundle != null)
			{
				asset_bundle.Unload(true);
				asset_bundle = null;
			}
			option.OnInvoke(this, load: false, done: true, 1f);
		}
	}
}
