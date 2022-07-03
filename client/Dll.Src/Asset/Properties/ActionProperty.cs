using System;
using UnityEngine;
using XFX.Asset.Attributes;

namespace XFX.Asset.Properties
{
	[CreateAssetMenu]
	public class ActionProperty : ScriptableObject, IAssetProperty
	{
		public enum EventPart
		{
			NONE,
			SING,
			FIRE,
			HIT
		}

		public enum EventType
		{
			NONE,
			PLAY_ANIMATION,
			PLAY_AUDIO,
			PLAY_EFFECT,
			PLAY_EFFECT_MAP
		}

		[Serializable]
		public class Event
		{
			[Description("名称")]
			public string name = string.Empty;

			[Description("触发阶段")]
			public EventPart part;

			[Description("事件类型")]
			public EventType type;

			[Description("偏移时间(秒)")]
			public float offset;

			[Description("动画")]
			public string anim = string.Empty;

			[AssetPath("音效", "audio", typeof(AudioClip))]
			public string audio = string.Empty;

			[AssetPath("特效", "特效", ".prefab")]
			public string effect = string.Empty;

			[Description("脚本id")]
			public string scriptid;
		}

		[Description("持续时间(秒)")]
		public float duration = 5f;

		public Event[] events = Array.Empty<Event>();

		public ActionProperty()
		{
		}

		public bool Validate(IAssetValidator validator)
		{
			return validator?.Validate(this) ?? false;
		}
	}
}
