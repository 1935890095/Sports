namespace ZF.Core.Render
{
	public interface IRenderComponent
	{
		IRenderObject renderObject { get; }

		bool enabled { get; set; }

		void Create(IRenderObject renderObject);

		void Destroy();

		void Update();

		void LateUpdate();

		void Command(string cmd, params object[] args);
	}
}
