using System.Collections;

namespace ZF.Asset
{
	public interface IAyncOperation : IYieldInstruction, IEnumerator
	{
		bool isDone { get; }

		float progress { get; }
	}
}
