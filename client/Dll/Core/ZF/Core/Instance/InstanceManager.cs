using System;
using ZF.Core.Util;

namespace ZF.Core.Instance
{
	public class InstanceManager : IInstanceManager, IInstance
	{
		private TypeCollection<IInstance, Instance> singles = new TypeCollection<IInstance, Instance>();

		private TypeNameCollection<IInstance, Instance> objects = new TypeNameCollection<IInstance, Instance>();

		public IInstanceManager mgr => this;

		public T CreateSingle<T>() where T : IInstance
		{
			IInstance instance = CreateSingle(typeof(T));
			if (instance == null)
			{
				return default(T);
			}
			return (T)instance;
		}

		public T GetSingle<T>() where T : IInstance
		{
			IInstance single = GetSingle(typeof(T));
			if (single == null)
			{
				return default(T);
			}
			return (T)single;
		}

		public IInstance CreateSingle(Type type)
		{
			IInstance instance = (IInstance)Activator.CreateInstance(type);
			if (instance == null)
			{
				return null;
			}
			singles.Add(type, instance);
			Init(instance, string.Empty);
			return instance;
		}

		public IInstance GetSingle(Type type)
		{
			return singles.Get(type);
		}

		public T Create<T>(string name) where T : IInstance
		{
			IInstance instance = Create(typeof(T), name);
			if (instance == null)
			{
				return default(T);
			}
			return (T)instance;
		}

		public T Get<T>(string name) where T : IInstance
		{
			IInstance instance = Get(typeof(T), name);
			if (instance == null)
			{
				return default(T);
			}
			return (T)instance;
		}

		public IInstance Create(Type type, string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new Exception($"Create empty name, type {type.Name}");
			}
			IInstance instance = (IInstance)Activator.CreateInstance(type);
			if (instance == null)
			{
				return null;
			}
			objects.Add(type, name, instance);
			Init(instance, name);
			return instance;
		}

		public IInstance Get(Type type, string name)
		{
			return objects.Get(type, name);
		}

		public void DestroySingle<T>() where T : IInstance
		{
			DestroySingle(typeof(T));
		}

		public void DestroySingle(Type type)
		{
			singles.Remove(type)?.Destroy();
		}

		public void Destroy<T>(string name) where T : IInstance
		{
			Destroy(typeof(T), name);
		}

		public void Destroy(Type type, string name)
		{
			objects.Get(type, name)?.Destroy();
		}

		public bool RemoveInstance(IInstance obj, string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				IInstance instance = singles.Remove(obj.GetType());
				return instance != null;
			}
			IInstance instance2 = objects.Remove(obj.GetType(), name);
			return instance2 != null;
		}

		public virtual void Init(IInstance obj, string name)
		{
			obj.Init(this, name);
		}

		public void Destroy()
		{
		}
	}
}
