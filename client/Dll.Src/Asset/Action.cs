using System;
using System.Collections.Generic;
using UnityEngine;
using XFX.Asset.Properties;
using XFX.Core.Render;

namespace XFX.Asset
{
	public class Action : RenderObject, IAction, IRenderObject
	{
		internal enum Status
		{
			None,
			Playing,
			Paused,
			Stopped
		}

		private float speed_ = 1f;

		private Status status;

		private ActionProperty property;

		private IActionContext context;

		private float elapse;

		private int pos;

		private List<IRenderObject> container = new List<IRenderObject>();

		public float duration { get; private set; }

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
					Debug.LogWarning((object)"action speed require more then zero");
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

		public bool isPlaying => status == Status.Playing;

		protected override void OnCreate(IRenderResource resource)
		{
			property = resource.asset as ActionProperty;
			duration = property.duration;
			if (duration <= 0f)
			{
				duration = 60f;
			}
			base.destroyChildrenOnDestroy = true;
		}

		public void Play(IActionContext context = null)
		{
			this.context = context;
			status = Status.Playing;
			elapse = 0f;
			pos = 0;
		}

		public void Stop()
		{
			elapse = 0f;
			pos = 0;
			status = Status.Stopped;
			Destroy();
		}

		public void Pause()
		{
			if (status == Status.Playing)
			{
				status = Status.Paused;
			}
		}

		public void Continue()
		{
			if (status == Status.Paused)
			{
				status = Status.Playing;
			}
		}

		protected override void OnDestroy()
		{
			for (int i = 0; i < container.Count; i++)
			{
				container[i].Destroy();
			}
			container.Clear();
		}

		protected override void OnUpdate()
		{
			if (!base.complete || !isPlaying)
			{
				return;
			}
			elapse += Time.deltaTime * speed;
			while (pos < property.events.Length)
			{
				ActionProperty.Event e = property.events[pos];
				if (!(e.offset <= elapse))
				{
					break;
				}
				if (context != null)
				{
					IRole[] array = null;
					switch (e.part)
					{
					case ActionProperty.EventPart.SING:
						array = context.OnSing(e.scriptid);
						break;
					case ActionProperty.EventPart.FIRE:
						array = context.OnFire(e.scriptid);
						break;
					case ActionProperty.EventPart.HIT:
						array = context.OnHit(e.scriptid);
						break;
					}
					if (array != null)
					{
						Array.ForEach(array, delegate(IRole role)
						{
							OnEvent(e, role);
						});
					}
				}
				else
				{
					OnEvent(e, null);
				}
				pos++;
			}
			if (elapse >= duration)
			{
				Stop();
			}
		}

		protected virtual void OnEvent(ActionProperty.Event evt, IRole role)
		{
			switch (evt.type)
			{
			case ActionProperty.EventType.PLAY_ANIMATION:
				PlayAnim(evt, role);
				break;
			case ActionProperty.EventType.PLAY_AUDIO:
				PlayAudio(evt, role);
				break;
			case ActionProperty.EventType.PLAY_EFFECT:
				PlayEffect(evt, role, onmap: false);
				break;
			case ActionProperty.EventType.PLAY_EFFECT_MAP:
				PlayEffect(evt, role, onmap: true);
				break;
			}
		}

		private void PlayAnim(ActionProperty.Event evt, IRole role)
		{
			if (role != null && !role.destroyed)
			{
				if (evt.anim == "attack")
				{
					int num = Time.frameCount % 3;
					role.Play(evt.anim, speed, "attack_value=" + num);
				}
				else
				{
					role.Play(evt.anim, speed, string.Empty);
				}
			}
		}

		private void PlayAudio(ActionProperty.Event evt, IRole role)
		{
			if (!string.IsNullOrEmpty(evt.audio))
			{
				Audio audio = RenderInstance.Create<Audio>(evt.audio, this, 0, string.Empty);
				audio.Play();
				container.Add(audio);
			}
		}

		private void PlayEffect(ActionProperty.Event evt, IRole role, bool onmap)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			if (role != null)
			{
				if (onmap)
				{
					Effect effect = RenderInstance.Create<Effect>(evt.effect, this, 0, string.Empty);
					effect.position = role.position;
					effect.speed = speed;
					effect.Play();
					container.Add(effect);
				}
				else if (!role.destroyed)
				{
					Effect effect2 = RenderInstance.Create<Effect>(evt.effect, role, 0, string.Empty);
					effect2.speed = speed;
					effect2.Play();
					container.Add(effect2);
				}
			}
		}
	}
}
