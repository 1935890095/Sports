using System;
using System.Collections.Generic;

namespace ZF.Core.Util
{
	internal class TypeNameCollection<T, U>
	{
		private static Type BASE_TYPE = typeof(U);

		private HashSet<T> hash_set = new HashSet<T>();

		private Dictionary<Tuple<Type, string>, T> dict_ = new Dictionary<Tuple<Type, string>, T>();

		public void Add(Type type, string name, T instance)
		{
			if ((!BASE_TYPE.IsInterface || !BASE_TYPE.IsAssignableFrom(type)) && (!BASE_TYPE.IsClass || !type.IsSubclassOf(BASE_TYPE)))
			{
				throw new Exception($"invalid type, {type.Name} must implement/inherit from {BASE_TYPE.Name}(interface/class)");
			}
			if (type.BaseType != BASE_TYPE && BASE_TYPE.IsAssignableFrom(type.BaseType))
			{
				Add(type.BaseType, name, instance);
			}
			Tuple<Type, string> key = Tuple.Create(type, name);
			T value = default(T);
			if (dict_.TryGetValue(key, out value))
			{
				if (value.GetType() == instance.GetType())
				{
					throw new Exception($"Add instance {instance.GetType().Name} dunplication");
				}
				if (value.GetType() == type)
				{
					return;
				}
				dict_.Remove(key);
			}
			dict_.Add(key, instance);
		}

		public T Remove(Type type, string name)
		{
			Tuple<Type, string> key = Tuple.Create(type, name);
			if (!dict_.ContainsKey(key))
			{
				return default(T);
			}
			T val = dict_[key];
			if (val.GetType() != type)
			{
				return default(T);
			}
			Remove(type, name, val);
			return val;
		}

		private void Remove(Type type, string name, T instance)
		{
			if (type.BaseType != BASE_TYPE && BASE_TYPE.IsAssignableFrom(type.BaseType))
			{
				Remove(type.BaseType, name, instance);
			}
			Tuple<Type, string> key = Tuple.Create(type, name);
			T value = default(T);
			if (dict_.TryGetValue(key, out value) && object.ReferenceEquals(value, instance))
			{
				dict_.Remove(key);
			}
		}

		public T Get(Type type, string name)
		{
			Tuple<Type, string> key = Tuple.Create(type, name);
			T value = default(T);
			dict_.TryGetValue(key, out value);
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
			Dictionary<Tuple<Type, string>, T>.Enumerator enumerator = dict_.GetEnumerator();
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
	internal class TypeNameCollection<T> : TypeNameCollection<T, T>
	{
	}
}
