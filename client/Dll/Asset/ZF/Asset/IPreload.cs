using System.Collections;

namespace ZF.Asset
{
	public interface IPreload : IEnumerator
	{
		bool isDone { get; }

		float progress { get; }
	}
}
