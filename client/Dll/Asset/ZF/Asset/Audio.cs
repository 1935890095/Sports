using UnityEngine;
using ZF.Core.Render;

namespace ZF.Asset
{
	public class Audio : RenderObject, IAudio, IRenderObject
	{
		internal enum Status
		{
			None,
			Playing,
			Paused,
			Stopped
		}

		private bool loop_;

		private float volume_ = 1f;

		private float distance_ = 100f;

		private float duration_;

		private bool playOnAwake_;

		private AudioSource source;

		private Status status;

		public bool loop
		{
			get
			{
				return loop_;
			}
			set
			{
				loop_ = value;
				if ((Object)(object)source != (Object)null)
				{
					source.loop = (value);
				}
			}
		}

		public float volume
		{
			get
			{
				return volume_;
			}
			set
			{
				volume_ = value;
				if ((Object)(object)source != (Object)null)
				{
					source.volume = (value);
				}
			}
		}

		public float distance
		{
			get
			{
				return distance_;
			}
			set
			{
				distance_ = value;
				if ((Object)(object)source != (Object)null)
				{
					source.maxDistance = (value);
				}
			}
		}

		public float duration
		{
			get
			{
				return duration_;
			}
			set
			{
				duration_ = value;
				life = value;
			}
		}

		public bool playOnAwake
		{
			get
			{
				return playOnAwake_;
			}
			set
			{
				playOnAwake_ = value;
				if ((Object)(object)source != (Object)null)
				{
					source.playOnAwake = (value);
				}
			}
		}

		public float life { get; private set; }

		public bool dontDestroyOnStop { get; set; }

		public bool isPlaying => status == Status.Playing;

		protected override void OnCreate(IRenderResource resource)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Expected O, but got Unknown
			if (!(resource.asset is AudioClip))
			{
				Destroy();
				return;
			}
			base.gameObject = new GameObject("audio");
			source = base.gameObject.AddComponent<AudioSource>();
			_003F val = source;
			Object asset = resource.asset;
			((AudioSource)val).clip((AudioClip)(object)((asset is AudioClip) ? asset : null));
			source.loop = (loop);
			source.dopplerLevel = (0f);
			source.maxDistance = (distance);
			source.rolloffMode = ((AudioRolloffMode)1);
			source.volume = (volume);
			source.playOnAwake = (playOnAwake);
			if (duration == 0f)
			{
				duration = source.clip.length;
			}
			if (status == Status.Playing)
			{
				Play();
			}
		}

		public void Play()
		{
			if ((Object)(object)source != (Object)null)
			{
				source.Play();
			}
			status = Status.Playing;
			if (duration > 0f)
			{
				life = duration;
			}
		}

		public void Pause()
		{
			if (isPlaying)
			{
				if ((Object)(object)source != (Object)null)
				{
					source.Pause();
				}
				status = Status.Paused;
			}
		}

		public void Continue()
		{
			if (status == Status.Paused)
			{
				if ((Object)(object)source != (Object)null)
				{
					source.UnPause();
				}
				status = Status.Playing;
			}
		}

		public void Stop()
		{
			if ((Object)(object)source != (Object)null)
			{
				source.Stop();
			}
			status = Status.Stopped;
			if (!dontDestroyOnStop)
			{
				Destroy();
			}
		}

		public void Loop()
		{
			if (!isPlaying || !(duration > 0f))
			{
				return;
			}
			life -= Time.deltaTime;
			if (life <= 0f && status != Status.Stopped)
			{
				if (loop)
				{
					life = duration;
				}
				else
				{
					Stop();
				}
			}
		}

		protected override void OnUpdate()
		{
			Loop();
		}

		protected override void OnDestroy()
		{
			if (base.gameObject)
			{
				Object.Destroy((Object)(object)base.gameObject);
				base.gameObject = null;
			}
		}
	}
}
