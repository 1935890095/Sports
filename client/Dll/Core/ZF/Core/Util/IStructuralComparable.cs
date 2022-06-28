using System.Collections;

namespace ZF.Core.Util
{
	public interface IStructuralComparable
	{
		int CompareTo(object other, IComparer comparer);
	}
}
