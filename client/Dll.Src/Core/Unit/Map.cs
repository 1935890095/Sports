using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace XFX.Core.Util
{
	[Serializable]
	[ComVisible(false)]
	[DebuggerDisplay("Count={Count}")]
	[DebuggerTypeProxy(typeof(CollectionDebuggerView<, >))]
	public class Map<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary, ISerializable, IDeserializationCallback, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, ICollection
	{
		private delegate TRet Transform<TRet>(TKey key, TValue value);

		[Serializable]
		private class ShimEnumerator : IDictionaryEnumerator, IEnumerator
		{
			private Enumerator host_enumerator;

			public DictionaryEntry Entry => ((IDictionaryEnumerator)host_enumerator).Entry;

			public object Key => host_enumerator.Current.Key;

			public object Value => host_enumerator.Current.Value;

			public object Current => Entry;

			public ShimEnumerator(Map<TKey, TValue> host)
			{
				host_enumerator = host.GetEnumerator();
			}

			public void Dispose()
			{
				host_enumerator.Dispose();
			}

			public bool MoveNext()
			{
				return host_enumerator.MoveNext();
			}

			public void Reset()
			{
				host_enumerator.Reset();
			}
		}

		[Serializable]
		public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDisposable, IDictionaryEnumerator, IEnumerator
		{
			private Map<TKey, TValue> mapContainer;

			private int next;

			private int stamp;

			internal KeyValuePair<TKey, TValue> current;

			object IEnumerator.Current
			{
				get
				{
					VerifyCurrent();
					return current;
				}
			}

			DictionaryEntry IDictionaryEnumerator.Entry
			{
				get
				{
					VerifyCurrent();
					return new DictionaryEntry(current.Key, current.Value);
				}
			}

			object IDictionaryEnumerator.Key => CurrentKey;

			object IDictionaryEnumerator.Value => CurrentValue;

			public KeyValuePair<TKey, TValue> Current => current;

			internal TKey CurrentKey
			{
				get
				{
					VerifyCurrent();
					return current.Key;
				}
			}

			internal TValue CurrentValue
			{
				get
				{
					VerifyCurrent();
					return current.Value;
				}
			}

			internal Enumerator(Map<TKey, TValue> mapContainer)
			{
				this = default(Enumerator);
				this.mapContainer = mapContainer;
				stamp = mapContainer.generation;
			}

			public bool MoveNext()
			{
				VerifyState();
				if (next < 0)
				{
					return false;
				}
				while (next < mapContainer.touchedSlots)
				{
					int num = next++;
					if (((uint)mapContainer.linkSlots[num].HashCode & 0x80000000u) != 0)
					{
						current = new KeyValuePair<TKey, TValue>(mapContainer.keySlots[num], mapContainer.valueSlots[num]);
						return true;
					}
				}
				next = -1;
				return false;
			}

			void IEnumerator.Reset()
			{
				Reset();
			}

			internal void Reset()
			{
				VerifyState();
				next = 0;
			}

			private void VerifyState()
			{
				if (mapContainer == null)
				{
					throw new ObjectDisposedException(null);
				}
				if (mapContainer.generation != stamp)
				{
					throw new InvalidOperationException("out of sync");
				}
			}

			private void VerifyCurrent()
			{
				VerifyState();
				if (next <= 0)
				{
					throw new InvalidOperationException("Current is not valid");
				}
			}

			public void Dispose()
			{
				mapContainer = null;
			}
		}

		[Serializable]
		[DebuggerDisplay("Count={Count}")]
		[DebuggerTypeProxy(typeof(CollectionDebuggerView<, >))]
		public sealed class KeyCollection : ICollection<TKey>, IEnumerable<TKey>, ICollection, IEnumerable
		{
			[Serializable]
			public struct Enumerator : IEnumerator<TKey>, IDisposable, IEnumerator
			{
				private Map<TKey, TValue>.Enumerator host_enumerator;

				object IEnumerator.Current => host_enumerator.CurrentKey;

				public TKey Current => host_enumerator.current.Key;

				internal Enumerator(Map<TKey, TValue> host)
				{
					host_enumerator = host.GetEnumerator();
				}

				public void Dispose()
				{
					host_enumerator.Dispose();
				}

				public bool MoveNext()
				{
					return host_enumerator.MoveNext();
				}

				void IEnumerator.Reset()
				{
					host_enumerator.Reset();
				}
			}

			private Map<TKey, TValue> mapContainer;

			bool ICollection<TKey>.IsReadOnly => true;

			bool ICollection.IsSynchronized => false;

			object ICollection.SyncRoot => ((ICollection)mapContainer).SyncRoot;

			public int Count => mapContainer.Count;

			public KeyCollection(Map<TKey, TValue> mapContainer)
			{
				if (mapContainer == null)
				{
					throw new ArgumentNullException("mapContainer");
				}
				this.mapContainer = mapContainer;
			}

			public void CopyTo(TKey[] array, int index)
			{
				mapContainer.CopyToCheck(array, index);
				mapContainer.CopyKeys(array, index);
			}

			public Enumerator GetEnumerator()
			{
				return new Enumerator(mapContainer);
			}

			void ICollection<TKey>.Add(TKey item)
			{
				throw new NotSupportedException("this is a read-only collection");
			}

			void ICollection<TKey>.Clear()
			{
				throw new NotSupportedException("this is a read-only collection");
			}

			bool ICollection<TKey>.Contains(TKey item)
			{
				return mapContainer.ContainsKey(item);
			}

			bool ICollection<TKey>.Remove(TKey item)
			{
				throw new NotSupportedException("this is a read-only collection");
			}

			IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator()
			{
				return GetEnumerator();
			}

			void ICollection.CopyTo(Array array, int index)
			{
				TKey[] array2 = array as TKey[];
				if (array2 != null)
				{
					CopyTo(array2, index);
					return;
				}
				mapContainer.CopyToCheck(array, index);
				mapContainer.Do_ICollectionCopyTo(array, index, Map<TKey, TValue>.pick_key);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}

		[Serializable]
		[DebuggerDisplay("Count={Count}")]
		[DebuggerTypeProxy(typeof(CollectionDebuggerView<, >))]
		public sealed class ValueCollection : ICollection<TValue>, IEnumerable<TValue>, ICollection, IEnumerable
		{
			[Serializable]
			public struct Enumerator : IEnumerator<TValue>, IDisposable, IEnumerator
			{
				private Map<TKey, TValue>.Enumerator host_enumerator;

				object IEnumerator.Current => host_enumerator.CurrentValue;

				public TValue Current => host_enumerator.current.Value;

				internal Enumerator(Map<TKey, TValue> host)
				{
					host_enumerator = host.GetEnumerator();
				}

				public void Dispose()
				{
					host_enumerator.Dispose();
				}

				public bool MoveNext()
				{
					return host_enumerator.MoveNext();
				}

				void IEnumerator.Reset()
				{
					host_enumerator.Reset();
				}
			}

			private Map<TKey, TValue> mapContainer;

			bool ICollection<TValue>.IsReadOnly => true;

			bool ICollection.IsSynchronized => false;

			object ICollection.SyncRoot => ((ICollection)mapContainer).SyncRoot;

			public int Count => mapContainer.Count;

			public ValueCollection(Map<TKey, TValue> mapContainer)
			{
				if (mapContainer == null)
				{
					throw new ArgumentNullException("mapContainer");
				}
				this.mapContainer = mapContainer;
			}

			public void CopyTo(TValue[] array, int index)
			{
				mapContainer.CopyToCheck(array, index);
				mapContainer.CopyValues(array, index);
			}

			public Enumerator GetEnumerator()
			{
				return new Enumerator(mapContainer);
			}

			void ICollection<TValue>.Add(TValue item)
			{
				throw new NotSupportedException("this is a read-only collection");
			}

			void ICollection<TValue>.Clear()
			{
				throw new NotSupportedException("this is a read-only collection");
			}

			bool ICollection<TValue>.Contains(TValue item)
			{
				return mapContainer.ContainsValue(item);
			}

			bool ICollection<TValue>.Remove(TValue item)
			{
				throw new NotSupportedException("this is a read-only collection");
			}

			IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
			{
				return GetEnumerator();
			}

			void ICollection.CopyTo(Array array, int index)
			{
				TValue[] array2 = array as TValue[];
				if (array2 != null)
				{
					CopyTo(array2, index);
					return;
				}
				mapContainer.CopyToCheck(array, index);
				mapContainer.Do_ICollectionCopyTo(array, index, Map<TKey, TValue>.pick_value);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}

		private const int INITIAL_SIZE = 10;

		private const float DEFAULT_LOAD_FACTOR = 0.9f;

		private const int NO_SLOT = -1;

		private const int HASH_FLAG = int.MinValue;

		private int[] table;

		private Link[] linkSlots;

		private TKey[] keySlots;

		private TValue[] valueSlots;

		private IEqualityComparer<TKey> hcp;

		private SerializationInfo serialization_info;

		private int touchedSlots;

		private int emptySlot;

		private int count;

		private int threshold;

		private int generation;

		private int iteratorIndex = -1;

		private bool iteratorState;

		ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys;

		ICollection<TValue> IDictionary<TKey, TValue>.Values => Values;

		ICollection IDictionary.Keys => Keys;

		ICollection IDictionary.Values => Values;

		bool IDictionary.IsFixedSize => false;

		bool IDictionary.IsReadOnly => false;

		object IDictionary.this[object key]
		{
			get
			{
				if (key is TKey && ContainsKey((TKey)key))
				{
					return this[ToTKey(key)];
				}
				return null;
			}
			set
			{
				this[ToTKey(key)] = ToTValue(value);
			}
		}

		bool ICollection.IsSynchronized => false;

		object ICollection.SyncRoot => this;

		bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

		public int Count => count;

		public TValue this[TKey key]
		{
			get
			{
				if (key == null)
				{
					throw new ArgumentNullException("key");
				}
				int num = hcp.GetHashCode(key) | int.MinValue;
				for (int num2 = table[(num & 0x7FFFFFFF) % table.Length] - 1; num2 != -1; num2 = linkSlots[num2].Next)
				{
					if (linkSlots[num2].HashCode == num && hcp.Equals(keySlots[num2], key))
					{
						return valueSlots[num2];
					}
				}
				throw new KeyNotFoundException();
			}
			set
			{
				if (key == null)
				{
					throw new ArgumentNullException("key");
				}
				int num = hcp.GetHashCode(key) | int.MinValue;
				int num2 = (num & 0x7FFFFFFF) % table.Length;
				int num3 = table[num2] - 1;
				int num4 = -1;
				if (num3 != -1)
				{
					while (linkSlots[num3].HashCode != num || !hcp.Equals(keySlots[num3], key))
					{
						num4 = num3;
						num3 = linkSlots[num3].Next;
						if (num3 == -1)
						{
							break;
						}
					}
				}
				if (num3 == -1)
				{
					if (++count > threshold)
					{
						Resize();
						num2 = (num & 0x7FFFFFFF) % table.Length;
					}
					num3 = emptySlot;
					if (num3 == -1)
					{
						num3 = touchedSlots++;
					}
					else
					{
						emptySlot = linkSlots[num3].Next;
					}
					linkSlots[num3].Next = table[num2] - 1;
					table[num2] = num3 + 1;
					linkSlots[num3].HashCode = num;
					keySlots[num3] = key;
				}
				else if (num4 != -1)
				{
					linkSlots[num4].Next = linkSlots[num3].Next;
					linkSlots[num3].Next = table[num2] - 1;
					table[num2] = num3 + 1;
				}
				valueSlots[num3] = value;
				generation++;
			}
		}

		public IEqualityComparer<TKey> Comparer => hcp;

		public KeyCollection Keys => new KeyCollection(this);

		public ValueCollection Values => new ValueCollection(this);

		public TKey Key
		{
			get
			{
				if (!iteratorState)
				{
					throw new Exception("Key must access in iterator");
				}
				if (iteratorIndex < 0 || iteratorIndex >= touchedSlots)
				{
					throw new IndexOutOfRangeException("iterator invalid");
				}
				return keySlots[iteratorIndex];
			}
		}

		public TValue Value
		{
			get
			{
				if (!iteratorState)
				{
					throw new Exception("Value must access in iterator");
				}
				if (iteratorIndex < 0 || iteratorIndex >= touchedSlots)
				{
					throw new IndexOutOfRangeException("iterator invalid");
				}
				return valueSlots[iteratorIndex];
			}
		}

		public Map()
		{
			Init(10, null);
		}

		public Map(IEqualityComparer<TKey> comparer)
		{
			Init(10, comparer);
		}

		public Map(IDictionary<TKey, TValue> mapContainer)
			: this(mapContainer, (IEqualityComparer<TKey>)null)
		{
		}

		public Map(int capacity)
		{
			Init(capacity, null);
		}

		public Map(IDictionary<TKey, TValue> mapContainer, IEqualityComparer<TKey> comparer)
		{
			if (mapContainer == null)
			{
				throw new ArgumentNullException("mapContainer");
			}
			int capacity = mapContainer.Count;
			Init(capacity, comparer);
			foreach (KeyValuePair<TKey, TValue> item in mapContainer)
			{
				Add(item.Key, item.Value);
			}
		}

		public Map(int capacity, IEqualityComparer<TKey> comparer)
		{
			Init(capacity, comparer);
		}

		protected Map(SerializationInfo info, StreamingContext context)
		{
			serialization_info = info;
		}

		public Map(IList<TKey> keyList, IList<TValue> valList)
		{
			if (keyList.Count != valList.Count)
			{
				throw new Exception("Key of the number not equal to value of the number");
			}
			Init(keyList.Count, null);
			int num = keyList.Count;
			for (int i = 0; i < num; i++)
			{
				Add(keyList[i], valList[i]);
			}
		}

		public void Reset(int size)
		{
			Clear();
			if (size == 0)
			{
				size = 1;
			}
			InitArrays(size);
		}

		private void Init(int capacity, IEqualityComparer<TKey> hcp)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException("capacity");
			}
			this.hcp = ((hcp == null) ? EqualityComparer<TKey>.Default : hcp);
			capacity = (int)((float)capacity / 0.9f) + 1;
			InitArrays(capacity);
			generation = 0;
		}

		private void InitArrays(int size)
		{
			table = new int[size];
			linkSlots = new Link[size];
			emptySlot = -1;
			keySlots = new TKey[size];
			valueSlots = new TValue[size];
			touchedSlots = 0;
			threshold = (int)((float)table.Length * 0.9f);
			if (threshold == 0 && table.Length > 0)
			{
				threshold = 1;
			}
		}

		private void CopyToCheck(Array array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (index > array.Length)
			{
				throw new ArgumentException("index larger than largest valid index of array");
			}
			if (array.Length - index < Count)
			{
				throw new ArgumentException("Destination array cannot hold the requested elements!");
			}
		}

		private void CopyKeys(TKey[] array, int index)
		{
			for (int i = 0; i < touchedSlots; i++)
			{
				if (((uint)linkSlots[i].HashCode & 0x80000000u) != 0)
				{
					array[index++] = keySlots[i];
				}
			}
		}

		private void CopyValues(TValue[] array, int index)
		{
			for (int i = 0; i < touchedSlots; i++)
			{
				if (((uint)linkSlots[i].HashCode & 0x80000000u) != 0)
				{
					array[index++] = valueSlots[i];
				}
			}
		}

		private static KeyValuePair<TKey, TValue> make_pair(TKey key, TValue value)
		{
			return new KeyValuePair<TKey, TValue>(key, value);
		}

		private static TKey pick_key(TKey key, TValue value)
		{
			return key;
		}

		private static TValue pick_value(TKey key, TValue value)
		{
			return value;
		}

		private void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
		{
			CopyToCheck(array, index);
			for (int i = 0; i < touchedSlots; i++)
			{
				if (((uint)linkSlots[i].HashCode & 0x80000000u) != 0)
				{
					ref KeyValuePair<TKey, TValue> reference = ref array[index++];
					reference = new KeyValuePair<TKey, TValue>(keySlots[i], valueSlots[i]);
				}
			}
		}

		private void Do_ICollectionCopyTo<TRet>(Array array, int index, Transform<TRet> transform)
		{
			Type typeFromHandle = typeof(TRet);
			Type elementType = array.GetType().GetElementType();
			try
			{
				if ((typeFromHandle.IsPrimitive || elementType.IsPrimitive) && !elementType.IsAssignableFrom(typeFromHandle))
				{
					throw new Exception();
				}
				object[] array2 = (object[])array;
				for (int i = 0; i < touchedSlots; i++)
				{
					if (((uint)linkSlots[i].HashCode & 0x80000000u) != 0)
					{
						array2[index++] = transform(keySlots[i], valueSlots[i]);
					}
				}
			}
			catch (Exception innerException)
			{
				throw new ArgumentException("Cannot copy source collection elements to destination array", "array", innerException);
			}
		}

		private void Resize()
		{
			int num = HashPrimeNumbers.ToPrime((table.Length << 1) | 1);
			int[] array = new int[num];
			Link[] array2 = new Link[num];
			for (int i = 0; i < table.Length; i++)
			{
				for (int num2 = table[i] - 1; num2 != -1; num2 = linkSlots[num2].Next)
				{
					int num3 = ((array2[num2].HashCode = hcp.GetHashCode(keySlots[num2]) | int.MinValue) & 0x7FFFFFFF) % num;
					array2[num2].Next = array[num3] - 1;
					array[num3] = num2 + 1;
				}
			}
			table = array;
			linkSlots = array2;
			TKey[] destinationArray = new TKey[num];
			TValue[] destinationArray2 = new TValue[num];
			Array.Copy(keySlots, 0, destinationArray, 0, touchedSlots);
			Array.Copy(valueSlots, 0, destinationArray2, 0, touchedSlots);
			keySlots = destinationArray;
			valueSlots = destinationArray2;
			threshold = (int)((float)num * 0.9f);
		}

		public void Add(TKey key, TValue value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			int num = hcp.GetHashCode(key) | int.MinValue;
			int num2 = (num & 0x7FFFFFFF) % table.Length;
			int num3;
			for (num3 = table[num2] - 1; num3 != -1; num3 = linkSlots[num3].Next)
			{
				if (linkSlots[num3].HashCode == num && hcp.Equals(keySlots[num3], key))
				{
					throw new ArgumentException("An element with the same key already exists in the mapContainer.");
				}
			}
			if (++count > threshold)
			{
				Resize();
				num2 = (num & 0x7FFFFFFF) % table.Length;
			}
			num3 = emptySlot;
			if (num3 == -1)
			{
				num3 = touchedSlots++;
			}
			else
			{
				emptySlot = linkSlots[num3].Next;
			}
			linkSlots[num3].HashCode = num;
			linkSlots[num3].Next = table[num2] - 1;
			table[num2] = num3 + 1;
			keySlots[num3] = key;
			valueSlots[num3] = value;
			generation++;
		}

		public void Clear()
		{
			count = 0;
			Array.Clear(table, 0, table.Length);
			Array.Clear(keySlots, 0, keySlots.Length);
			Array.Clear(valueSlots, 0, valueSlots.Length);
			Array.Clear(linkSlots, 0, linkSlots.Length);
			emptySlot = -1;
			touchedSlots = 0;
			generation++;
		}

		public bool ContainsKey(TKey key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			int num = hcp.GetHashCode(key) | int.MinValue;
			for (int num2 = table[(num & 0x7FFFFFFF) % table.Length] - 1; num2 != -1; num2 = linkSlots[num2].Next)
			{
				if (linkSlots[num2].HashCode == num && hcp.Equals(keySlots[num2], key))
				{
					return true;
				}
			}
			return false;
		}

		public bool ContainsValue(TValue value)
		{
			IEqualityComparer<TValue> @default = EqualityComparer<TValue>.Default;
			for (int i = 0; i < table.Length; i++)
			{
				for (int num = table[i] - 1; num != -1; num = linkSlots[num].Next)
				{
					if (@default.Equals(valueSlots[num], value))
					{
						return true;
					}
				}
			}
			return false;
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("Version", generation);
			info.AddValue("Comparer", hcp);
			KeyValuePair<TKey, TValue>[] array = new KeyValuePair<TKey, TValue>[count];
			if (count > 0)
			{
				CopyTo(array, 0);
			}
			info.AddValue("HashSize", table.Length);
			info.AddValue("KeyValuePairs", array);
		}

		public virtual void OnDeserialization(object sender)
		{
			if (serialization_info == null)
			{
				return;
			}
			int num = 0;
			KeyValuePair<TKey, TValue>[] array = null;
			SerializationInfoEnumerator enumerator = serialization_info.GetEnumerator();
			while (enumerator.MoveNext())
			{
				switch (enumerator.Name)
				{
				case "Version":
					generation = (int)enumerator.Value;
					break;
				case "Comparer":
					hcp = (IEqualityComparer<TKey>)enumerator.Value;
					break;
				case "HashSize":
					num = (int)enumerator.Value;
					break;
				case "KeyValuePairs":
					array = (KeyValuePair<TKey, TValue>[])enumerator.Value;
					break;
				}
			}
			if (hcp == null)
			{
				hcp = EqualityComparer<TKey>.Default;
			}
			if (num < 10)
			{
				num = 10;
			}
			InitArrays(num);
			count = 0;
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					Add(array[i].Key, array[i].Value);
				}
			}
			generation++;
			serialization_info = null;
		}

		public bool Remove(TKey key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			int num = hcp.GetHashCode(key) | int.MinValue;
			int num2 = (num & 0x7FFFFFFF) % table.Length;
			int num3 = table[num2] - 1;
			if (num3 == -1)
			{
				return false;
			}
			int num4 = -1;
			while (linkSlots[num3].HashCode != num || !hcp.Equals(keySlots[num3], key))
			{
				num4 = num3;
				num3 = linkSlots[num3].Next;
				if (num3 == -1)
				{
					break;
				}
			}
			if (num3 == -1)
			{
				return false;
			}
			count--;
			if (num4 == -1)
			{
				table[num2] = linkSlots[num3].Next + 1;
			}
			else
			{
				linkSlots[num4].Next = linkSlots[num3].Next;
			}
			linkSlots[num3].Next = emptySlot;
			emptySlot = num3;
			linkSlots[num3].HashCode = 0;
			keySlots[num3] = default(TKey);
			valueSlots[num3] = default(TValue);
			generation++;
			return true;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			int num = hcp.GetHashCode(key) | int.MinValue;
			for (int num2 = table[(num & 0x7FFFFFFF) % table.Length] - 1; num2 != -1; num2 = linkSlots[num2].Next)
			{
				if (linkSlots[num2].HashCode == num && hcp.Equals(keySlots[num2], key))
				{
					value = valueSlots[num2];
					return true;
				}
			}
			value = default(TValue);
			return false;
		}

		private static TKey ToTKey(object key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (!(key is TKey))
			{
				throw new ArgumentException("not of type: " + typeof(TKey).ToString(), "key");
			}
			return (TKey)key;
		}

		private static TValue ToTValue(object value)
		{
			if (value == null && !typeof(TValue).IsValueType)
			{
				return default(TValue);
			}
			if (!(value is TValue))
			{
				throw new ArgumentException("not of type: " + typeof(TValue).ToString(), "value");
			}
			return (TValue)value;
		}

		void IDictionary.Add(object key, object value)
		{
			Add(ToTKey(key), ToTValue(value));
		}

		bool IDictionary.Contains(object key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (key is TKey)
			{
				return ContainsKey((TKey)key);
			}
			return false;
		}

		void IDictionary.Remove(object key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (key is TKey)
			{
				Remove((TKey)key);
			}
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> keyValuePair)
		{
			Add(keyValuePair.Key, keyValuePair.Value);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> keyValuePair)
		{
			return ContainsKeyValuePair(keyValuePair);
		}

		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
		{
			CopyTo(array, index);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> keyValuePair)
		{
			if (!ContainsKeyValuePair(keyValuePair))
			{
				return false;
			}
			return Remove(keyValuePair.Key);
		}

		private bool ContainsKeyValuePair(KeyValuePair<TKey, TValue> pair)
		{
			if (!TryGetValue(pair.Key, out var value))
			{
				return false;
			}
			return EqualityComparer<TValue>.Default.Equals(pair.Value, value);
		}

		void ICollection.CopyTo(Array array, int index)
		{
			KeyValuePair<TKey, TValue>[] array2 = array as KeyValuePair<TKey, TValue>[];
			if (array2 != null)
			{
				CopyTo(array2, index);
				return;
			}
			CopyToCheck(array, index);
			DictionaryEntry[] array3 = array as DictionaryEntry[];
			if (array3 != null)
			{
				for (int i = 0; i < touchedSlots; i++)
				{
					if (((uint)linkSlots[i].HashCode & 0x80000000u) != 0)
					{
						ref DictionaryEntry reference = ref array3[index++];
						reference = new DictionaryEntry(keySlots[i], valueSlots[i]);
					}
				}
			}
			else
			{
				Do_ICollectionCopyTo(array, index, make_pair);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Enumerator(this);
		}

		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
		{
			return new Enumerator(this);
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return new ShimEnumerator(this);
		}

		public Enumerator GetEnumerator()
		{
			return new Enumerator(this);
		}

		public void Begin()
		{
			iteratorIndex = -1;
			iteratorState = true;
		}

		public bool Next()
		{
			if (iteratorIndex >= touchedSlots)
			{
				iteratorIndex = -1;
				iteratorState = false;
				return false;
			}
			while (++iteratorIndex < touchedSlots)
			{
				if (((uint)linkSlots[iteratorIndex].HashCode & 0x80000000u) != 0)
				{
					return true;
				}
			}
			iteratorIndex = -1;
			iteratorState = false;
			return false;
		}
	}
}
