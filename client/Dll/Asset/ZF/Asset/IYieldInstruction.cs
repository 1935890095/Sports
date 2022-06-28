using System.Collections;

namespace ZF.Asset
{
	public interface IYieldInstruction : IEnumerator
	{
		bool keepWaiting { get; }
	}
}
