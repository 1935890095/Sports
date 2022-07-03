using System;
using System.Collections.Generic;

namespace XFX.Core.Util
{
	internal class TypeCollection<T, U>
	{
		private static Type BASE_TYPE = typeof(U);

		private HashSet<T> hash_set = new HashSet<T>();

		private Dictionary<Type, T> dict_ = new Dictionary<Type, T>();

		public void Add(Type type, T instance)
		{
			if ((!BASE_TYPE.IsInterface || !BASE_TYPE.IsAssignableFrom(type)) && (!BASE_TYPE.IsClass || !type.IsSubclassOf(BASE_TYPE)))
			{
				throw new Exception($"invalid type, {type.Name} must implement/inherit from {BASE_TYPE.Name}(interface/class)");
			}
			if (type.BaseType != BASE_TYPE && BASE_TYPE.IsAssignableFrom(type.BaseType))
			{
				Add(type.BaseType, instance);
			}
			T value = default(T);
			if (dict_.TryGetValue(type, out value))
			{
				if (value.GetType() == instance.GetType())
				{
					throw new Exception($"add instance {instance.GetType().Name} duplicated");
				}
				if (value.GetType() == type)
				{
					return;
				}
				dict_.Remove(type);
			}
			dict_.Add(type, instance);
		}

		public T Remove(Type type)
		{
			if (!dict_.ContainsKey(type))
			{
				return default(T);
			}
			T val = dict_[type];
			if (val.GetType() != type)
			{
				return default(T);
			}
			Remove(type, val);
			return val;
		}

		private void Remove(Type type, T instance)
		{
			if (type.BaseType != BASE_TYPE && BASE_TYPE.IsAssignableFrom(type.BaseType))
			{
				Remove(type.BaseType, instance);
			}
			T value = default(T);
			if (dict_.TryGetValue(type, out value) && object.ReferenceEquals(value, instance))
			{
				dict_.Remove(type);
			}
		}

		public T Get(Type type)
		{
			T value = default(T);
			dict_.TryGetValue(type, out value);
			return value;
		}

		public void Clear()
		{
			dict_.Clear();
		}

		public void Foreach(Action<T> action)
		{
			if (action == null)
			{
				return;
			}
			hash_set.Clear();
			Dictionary<Type, T>.Enumerator enumerator = dict_.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (!hash_set.Contains(enumerator.Current.Value))
				{
					hash_set.Add(enumerator.Current.Value);
					action(enumerator.Current.Value);
				}
			}
			enumerator.Dispose();
		}
	}
	internal class TypeCollection<T> : TypeCollection<T, T>
	{
	}
}
