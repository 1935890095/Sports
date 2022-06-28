namespace ZF.Core.Scene
{
	internal class DefaultSceneOption : ISceneOption
	{
		public bool synchronize => false;

		public bool denySceneActivation => false;

		public bool builtin => false;

		public bool additive => false;

		public void OnInvoke(IScene scene, bool load, bool done, float progress)
		{
		}
	}
}
