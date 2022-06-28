namespace ZF.Core.Scene
{
	public interface ISceneOption
	{
		bool synchronize { get; }

		bool denySceneActivation { get; }

		bool builtin { get; }

		bool additive { get; }

		void OnInvoke(IScene scene, bool load, bool done, float progress);
	}
}
