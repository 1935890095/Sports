using System.Collections;

namespace XFX.Asset
{
	public interface IPreload : IEnumerator
	{
		bool isDone { get; }

		float progress { get; }
	}
}
