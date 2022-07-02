using System;
using System.Collections;
using UnityEngine;
using ZF.Asset.Attributes;
using Object = UnityEngine.Object;

namespace ZF.Asset.Properties
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
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Expected O, but got Unknown
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Invalid comparison between Unknown and I4
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Invalid comparison between Unknown and I4
			ParticleSystem[] componentsInChildren = gameObject.GetComponentsInChildren<ParticleSystem>();
			Animation[] componentsInChildren2 = gameObject.GetComponentsInChildren<Animation>();
			Animator[] componentsInChildren3 = gameObject.GetComponentsInChildren<Animator>();
			float num = 0f;
			if (componentsInChildren != null)
			{
				ParticleSystem[] array = componentsInChildren;
				foreach (ParticleSystem val in array)
				{
					EmissionModule emission = val.emission;
					if (!((EmissionModule)emission).enabled)
					{
						continue;
					}
					ParticleSystem.MainModule main = val.main;
					if (!((ParticleSystem.MainModule)main).loop)
					{
						ParticleSystem.MainModule main2 = val.main;
						float maxValue = GetMaxValue(((ParticleSystem.MainModule)main2).startDelay);
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
					if (val2 == (Object)null)
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
					if (val4 == (Object)null || val4.runtimeAnimatorController == (Object)null)
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
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Expected I4, but got Unknown
			ParticleSystemCurveMode mode = ((ParticleSystem.MinMaxCurve)minMaxCurve).mode;
			switch ((int)mode)
			{
			case 0:
				return ((ParticleSystem.MinMaxCurve)minMaxCurve).constant;
			case 3:
				return ((ParticleSystem.MinMaxCurve)minMaxCurve).constantMax;
			case 1:
				return GetMaxValue(((ParticleSystem.MinMaxCurve)minMaxCurve).curve);
			case 2:
			{
				float maxValue = GetMaxValue(((ParticleSystem.MinMaxCurve)minMaxCurve).curveMin);
				float maxValue2 = GetMaxValue(((ParticleSystem.MinMaxCurve)minMaxCurve).curveMax);
				return (!(maxValue > maxValue2)) ? maxValue2 : maxValue;
			}
			default:
				return 0f;
			}
		}

		private static float GetMaxValue(AnimationCurve curve)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			float num = 0f;
			for (int i = 0; i < curve.length; i++)
			{
				Keyframe val = curve.keys[i];
				if (((Keyframe)val).time > num)
				{
					Keyframe val2 = curve.keys[i];
					num = ((Keyframe)val2).time;
				}
			}
			return num;
		}
	}
}
