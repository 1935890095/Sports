namespace XFX.Core.Util
{
	public class PListNode
	{
		public PListNode next;

		public PListNode prev;
	}
	public class PListNode<T> where T : PListNode<T>
	{
		internal PListNode<T> next;

		internal PListNode<T> prev;

		public static implicit operator T(PListNode<T> n)
		{
			return n as T;
		}
	}
}
