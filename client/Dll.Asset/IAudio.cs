using XFX.Core.Render;

namespace XFX.Asset
{
	public interface IAudio : IRenderObject
	{
		float duration { get; set; }

		float life { get; }

		float volume { get; set; }

		bool loop { get; set; }

		bool dontDestroyOnStop { get; set; }

		bool isPlaying { get; }

		void Play();

		void Pause();

		void Continue();

		void Stop();

		void Loop();
	}
}
