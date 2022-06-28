using System;

namespace ZF.Core.Instance
{
	public interface IInstance
	{
		T CreateSingle<T>() where T : IInstance;

		T GetSingle<T>() where T : IInstance;

		IInstance CreateSingle(Type type);

		IInstance GetSingle(Type type);

		T Create<T>(string name) where T : IInstance;

		T Get<T>(string name) where T : IInstance;

		IInstance Create(Type type, string name);

		IInstance Get(Type type, string name);

		void DestroySingle<T>() where T : IInstance;

		void DestroySingle(Type type);

		void Destroy<T>(string name) where T : IInstance;

		void Destroy(Type type, string name);

		void Init(IInstance inst, string name);

		void Destroy();
	}
}
