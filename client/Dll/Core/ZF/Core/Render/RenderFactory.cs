using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ZF.Core.Render
{
	public class RenderFactory : IRenderFactory
	{
		public enum ResourceType
		{
			BUNDLE,
			TEXT,
			TEXTURE,
			Editor
		}

		public static class Settings
		{
			public static ResourceCreator EditorResourceCreator;

			public static bool ReplaceShaderInEditorMode;
		}

		private Dictionary<string, IRenderResource> dict;

		private Dictionary<string, float> idle;

		private Queue<IRenderResource> load_queue;

		private Queue<IRenderResource> prior_load_queue;

		private List<IEnumerator> load_yield;

		private Queue<string> remove_queue;

		public int concurrency { get; set; }

		public int lingerTime { get; set; }

		public bool isIdle => load_queue.Count == 0 && prior_load_queue.Count == 0 && load_yield.Count == 0;

		public RenderFactory()
		{
			dict = new Dictionary<string, IRenderResource>();
			idle = new Dictionary<string, float>();
			load_queue = new Queue<IRenderResource>();
			prior_load_queue = new Queue<IRenderResource>();
			load_yield = new List<IEnumerator>();
			remove_queue = new Queue<string>();
			concurrency = 1;
			lingerTime = 1;
		}

		public T CreateInstance<T>(string filename, IRenderObject parent = null, int priority = 0, string tag = "") where T : IRenderObject
		{
			return (T)CreateInstance(typeof(T), filename, parent, priority, tag);
		}

		public IRenderObject CreateInstance(Type type, string filename, IRenderObject parent = null, int priority = 0, string tag = "")
		{
			if (string.IsNullOrEmpty(filename))
			{
				throw new Exception("[RenderFactory]Create filename is empty");
			}
			if (type.IsSubclassOf(typeof(IRenderObject)))
			{
				throw new Exception($"CreateInstance invalid type: {type.Name}");
			}
			if (filename == "empty")
			{
				return _CreateInstanceEmpty(type, parent);
			}
			if (!dict.TryGetValue(filename, out var value))
			{
				switch (GetResourceType(filename))
				{
				case ResourceType.TEXT:
					value = new TextResource(filename, this, priority);
					break;
				case ResourceType.TEXTURE:
					value = new TextureResource(filename, this, priority);
					break;
				case ResourceType.Editor:
					if (Settings.EditorResourceCreator != null)
					{
						value = Settings.EditorResourceCreator(filename, this, priority);
						break;
					}
					throw new Exception("unsupported to load: " + filename);
				default:
					value = new RenderResource(filename, this, priority);
					break;
				}
				dict.Add(filename, value);
				if (priority > 0)
				{
					prior_load_queue.Enqueue(value);
				}
				else
				{
					load_queue.Enqueue(value);
				}
			}
			else
			{
				idle.Remove(filename);
			}
			return value.CreateInstance(type, parent);
		}

		protected ResourceType GetResourceType(string filename)
		{
			switch (Path.GetExtension(filename).ToLower())
			{
			case ".txt":
			case ".html":
			case ".htm":
			case ".xml":
			case ".bytes":
			case ".json":
			case ".csv":
			case ".yaml":
			case ".fnt":
				return ResourceType.TEXT;
			case ".png":
			case ".jpg":
				return ResourceType.TEXTURE;
			case ".prefab":
			case ".asset":
			case ".mp3":
			case ".avi":
				return ResourceType.Editor;
			default:
				return ResourceType.BUNDLE;
			}
		}

		private IRenderObject _CreateInstanceEmpty(Type type, IRenderObject parent)
		{
			IRenderObject renderObject = RenderResource.AllocInstance(type);
			renderObject.parent = parent;
			renderObject.Create(null);
			return renderObject;
		}

		public void RemoveResource(IRenderResource resource)
		{
			if (!dict.ContainsKey(resource.name))
			{
				if (resource.loading)
				{
					throw new Exception("Can not destroy resource when loading");
				}
				resource.Destroy();
			}
			else
			{
				float realtimeSinceStartup = Time.get_realtimeSinceStartup();
				idle[resource.name] = realtimeSinceStartup;
			}
		}

		public void Loop()
		{
			int num = 0;
			while (prior_load_queue.Count > 0 && num < concurrency)
			{
				IRenderResource renderResource = prior_load_queue.Dequeue();
				load_yield.Add(renderResource.Load());
				num++;
			}
			while (load_queue.Count > 0 && num < concurrency)
			{
				IRenderResource renderResource2 = load_queue.Dequeue();
				load_yield.Add(renderResource2.Load());
				num++;
			}
			if (load_yield.Count > 0)
			{
				int num2 = 0;
				while (num2 < load_yield.Count)
				{
					IEnumerator enumerator = load_yield[num2];
					bool flag = false;
					try
					{
						flag = enumerator.MoveNext();
					}
					catch (Exception ex)
					{
						flag = false;
						Debug.LogError((object)("[RenderFactroy] error " + ex.ToString()));
					}
					if (!flag)
					{
						load_yield.RemoveAt(num2);
					}
					else
					{
						num2++;
					}
				}
			}
			float realtimeSinceStartup = Time.get_realtimeSinceStartup();
			Dictionary<string, float>.Enumerator enumerator2 = idle.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				float value = enumerator2.Current.Value;
				if (!(realtimeSinceStartup < value + (float)lingerTime))
				{
					remove_queue.Enqueue(enumerator2.Current.Key);
				}
			}
			enumerator2.Dispose();
			while (remove_queue.Count > 0)
			{
				string key = remove_queue.Dequeue();
				if (dict.TryGetValue(key, out var value2))
				{
					if (value2.loading)
					{
						continue;
					}
					value2.Destroy();
					dict.Remove(key);
				}
				idle.Remove(key);
			}
		}
	}
}
