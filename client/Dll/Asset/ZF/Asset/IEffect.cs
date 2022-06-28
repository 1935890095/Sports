using ZF.Core.Render;

namespace ZF.Asset
{
	public interface IEffect : IRenderObject
	{
		float duration { get; set; }

		float life { get; }

		float speed { get; set; }

		bool billbord { get; set; }

		bool dontDestroyOnStop { get; set; }

		bool isPlaying { get; }

		void Play();

		void Pause();

		void Continue();

		void Sample(float time);

		void Stop();
	}
}
