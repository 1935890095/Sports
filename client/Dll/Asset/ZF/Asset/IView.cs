namespace ZF.Asset
{
	public interface IView
	{
		bool isShow { get; }

		void Show();

		void Hide();

		void Destroy();
	}
}
