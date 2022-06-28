namespace ZF.Core.Util
{
	internal class TimerEventObject : PListNode
	{
		public delegate bool TimerProc(object obj, int p1, int p2);

		public TimerProc proc;

		public object obj;

		public int p1;

		public int p2;

		public int interval;

		public short circle;

		public short enable;
	}
}
