using System;
using System.Collections;
using UnityEngine;
using XFX.Asset.Attributes;

namespace XFX.Asset.Properties
{
	public class EffectProperty : DependsProperty, IAssetProperty
	{
		[Description("持续时间(秒)")]
		public float duration;

		[Description("骨骼挂载点")]
		public string bone = string.Empty;

		[Description("缩放比例")]
		public float scale = 1f;

		[HideInInspector]
		public Renderer[] renderers = Array.Empty<Renderer>();

		[ExecuteInEditMode]
		public bool Validate(IAssetValidator validator)
		{
			if (validator != null)
			{
				return validator.Validate(this);
			}
			float num = CalcDuration(((Component)this).gameObject);
			if (num != duration)
			{
				duration = num;
			}
			renderers = ((Component)this).gameObject.GetComponentsInChildren<Renderer>(true);
			Collect();
			return true;
		}

		public static float CalcDuration(GameObject gameObject)
		{
			ParticleSystem[] componentsInChildren = gameObject.GetComponentsInChildren<ParticleSystem>();
			Animation[] componentsInChildren2 = gameObject.GetComponentsInChildren<Animation>();
			Animator[] componentsInChildren3 = gameObject.GetComponentsInChildren<Animator>();
			float num = 0f;
			if (componentsInChildren != null)
			{
				ParticleSystem[] array = componentsInChildren;
				foreach (ParticleSystem val in array)
				{
					ParticleSystem.EmissionModule emission = val.emission;
					if (!emission.enabled)
					{
						continue;
					}
					ParticleSystem.MainModule main = val.main;
					if (!main.loop)
					{
						ParticleSystem.MainModule main2 = val.main;
						float maxValue = GetMaxValue(main2.startDelay);
						ParticleSystem.MainModule main3 = val.main;
						float num2 = ((ParticleSystem.MainModule)main3).duration;
						ParticleSystem.MainModule main4 = val.main;
						float num3 = maxValue + Mathf.Max(num2, GetMaxValue(((ParticleSystem.MainModule)main4).startLifetime));
						if (num3 > num)
						{
							num = num3;
						}
						continue;
					}
					goto IL_0062;
				}
			}
			if (componentsInChildren2 != null)
			{
				foreach (Animation val2 in componentsInChildren2)
				{
					if (val2 == null)
					{
						continue;
					}
					float num4 = 0f;
					IEnumerator enumerator = val2.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							AnimationState val3 = (AnimationState)enumerator.Current;
							if ((int)val3.wrapMode == 2)
							{
								num = 0f;
								return num;
							}
							num4 += val3.length;
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
					if (num4 > num)
					{
						num = num4;
					}
				}
			}
			if (componentsInChildren3 != null)
			{
				foreach (Animator val4 in componentsInChildren3)
				{
					if (val4 == null || val4.runtimeAnimatorController == null)
					{
						continue;
					}
					AnimationClip[] animationClips = val4.runtimeAnimatorController.animationClips;
					float num5 = 0f;
					if (animationClips != null)
					{
						AnimationClip[] array2 = animationClips;
						int num6 = 0;
						while (num6 < array2.Length)
						{
							AnimationClip val5 = array2[num6];
							num5 += val5.length;
							if ((int)val5.wrapMode != 2)
							{
								num6++;
								continue;
							}
							goto IL_01fb;
						}
					}
					if (num5 > num)
					{
						num = num5;
					}
					continue;
					IL_01fb:
					num = 0f;
					break;
				}
			}
			goto IL_0232;
			IL_0232:
			return num;
			IL_0062:
			num = 0f;
			goto IL_0232;
		}

		private static float GetMaxValue(ParticleSystem.MinMaxCurve minMaxCurve)
		{
			ParticleSystemCurveMode mode = minMaxCurve.mode;
			switch ((int)mode)
			{
			case 0:
				return minMaxCurve.constant;
			case 3:
				return minMaxCurve.constantMax;
			case 1:
				return GetMaxValue(minMaxCurve.curve);
			case 2:
			{
				float maxValue = GetMaxValue(minMaxCurve.curveMin);
				float maxValue2 = GetMaxValue(minMaxCurve.curveMax);
				return (!(maxValue > maxValue2)) ? maxValue2 : maxValue;
			}
			default:
				return 0f;
			}
		}

		private static float GetMaxValue(AnimationCurve curve)
		{
			float num = 0f;
			for (int i = 0; i < curve.length; i++)
			{
				Keyframe val = curve[i];
				if (val.time > num)
				{
					Keyframe val2 = curve[i];
					num = val2.time;
				}
			}
			return num;
		}
	}
}
