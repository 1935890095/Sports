using System.Collections;

namespace XFX.Core.Scene
{
	public interface IScene
	{
		string name { get; }

		ISceneOption option { get; set; }

		SceneState state { get; set; }

		IEnumerator Load(string dir, string ext);

		IEnumerator Unload();
	}
}
