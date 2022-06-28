using System.Collections.Generic;
using System.Diagnostics;

namespace ZF.Core.Util
{
	internal sealed class CollectionDebuggerView<T, U>
	{
		private readonly ICollection<KeyValuePair<T, U>> c;

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public KeyValuePair<T, U>[] Items
		{
			get
			{
				KeyValuePair<T, U>[] array = new KeyValuePair<T, U>[c.Count];
				c.CopyTo(array, 0);
				return array;
			}
		}

		public CollectionDebuggerView(ICollection<KeyValuePair<T, U>> col)
		{
			c = col;
		}
	}
}
