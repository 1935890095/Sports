using System.Collections;

namespace XFX.Core.Util
{
	public interface IStructuralComparable
	{
		int CompareTo(object other, IComparer comparer);
	}
}
