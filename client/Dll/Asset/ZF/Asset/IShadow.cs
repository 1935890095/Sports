using ZF.Core.Render;

namespace ZF.Asset
{
	public interface IShadow : IRenderObject
	{
		void Show();

		void Hide();
	}
}
