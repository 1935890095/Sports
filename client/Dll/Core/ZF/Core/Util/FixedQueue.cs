using System.Collections.Generic;
using System.Threading;

namespace ZF.Core.Util
{
	public class FixedQueue<T>
	{
		private T[] queue;

		private readonly int capacity;

		private volatile uint write;

		private volatile uint read;

		public int Count => (int)(write - read);

		public FixedQueue(int capacity)
		{
			this.capacity = capacity;
			queue = new T[capacity];
			write = 0u;
			read = 0u;
		}

		public IEnumerable<T> GetEnumerator()
		{
			uint write_count = write;
			uint read_count = read;
			for (uint i = read_count; i < write_count; i++)
			{
				int offset = (int)(i % (uint)capacity);
				yield return queue[offset];
			}
		}

		public bool Enqueue(T obj)
		{
			uint num = write;
			uint num2 = read;
			int num3 = (int)(num - num2);
			if (num3 >= capacity)
			{
				return false;
			}
			int num4 = (int)(num % (uint)capacity);
			queue[num4] = obj;
			Thread.MemoryBarrier();
			write++;
			return true;
		}

		public T Dequeue()
		{
			uint num = write;
			uint num2 = read;
			if (num <= num2)
			{
				return default(T);
			}
			int num3 = (int)(num2 % (uint)capacity);
			T result = queue[num3];
			queue[num3] = default(T);
			Thread.MemoryBarrier();
			read++;
			return result;
		}

		public T Peek()
		{
			uint num = write;
			uint num2 = read;
			if (num <= num2)
			{
				return default(T);
			}
			int num3 = (int)(num2 % (uint)capacity);
			return queue[num3];
		}

		public void Clear()
		{
			for (int i = 0; i < queue.Length; i++)
			{
				queue[i] = default(T);
			}
			write = (read = 0u);
		}
	}
}
