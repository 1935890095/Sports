using ZF.Core.Render;

namespace ZF.Asset
{
	public interface IModel : IRenderObject
	{
		int mask { get; }

		void Attach(ISkeleton skeleton, string bindBoneName);

		void Detach(ISkeleton skeleton);
	}
}
