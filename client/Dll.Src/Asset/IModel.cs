using XFX.Core.Render;

namespace XFX.Asset
{
	public interface IModel : IRenderObject
	{
		int mask { get; }

		void Attach(ISkeleton skeleton, string bindBoneName);

		void Detach(ISkeleton skeleton);
	}
}
