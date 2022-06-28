using System;

namespace ZF.Core.Router
{
	public interface IRouter
	{
		IDisposable On(int id, Action action);

		IDisposable On<T1>(int id, Action<T1> action);

		IDisposable On<T1, T2>(int id, Action<T1, T2> action);

		IDisposable On<T1, T2, T3>(int id, Action<T1, T2, T3> action);

		IDisposable On<T1, T2, T3, T4>(int id, Action<T1, T2, T3, T4> action);

		IDisposable On<T1, T2, T3, T4, T5>(int id, Action<T1, T2, T3, T4, T5> action);

		void Event(int id);

		void Event<T1>(int id, T1 arg1);

		void Event<T1, T2>(int id, T1 arg1, T2 arg2);

		void Event<T1, T2, T3>(int id, T1 arg1, T2 arg2, T3 arg3);

		void Event<T1, T2, T3, T4>(int id, T1 arg1, T2 arg2, T3 arg3, T4 arg4);

		void Event<T1, T2, T3, T4, T5>(int id, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
	}
}
