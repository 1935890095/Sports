namespace XFX.Core.Util
{
	internal class TimerEvent
	{
		private int socket_index_;

		private int socket_size_;

		private PList[] list_;

		public TimerEvent(int socket_size)
		{
			socket_size_ = socket_size;
			socket_index_ = 0;
			list_ = new PList[socket_size_];
			for (int i = 0; i < socket_size_; i++)
			{
				list_[i] = new PList();
			}
		}

		public TimerEventObject Add(int start, int interval, TimerEventObject.TimerProc proc, object o, int p1, int p2)
		{
			TimerEventObject timerEventObject = new TimerEventObject();
			timerEventObject.proc = proc;
			if (interval <= 0)
			{
				interval = 1;
			}
			timerEventObject.interval = interval;
			timerEventObject.obj = o;
			timerEventObject.p1 = p1;
			timerEventObject.p2 = p2;
			timerEventObject.enable = 1;
			timerEventObject.circle = (short)(start / socket_size_);
			int num = start % socket_size_ + socket_index_;
			if (num >= socket_size_)
			{
				num -= socket_size_;
			}
			list_[num].AddTail(timerEventObject);
			return timerEventObject;
		}

		public void Remove(TimerEventObject obj)
		{
			if (obj != null)
			{
				obj.enable = 0;
			}
		}

		public void Clear()
		{
			for (int i = 0; i < socket_size_; i++)
			{
				PList pList = list_[i];
				for (PListNode next = pList.next; next != pList; next = next.next)
				{
					TimerEventObject timerEventObject = (TimerEventObject)next;
					timerEventObject.enable = 0;
					timerEventObject.proc = null;
					timerEventObject.obj = null;
				}
				pList.Init();
			}
		}

		public void Process()
		{
			PList pList = list_[socket_index_];
			int num = socket_index_;
			socket_index_++;
			if (socket_index_ >= socket_size_)
			{
				socket_index_ = 0;
			}
			PListNode pListNode = null;
			PListNode pListNode2 = null;
			pListNode = pList.next;
			pListNode2 = pListNode.next;
			while (pListNode != pList)
			{
				TimerEventObject timerEventObject = (TimerEventObject)pListNode;
				if (timerEventObject.circle <= 0)
				{
					pList.Remove(pListNode);
					if (timerEventObject.enable != 0 && timerEventObject.proc(timerEventObject.obj, timerEventObject.p1, timerEventObject.p2))
					{
						timerEventObject.circle = (short)(timerEventObject.interval / socket_size_);
						int num2 = timerEventObject.interval % socket_size_ + num;
						if (num2 == num)
						{
							timerEventObject.circle--;
						}
						if (num2 >= socket_size_)
						{
							num2 -= socket_size_;
						}
						list_[num2].Add(pListNode);
					}
					else
					{
						timerEventObject.proc = null;
						timerEventObject.obj = null;
					}
				}
				else
				{
					timerEventObject.circle--;
				}
				pListNode = pListNode2;
				pListNode2 = pListNode.next;
			}
		}
	}
}
