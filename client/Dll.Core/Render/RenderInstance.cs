using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace XFX.Core.Render
{
	public static class RenderInstance
	{
		private class LoopKit : MonoBehaviour
		{
			public Action onUpdate;

			public LoopKit()
			{
			}

			private void Update()
			{
				if (onUpdate != null)
				{
					onUpdate();
				}
			}
		}

		private static IRenderFactory factory_;

		public static IRenderFactory factory => factory_;

		public static bool isIdle => factory_.isIdle;

		public static void Setup()
		{
			Setup(null);
		}

		public static IRenderFactory Setup(IRenderFactory instance)
		{
			if (factory_ == null)
			{
				if (instance != null)
				{
					factory_ = instance;
				}
				else
				{
					factory_ = new RenderFactory();
					GameObject val = new GameObject("RenderFactory");
					Object.DontDestroyOnLoad((Object)(object)val);
					LoopKit loopKit = val.AddComponent<LoopKit>();
					loopKit.onUpdate = delegate
					{
						factory_.Loop();
					};
				}
			}
			return factory_;
		}

		public static T Create<T>(string filename, IRenderObject parent = null, int priority = 0, string tag = "") where T : IRenderObject
		{
			if (factory_ == null)
			{
				throw new Exception("You need call RenderInstance.Setup first");
			}
			return factory_.CreateInstance<T>(filename, parent, priority, tag);
		}

		public static IRenderObject Create(Type type, string filename, IRenderObject parent = null, int priority = 0, string tag = "")
		{
			if (factory_ == null)
			{
				throw new Exception("You need call RenderInstance.Setup first");
			}
			return factory_.CreateInstance(type, filename, parent, priority, tag);
		}
	}
}
