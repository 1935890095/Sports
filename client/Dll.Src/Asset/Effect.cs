using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XFX.Asset.Properties;
using XFX.Core.Render;
using Object = UnityEngine.Object;

namespace XFX.Asset
{
	public class Effect : Depends, IEffect, IRenderObject
	{
		internal enum Status
		{
			None,
			Playing,
			Paused,
			Stopped
		}

		private float duration_ = -1f;

		private float speed_ = 1f;

		private Status status;

		private ParticleSystem[] particle_systems;

		private Animation[] animations;

		private Animator[] animators;

		private EffectProperty property;

		private float elapse;

		private readonly Dictionary<int, float> speeds = new Dictionary<int, float>();

		public float duration
		{
			get
			{
				return duration_;
			}
			set
			{
				duration_ = value;
			}
		}

		public float speed
		{
			get
			{
				return speed_;
			}
			set
			{
				if (value <= 0f)
				{
					Debug.LogWarning((object)"effect speed require more then zero");
				}
				else
				{
					speed_ = value;
				}
			}
		}

		public float life
		{
			get
			{
				if (!isPlaying)
				{
					return 0f;
				}
				return (!(duration >= elapse)) ? 0f : (duration - elapse);
			}
		}

		public bool billbord { get; set; }

		public bool dontDestroyOnStop { get; set; }

		public bool isPlaying => status == Status.Playing;

		protected override void OnCreate(IRenderResource resource)
		{
			if (!(resource.asset is GameObject))
			{
				Destroy();
				Debug.LogError((object)("create effect error, resource is not GameObject: " + resource.name));
				return;
			}
			Object obj = Object.Instantiate(resource.asset);
			base.gameObject = (GameObject)(object)((obj is GameObject) ? obj : null);
			particle_systems = base.gameObject.GetComponentsInChildren<ParticleSystem>();
			animations = base.gameObject.GetComponentsInChildren<Animation>();
			animators = base.gameObject.GetComponentsInChildren<Animator>();
			property = base.gameObject.GetComponent<EffectProperty>();
			if ((Object)(object)property != (Object)null)
			{
				if (duration == -1f)
				{
					duration = property.duration;
				}
				base.renderers = property.renderers;
				if (property.scale != 1f && base.scale == Vector3.one)
				{
					float num = property.scale;
					base.scale = new Vector3(num, num, num);
				}
			}
			else
			{
				duration = 0f;
			}
			base.active = false;
			ParticleSystem[] array = particle_systems;
			foreach (ParticleSystem val in array)
			{
				Dictionary<int, float> dictionary = speeds;
				ParticleSystem.MainModule main = val.main;
				int hashCode = ((object)(ParticleSystem.MainModule)main).GetHashCode();
				ParticleSystem.MainModule main2 = val.main;
				dictionary[hashCode] = main2.simulationSpeed;
			}
			Animation[] array2 = animations;
			foreach (Animation val2 in array2)
			{
				if (!((Object)(object)val2 != (Object)null))
				{
					continue;
				}
				IEnumerator enumerator = val2.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						AnimationState val3 = (AnimationState)enumerator.Current;
						speeds[((object)val3).GetHashCode()] = val3.speed;
					}
				}
				finally
				{
					IDisposable disposable;
					if ((disposable = enumerator as IDisposable) != null)
					{
						disposable.Dispose();
					}
				}
			}
			Animator[] array3 = animators;
			foreach (Animator val4 in array3)
			{
				if ((Object)(object)val4 != (Object)null)
				{
					speeds[((object)val4).GetHashCode()] = val4.speed;
				}
			}
			if (status == Status.Playing)
			{
				Play();
			}
		}

		protected override void OnDestroy()
		{
			if ((Object)(object)base.gameObject != (Object)null)
			{
				RenderObject.DestroyObject(base.gameObject);
				base.gameObject = null;
			}
			particle_systems = null;
			animations = null;
			animators = null;
		}

		public void Play()
		{
			if (!base.visible)
			{
				return;
			}
			base.active = true;
			elapse = 0f;
			if (particle_systems != null)
			{
				for (int i = 0; i < particle_systems.Length; i++)
				{
					ParticleSystem val = particle_systems[i];
					if (!((Object)(object)val == (Object)null))
					{
						ParticleSystem.MainModule main = val.main;
						float num = speeds[main.GetHashCode()];
						main.simulationSpeed = (num * speed_);
						val.time = (0f);
						val.Play();
					}
				}
			}
			if (animations != null)
			{
				for (int j = 0; j < animations.Length; j++)
				{
					Animation val2 = animations[j];
					if ((Object)(object)val2 == (Object)null)
					{
						continue;
					}
					IEnumerator enumerator = val2.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							AnimationState val3 = (AnimationState)enumerator.Current;
							float num2 = speeds[((object)val3).GetHashCode()];
							val3.speed = (num2 * speed_);
						}
					}
					finally
					{
						IDisposable disposable;
						if ((disposable = enumerator as IDisposable) != null)
						{
							disposable.Dispose();
						}
					}
					val2.Stop();
					val2.Play();
				}
			}
			if (animators != null)
			{
				for (int k = 0; k < animators.Length; k++)
				{
					Animator val4 = animators[k];
					if (!((Object)(object)val4 == (Object)null) && !((Object)(object)val4.runtimeAnimatorController == (Object)null))
					{
						float num3 = speeds[((object)val4).GetHashCode()];
						val4.speed = (num3 * speed_);
						AnimatorStateInfo currentAnimatorStateInfo = val4.GetCurrentAnimatorStateInfo(0);
						val4.Play(currentAnimatorStateInfo.shortNameHash, 0, 0f);
					}
				}
			}
			status = Status.Playing;
		}

		public void Pause()
		{
			if (status != Status.Playing)
			{
				return;
			}
			if (particle_systems != null)
			{
				for (int i = 0; i < particle_systems.Length; i++)
				{
					ParticleSystem val = particle_systems[i];
					if (!((Object)(object)val == (Object)null))
					{
						ParticleSystem.MainModule main = val.main;
						 main.simulationSpeed = 0f;
					}
				}
			}
			if (animations != null)
			{
				for (int j = 0; j < animations.Length; j++)
				{
					Animation val2 = animations[j];
					if ((Object)(object)val2 == (Object)null)
					{
						continue;
					}
					IEnumerator enumerator = val2.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							AnimationState val3 = (AnimationState)enumerator.Current;
							val3.speed = (0f);
						}
					}
					finally
					{
						IDisposable disposable;
						if ((disposable = enumerator as IDisposable) != null)
						{
							disposable.Dispose();
						}
					}
				}
			}
			if (animators != null)
			{
				for (int k = 0; k < animators.Length; k++)
				{
					Animator val4 = animators[k];
					if (!((Object)(object)val4 == (Object)null))
					{
						val4.speed = (0f);
					}
				}
			}
			status = Status.Paused;
		}

		public void Continue()
		{
			if (status != Status.Paused)
			{
				return;
			}
			if (particle_systems != null)
			{
				for (int i = 0; i < particle_systems.Length; i++)
				{
					ParticleSystem val = particle_systems[i];
					if (!((Object)(object)val == (Object)null))
					{
						ParticleSystem.MainModule main = val.main;
						float num = speeds[main.GetHashCode()];
						main.simulationSpeed = (num * speed_);
					}
				}
			}
			if (animations != null)
			{
				for (int j = 0; j < animations.Length; j++)
				{
					Animation val2 = animations[j];
					if ((Object)(object)val2 == (Object)null)
					{
						continue;
					}
					IEnumerator enumerator = val2.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							AnimationState val3 = (AnimationState)enumerator.Current;
							float num2 = speeds[((object)val3).GetHashCode()];
							val3.speed = (num2 * speed_);
						}
					}
					finally
					{
						IDisposable disposable;
						if ((disposable = enumerator as IDisposable) != null)
						{
							disposable.Dispose();
						}
					}
				}
			}
			if (animators != null)
			{
				for (int k = 0; k < animators.Length; k++)
				{
					Animator val4 = animators[k];
					if (!((Object)(object)val4 == (Object)null))
					{
						float num3 = speeds[((object)val4).GetHashCode()];
						val4.speed = (num3 * speed_);
					}
				}
			}
			status = Status.Playing;
		}

		public void Sample(float time)
		{
			if (!base.visible)
			{
				return;
			}
			if (!base.active)
			{
				base.active = true;
			}
			if (particle_systems != null)
			{
				for (int i = 0; i < particle_systems.Length; i++)
				{
					ParticleSystem val = particle_systems[i];
					if (!((Object)(object)val == (Object)null))
					{
						val.Simulate(time, true, false);
					}
				}
			}
			if (animations != null)
			{
				for (int j = 0; j < animations.Length; j++)
				{
					Animation val2 = animations[j];
					if (!((Object)(object)val2 == (Object)null))
					{
						AnimationState val3 = val2[((Object)val2.clip).name];
						val3.time = (val3.time + time);
						val2.Sample();
					}
				}
			}
			if (animators == null)
			{
				return;
			}
			for (int k = 0; k < animators.Length; k++)
			{
				Animator val4 = animators[k];
				if (!((Object)(object)val4 == (Object)null))
				{
					AnimatorClipInfo[] currentAnimatorClipInfo = val4.GetCurrentAnimatorClipInfo(0);
					for (int l = 0; l < currentAnimatorClipInfo.Length; l++)
					{
						currentAnimatorClipInfo[l].clip.SampleAnimation(((Component)val4).gameObject, elapse);
					}
				}
			}
		}

		public void Stop()
		{
			Pause();
			base.active = false;
			status = Status.Stopped;
			elapse = 0f;
			if (!dontDestroyOnStop)
			{
				Destroy();
			}
		}

		protected override void OnUpdate()
		{
			if (!base.complete || !isPlaying)
			{
				return;
			}
			if (billbord && (Object)(object)base.gameObject != (Object)null && Camera.main)
			{
				Vector3 val = ((Component)Camera.main).transform.position - base.gameObject.transform.position;
				val.y = 0f;
				base.rotation = Quaternion.LookRotation(val);
			}
			if (duration > 0f)
			{
				elapse += Time.deltaTime * speed;
				if (elapse >= duration)
				{
					Stop();
				}
			}
		}
	}
}
