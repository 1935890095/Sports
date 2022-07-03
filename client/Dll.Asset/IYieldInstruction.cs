using System.Collections;

namespace XFX.Asset
{
	public interface IYieldInstruction : IEnumerator
	{
		bool keepWaiting { get; }
	}
}
