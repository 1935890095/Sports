using System.Collections;

namespace XFX.Core.Util
{
	public interface IStructuralEquatable
	{
		bool Equals(object other, IEqualityComparer comparer);

		int GetHashCode(IEqualityComparer comparer);
	}
}
