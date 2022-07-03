using XFX.Core.Render;

namespace XFX.Asset
{
	public interface IAction : IRenderObject
	{
		float duration { get; }

		float life { get; }

		float speed { get; set; }

		bool isPlaying { get; }

		void Play(IActionContext context = null);

		void Pause();

		void Continue();

		void Stop();
	}
}
