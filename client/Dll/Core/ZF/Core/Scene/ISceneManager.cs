using ZF.Core.Logging;

namespace ZF.Core.Scene
{
	public interface ISceneManager
	{
		string scene_dir { get; set; }

		string scene_ext { get; set; }

		ILogger logger { get; set; }

		IScene LoadScene(string name, ISceneOption option);

		void UnloadScene(string name);

		void UnloadScene(IScene scene);

		void UnloadAllScene();

		void Loop();
	}
}
