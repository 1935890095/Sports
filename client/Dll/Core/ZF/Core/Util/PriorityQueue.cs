using System;
using System.Collections.Generic;
using System.Threading;

namespace ZF.Core.Util
{
	public class PriorityQueue<T> where T : IComparable<T>
	{
		private struct IndexedItem : IComparable<IndexedItem>
		{
			public T Value;

			public long Id;

			public int CompareTo(IndexedItem other)
			{
				int num = Value.CompareTo(other.Value);
				if (num == 0)
				{
					num = Id.CompareTo(other.Id);
				}
				return num;
			}
		}

		private static long count = long.MinValue;

		private IndexedItem[] items;

		private int size;

		public int Count => size;

		public PriorityQueue()
			: this(16)
		{
		}

		public PriorityQueue(int capacity)
		{
			items = new IndexedItem[capacity];
			size = 0;
		}

		private bool IsHigherPriority(int left, int right)
		{
			return items[left].CompareTo(items[right]) < 0;
		}

		private void Percolate(int index)
		{
			if (index < size && index >= 0)
			{
				int num = (index - 1) / 2;
				if (num >= 0 && num != index && IsHigherPriority(index, num))
				{
					IndexedItem indexedItem = items[index];
					ref IndexedItem reference = ref items[index];
					reference = items[num];
					items[num] = indexedItem;
					Percolate(num);
				}
			}
		}

		private void Heapify()
		{
			Heapify(0);
		}

		private void Heapify(int index)
		{
			if (index < size && index >= 0)
			{
				int num = 2 * index + 1;
				int num2 = 2 * index + 2;
				int num3 = index;
				if (num < size && IsHigherPriority(num, num3))
				{
					num3 = num;
				}
				if (num2 < size && IsHigherPriority(num2, num3))
				{
					num3 = num2;
				}
				if (num3 != index)
				{
					IndexedItem indexedItem = items[index];
					ref IndexedItem reference = ref items[index];
					reference = items[num3];
					items[num3] = indexedItem;
					Heapify(num3);
				}
			}
		}

		public T Peek()
		{
			if (size == 0)
			{
				throw new InvalidOperationException("HEAP is Empty");
			}
			return items[0].Value;
		}

		private void RemoveAt(int index)
		{
			ref IndexedItem reference = ref items[index];
			reference = items[--size];
			items[size] = default(IndexedItem);
			Heapify();
			if (size < items.Length / 4)
			{
				IndexedItem[] sourceArray = items;
				items = new IndexedItem[items.Length / 2];
				Array.Copy(sourceArray, 0, items, 0, size);
			}
		}

		public T Dequeue()
		{
			T result = Peek();
			RemoveAt(0);
			return result;
		}

		public void Enqueue(T item)
		{
			if (size >= items.Length)
			{
				IndexedItem[] array = items;
				items = new IndexedItem[items.Length * 2];
				Array.Copy(array, items, array.Length);
			}
			int num = size++;
			ref IndexedItem reference = ref items[num];
			reference = new IndexedItem
			{
				Value = item,
				Id = Interlocked.Increment(ref count)
			};
			Percolate(num);
		}

		public bool Remove(T item)
		{
			for (int i = 0; i < size; i++)
			{
				if (EqualityComparer<T>.Default.Equals(items[i].Value, item))
				{
					RemoveAt(i);
					return true;
				}
			}
			return false;
		}
	}
}
