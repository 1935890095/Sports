namespace ZF.Core.Util
{
	public class PList : PListNode
	{
		public int Count;

		public PList()
		{
			Init();
		}

		public bool Empty()
		{
			return prev == this;
		}

		public void Init()
		{
			next = this;
			prev = this;
			Count = 0;
		}

		public void Add(PListNode node)
		{
			next.prev = node;
			node.next = next;
			node.prev = this;
			next = node;
			Count++;
		}

		public void AddTail(PListNode node)
		{
			PListNode pListNode = prev;
			prev = node;
			node.next = this;
			node.prev = pListNode;
			pListNode.next = node;
			Count++;
		}

		public void Remove(PListNode node)
		{
			node.prev.next = node.next;
			node.next.prev = node.prev;
			node.prev = null;
			node.next = null;
			Count--;
		}

		public PListNode Pop()
		{
			if (prev == this)
			{
				return null;
			}
			PListNode pListNode = next;
			pListNode.prev.next = pListNode.next;
			pListNode.next.prev = pListNode.prev;
			pListNode.prev = null;
			pListNode.next = null;
			Count--;
			return pListNode;
		}

		public void AddListTail(PList list)
		{
			if (list.next != list)
			{
				PListNode pListNode = list.next;
				PListNode pListNode2 = list.prev;
				PListNode pListNode3 = prev;
				pListNode3.next = pListNode;
				pListNode.prev = pListNode3;
				prev = pListNode2;
				pListNode2.next = this;
				Count += list.Count;
				list.Init();
			}
		}
	}
	public class PList<T> : PListNode<T> where T : PListNode<T>
	{
		public int Count;

		public PList()
		{
			Init();
		}

		public bool Empty()
		{
			return prev == this;
		}

		public void Init()
		{
			next = this;
			prev = this;
			Count = 0;
		}

		public void Clear()
		{
			for (T val = Pop(); val != null; val = Pop())
			{
			}
			Init();
		}

		public void Add(T node)
		{
			next.prev = node;
			node.next = next;
			node.prev = this;
			next = node;
			Count++;
		}

		public void AddTail(T node)
		{
			PListNode<T> pListNode = prev;
			prev = node;
			node.next = this;
			node.prev = pListNode;
			pListNode.next = node;
			Count++;
		}

		public void Remove(T node)
		{
			node.prev.next = node.next;
			node.next.prev = node.prev;
			node.prev = null;
			node.next = null;
			Count--;
		}

		public T Pop()
		{
			PListNode<T> pListNode = next;
			if (pListNode == this)
			{
				return (T)null;
			}
			pListNode.prev.next = pListNode.next;
			pListNode.next.prev = pListNode.prev;
			pListNode.prev = null;
			pListNode.next = null;
			Count--;
			return (T)pListNode;
		}

		public T PopTail()
		{
			PListNode<T> pListNode = prev;
			if (pListNode == this)
			{
				return (T)null;
			}
			pListNode.prev.next = pListNode.next;
			pListNode.next.prev = pListNode.prev;
			pListNode.prev = null;
			pListNode.next = null;
			Count--;
			return (T)pListNode;
		}

		public void AddListTail(PList<T> list)
		{
			if (list.next != list)
			{
				PListNode<T> pListNode = list.next;
				PListNode<T> pListNode2 = list.prev;
				PListNode<T> pListNode3 = prev;
				pListNode3.next = pListNode;
				pListNode.prev = pListNode3;
				prev = pListNode2;
				pListNode2.next = this;
				Count += list.Count;
				list.Init();
			}
		}

		public T Next(T node)
		{
			if (node == null)
			{
				if (next == this)
				{
					return (T)null;
				}
				return (T)next;
			}
			if (node.next == this)
			{
				return (T)null;
			}
			return (T)node.next;
		}
	}
}
