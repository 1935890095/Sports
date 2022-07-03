using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using XFX.Core.Util;

namespace XFX.Core.Router
{
	public class Router : IRouter
	{
		private class Handle : PListNode<Handle>, IDisposable
		{
			private interface IArgs
			{
			}

			[StructLayout(LayoutKind.Sequential, Size = 1)]
			private struct Args : IArgs
			{
			}

			private struct Args<T1> : IArgs
			{
				public T1 v1;
			}

			private struct Args<T1, T2> : IArgs
			{
				public T1 v1;

				public T2 v2;
			}

			private struct Args<T1, T2, T3> : IArgs
			{
				public T1 v1;

				public T2 v2;

				public T3 v3;
			}

			private struct Args<T1, T2, T3, T4> : IArgs
			{
				public T1 v1;

				public T2 v2;

				public T3 v3;

				public T4 v4;
			}

			private struct Args<T1, T2, T3, T4, T5> : IArgs
			{
				public T1 v1;

				public T2 v2;

				public T3 v3;

				public T4 v4;

				public T5 v5;
			}

			private delegate void Reducer(IArgs args);

			private Reducer reducer;

			internal HandleList list;

			private Handle(Reducer reducer)
			{
				this.reducer = reducer;
			}

			public void Execute()
			{
				reducer(null);
			}

			public void Execute<T1>(T1 arg1)
			{
				reducer(new Args<T1>
				{
					v1 = arg1
				});
			}

			public void Execute<T1, T2>(T1 arg1, T2 arg2)
			{
				reducer(new Args<T1, T2>
				{
					v1 = arg1,
					v2 = arg2
				});
			}

			public void Execute<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3)
			{
				reducer(new Args<T1, T2, T3>
				{
					v1 = arg1,
					v2 = arg2,
					v3 = arg3
				});
			}

			public void Execute<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
			{
				reducer(new Args<T1, T2, T3, T4>
				{
					v1 = arg1,
					v2 = arg2,
					v3 = arg3,
					v4 = arg4
				});
			}

			public void Execute<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
			{
				reducer(new Args<T1, T2, T3, T4, T5>
				{
					v1 = arg1,
					v2 = arg2,
					v3 = arg3,
					v4 = arg4,
					v5 = arg5
				});
			}

			public static Handle Create(Action action)
			{
				return new Handle(CreateReducer(action));
			}

			public static Handle Create<T1>(Action<T1> action)
			{
				return new Handle(CreateReducer(action));
			}

			public static Handle Create<T1, T2>(Action<T1, T2> action)
			{
				return new Handle(CreateReducer(action));
			}

			public static Handle Create<T1, T2, T3>(Action<T1, T2, T3> action)
			{
				return new Handle(CreateReducer(action));
			}

			public static Handle Create<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action)
			{
				return new Handle(CreateReducer(action));
			}

			public static Handle Create<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action)
			{
				return new Handle(CreateReducer(action));
			}

			private static Reducer CreateReducer(Action action)
			{
				return delegate
				{
					action();
				};
			}

			private static Reducer CreateReducer<T1>(Action<T1> action)
			{
				return delegate(IArgs arg)
				{
					Args<T1> args = (Args<T1>)(object)arg;
					action(args.v1);
				};
			}

			private static Reducer CreateReducer<T1, T2>(Action<T1, T2> action)
			{
				return delegate(IArgs arg)
				{
					Args<T1, T2> args = (Args<T1, T2>)(object)arg;
					action(args.v1, args.v2);
				};
			}

			private static Reducer CreateReducer<T1, T2, T3>(Action<T1, T2, T3> action)
			{
				return delegate(IArgs arg)
				{
					Args<T1, T2, T3> args = (Args<T1, T2, T3>)(object)arg;
					action(args.v1, args.v2, args.v3);
				};
			}

			private static Reducer CreateReducer<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action)
			{
				return delegate(IArgs arg)
				{
					Args<T1, T2, T3, T4> args = (Args<T1, T2, T3, T4>)(object)arg;
					action(args.v1, args.v2, args.v3, args.v4);
				};
			}

			private static Reducer CreateReducer<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action)
			{
				return delegate(IArgs arg)
				{
					Args<T1, T2, T3, T4, T5> args = (Args<T1, T2, T3, T4, T5>)(object)arg;
					action(args.v1, args.v2, args.v3, args.v4, args.v5);
				};
			}

			public void Dispose()
			{
				if (list != null)
				{
					list.Remove(this);
				}
				list = null;
			}
		}

		private class HandleList : PList<Handle>
		{
		}

		private Dictionary<int, HandleList> dict = new Dictionary<int, HandleList>();

		public IDisposable On(int id, Action action)
		{
			return Reg(id, Handle.Create(action));
		}

		public IDisposable On<T1>(int id, Action<T1> action)
		{
			return Reg(id, Handle.Create(action));
		}

		public IDisposable On<T1, T2>(int id, Action<T1, T2> action)
		{
			return Reg(id, Handle.Create(action));
		}

		public IDisposable On<T1, T2, T3>(int id, Action<T1, T2, T3> action)
		{
			return Reg(id, Handle.Create(action));
		}

		public IDisposable On<T1, T2, T3, T4>(int id, Action<T1, T2, T3, T4> action)
		{
			return Reg(id, Handle.Create(action));
		}

		public IDisposable On<T1, T2, T3, T4, T5>(int id, Action<T1, T2, T3, T4, T5> action)
		{
			return Reg(id, Handle.Create(action));
		}

		public void Event(int id)
		{
			ViewHandles(id, delegate(Handle handle)
			{
				handle.Execute();
			});
		}

		public void Event<T1>(int id, T1 arg1)
		{
			ViewHandles(id, delegate(Handle handle)
			{
				handle.Execute(arg1);
			});
		}

		public void Event<T1, T2>(int id, T1 arg1, T2 arg2)
		{
			ViewHandles(id, delegate(Handle handle)
			{
				handle.Execute(arg1, arg2);
			});
		}

		public void Event<T1, T2, T3>(int id, T1 arg1, T2 arg2, T3 arg3)
		{
			ViewHandles(id, delegate(Handle handle)
			{
				handle.Execute(arg1, arg2, arg3);
			});
		}

		public void Event<T1, T2, T3, T4>(int id, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			ViewHandles(id, delegate(Handle handle)
			{
				handle.Execute(arg1, arg2, arg3, arg4);
			});
		}

		public void Event<T1, T2, T3, T4, T5>(int id, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
		{
			ViewHandles(id, delegate(Handle handle)
			{
				handle.Execute(arg1, arg2, arg3, arg4, arg5);
			});
		}

		private void ViewHandles(int id, Action<Handle> handle)
		{
			dict.TryGetValue(id, out var value);
			if (value != null)
			{
				Handle handle2 = value.Next(null);
				while (handle2 != null)
				{
					Handle handle3 = value.Next(handle2);
					handle?.Invoke(handle2);
					handle2 = handle3;
				}
			}
		}

		private IDisposable Reg(int id, Handle handle)
		{
			if (!dict.TryGetValue(id, out var value))
			{
				value = new HandleList();
				dict.Add(id, value);
			}
			handle.list = value;
			value.AddTail(handle);
			return handle;
		}
	}
}
