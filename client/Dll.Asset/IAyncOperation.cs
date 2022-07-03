using System.Collections;

namespace XFX.Asset
{
	public interface IAyncOperation : IYieldInstruction, IEnumerator
	{
		bool isDone { get; }

		float progress { get; }
	}
}
