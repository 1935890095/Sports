using System;

namespace XFX.Core.Instance
{
	public abstract class Instance : IInstance
	{
		public string name { get; private set; }

		public IInstanceManager mgr { get; private set; }

		public T CreateSingle<T>() where T : IInstance
		{
			return mgr.CreateSingle<T>();
		}

		public T GetSingle<T>() where T : IInstance
		{
			return mgr.GetSingle<T>();
		}

		public IInstance CreateSingle(Type type)
		{
			return mgr.CreateSingle(type);
		}

		public IInstance GetSingle(Type type)
		{
			return mgr.GetSingle(type);
		}

		public T Create<T>(string name) where T : IInstance
		{
			return mgr.Create<T>(name);
		}

		public IInstance Create(Type type, string name)
		{
			return mgr.Create(type, name);
		}

		public T Get<T>(string name) where T : IInstance
		{
			return mgr.Get<T>(name);
		}

		public IInstance Get(Type type, string name)
		{
			return mgr.Get(type, name);
		}

		public void DestroySingle<T>() where T : IInstance
		{
			mgr.DestroySingle<T>();
		}

		public void DestroySingle(Type type)
		{
			mgr.DestroySingle(type);
		}

		public void Destroy<T>(string name) where T : IInstance
		{
			mgr.Destroy<T>(name);
		}

		public void Destroy(Type type, string name)
		{
			mgr.Destroy(type, name);
		}

		void IInstance.Init(IInstance mgr, string name)
		{
			this.mgr = mgr as IInstanceManager;
			this.name = name;
			OnInit();
		}

		public void Destroy()
		{
			if (mgr != null)
			{
				mgr.RemoveInstance(this, name);
				OnDestroy();
				mgr = null;
			}
		}

		protected virtual void OnInit()
		{
		}

		protected virtual void OnDestroy()
		{
		}
	}
}
