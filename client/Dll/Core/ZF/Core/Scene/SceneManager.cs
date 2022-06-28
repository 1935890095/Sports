using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZF.Core.Logging;

namespace ZF.Core.Scene
{
	public class SceneManager : ISceneManager
	{
		private Dictionary<string, IScene> dict = new Dictionary<string, IScene>();

		private Queue<IScene> load_queue = new Queue<IScene>();

		private Queue<IScene> unload_queue = new Queue<IScene>();

		private IEnumerator load_yield;

		private IEnumerator unload_yield;

		private ILogger logger_ = NullLogger.Instance;

		private SceneCreator creator;

		public string scene_dir { get; set; }

		public string scene_ext { get; set; }

		public ILogger logger
		{
			get
			{
				return logger_;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("logger can't be null");
				}
				logger_ = value;
			}
		}

		public SceneManager(SceneCreator creator = null)
		{
			this.creator = creator;
		}

		public IScene LoadScene(string name, ISceneOption option)
		{
			IScene value = null;
			if (dict.TryGetValue(name, out value))
			{
				if (value.state == SceneState.Unloading)
				{
					value.state = SceneState.Loading;
					if (option != null)
					{
						value.option = option;
					}
				}
			}
			else
			{
				if (option == null)
				{
					option = new DefaultSceneOption();
				}
				value = ((creator == null) ? new Scene(name, option) : creator(name, option));
				value.state = SceneState.Loading;
				dict.Add(name, value);
				load_queue.Enqueue(value);
			}
			return value;
		}

		public void UnloadScene(string name)
		{
			IScene value = null;
			dict.TryGetValue(name, out value);
			if (value != null && value.state != SceneState.Unloading)
			{
				if (value.state == SceneState.Instantiated)
				{
					value.state = SceneState.Unloading;
					unload_queue.Enqueue(value);
				}
				else
				{
					value.state = SceneState.Unloading;
				}
			}
		}

		public void UnloadScene(IScene scene)
		{
			if (scene.state != SceneState.Unloading)
			{
				scene.state = SceneState.Unloading;
				unload_queue.Enqueue(scene);
			}
		}

		public void UnloadAllScene()
		{
			Dictionary<string, IScene>.Enumerator enumerator = dict.GetEnumerator();
			while (enumerator.MoveNext())
			{
				IScene value = enumerator.Current.Value;
				if (value.state != SceneState.Unloading)
				{
					unload_queue.Enqueue(value);
				}
			}
			enumerator.Dispose();
		}

		public void Loop()
		{
			if (load_yield != null)
			{
				if (!load_yield.MoveNext())
				{
					load_yield = null;
				}
			}
			else if (load_queue.Count > 0)
			{
				IScene scene = load_queue.Peek();
				load_yield = LoadSceneInternal(scene);
			}
			if (unload_yield != null)
			{
				if (!unload_yield.MoveNext())
				{
					unload_yield = null;
				}
			}
			else if (unload_queue.Count > 0)
			{
				IScene scene2 = unload_queue.Peek();
				unload_yield = UnloadSceneInternal(scene2);
			}
		}

		private IEnumerator LoadSceneInternal(IScene scene)
		{
			IEnumerator itr2 = scene.Load(scene_dir, scene_ext);
			while (itr2.MoveNext())
			{
				yield return null;
			}
			bool find = false;
			if (load_queue.Count > 0)
			{
				IScene scene2 = load_queue.Peek();
				if (scene2.name == scene.name)
				{
					load_queue.Dequeue();
					find = true;
				}
			}
			if (!find)
			{
				logger.Fatal("load scene internal error");
				yield break;
			}
			if (scene.state == SceneState.Unloading)
			{
				unload_queue.Enqueue(scene);
				yield break;
			}
			scene.state = SceneState.Loaded;
			if (scene.option.synchronize)
			{
				InstantiateScene(scene);
				yield break;
			}
			itr2 = InstantiateSceneAsync(scene);
			while (itr2.MoveNext())
			{
				yield return null;
			}
		}

		private IEnumerator InstantiateSceneAsync(IScene scene)
		{
			scene.state = SceneState.Instantiating;
			if (scene.option.additive)
			{
				AsyncOperation request2 = SceneManager.LoadSceneAsync(scene.name, (LoadSceneMode)1);
				if (scene.option.denySceneActivation)
				{
					request2.set_allowSceneActivation(false);
				}
				yield return request2;
				SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene.name));
			}
			else
			{
				AsyncOperation request = SceneManager.LoadSceneAsync(scene.name);
				if (scene.option.denySceneActivation)
				{
					request.set_allowSceneActivation(false);
				}
				yield return request;
			}
			if (scene.state == SceneState.Unloading)
			{
				unload_queue.Enqueue(scene);
				yield break;
			}
			scene.state = SceneState.Instantiated;
			logger.Info("scene {0} loaded", scene.name);
		}

		private void InstantiateScene(IScene scene)
		{
			scene.state = SceneState.Instantiating;
			if (scene.option.additive)
			{
				SceneManager.LoadScene(scene.name, (LoadSceneMode)1);
			}
			else
			{
				SceneManager.LoadScene(scene.name);
			}
			scene.state = SceneState.Instantiated;
			logger.Info("scene {0} loaded", scene.name);
		}

		private IEnumerator UnloadSceneInternal(IScene scene)
		{
			IEnumerator itr = scene.Unload();
			while (itr.MoveNext())
			{
				yield return null;
			}
			bool find = false;
			if (unload_queue.Count > 0)
			{
				IScene scene2 = unload_queue.Peek();
				if (scene2.name == scene.name)
				{
					unload_queue.Dequeue();
					find = true;
				}
			}
			if (!find)
			{
				logger.Fatal("load scene internal error");
				yield break;
			}
			if (scene.state == SceneState.Loading)
			{
				load_queue.Enqueue(scene);
				yield break;
			}
			scene.state = SceneState.Unloaded;
			dict.Remove(scene.name);
			logger.Info("scene {0} unloaded", scene.name);
		}
	}
}
