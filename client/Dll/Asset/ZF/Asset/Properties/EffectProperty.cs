using System;
using System.Collections;
using UnityEngine;
using ZF.Asset.Attributes;

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
			float num = CalcDuration(((Component)this).get_gameObject());
			if (num != duration)
			{
				duration = num;
			}
			renderers = ((Component)this).get_gameObject().GetComponentsInChildren<Renderer>(true);
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
					EmissionModule emission = val.get_emission();
					if (!((EmissionModule)(ref emission)).get_enabled())
					{
						continue;
					}
					MainModule main = val.get_main();
					if (!((MainModule)(ref main)).get_loop())
					{
						MainModule main2 = val.get_main();
						float maxValue = GetMaxValue(((MainModule)(ref main2)).get_startDelay());
						MainModule main3 = val.get_main();
						float num2 = ((MainModule)(ref main3)).get_duration();
						MainModule main4 = val.get_main();
						float num3 = maxValue + Mathf.Max(num2, GetMaxValue(((MainModule)(ref main4)).get_startLifetime()));
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
					if ((Object)(object)val2 == (Object)null)
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
							if ((int)val3.get_wrapMode() == 2)
							{
								num = 0f;
								return num;
							}
							num4 += val3.get_length();
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
					if ((Object)(object)val4 == (Object)null || (Object)(object)val4.get_runtimeAnimatorController() == (Object)null)
					{
						continue;
					}
					AnimationClip[] animationClips = val4.get_runtimeAnimatorController().get_animationClips();
					float num5 = 0f;
					if (animationClips != null)
					{
						AnimationClip[] array2 = animationClips;
						int num6 = 0;
						while (num6 < array2.Length)
						{
							AnimationClip val5 = array2[num6];
							num5 += val5.get_length();
							if ((int)val5.get_wrapMode() != 2)
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

		private static float GetMaxValue(MinMaxCurve minMaxCurve)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Expected I4, but got Unknown
			ParticleSystemCurveMode mode = ((MinMaxCurve)(ref minMaxCurve)).get_mode();
			switch ((int)mode)
			{
			case 0:
				return ((MinMaxCurve)(ref minMaxCurve)).get_constant();
			case 3:
				return ((MinMaxCurve)(ref minMaxCurve)).get_constantMax();
			case 1:
				return GetMaxValue(((MinMaxCurve)(ref minMaxCurve)).get_curve());
			case 2:
			{
				float maxValue = GetMaxValue(((MinMaxCurve)(ref minMaxCurve)).get_curveMin());
				float maxValue2 = GetMaxValue(((MinMaxCurve)(ref minMaxCurve)).get_curveMax());
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
			for (int i = 0; i < curve.get_length(); i++)
			{
				Keyframe val = curve.get_Item(i);
				if (((Keyframe)(ref val)).get_time() > num)
				{
					Keyframe val2 = curve.get_Item(i);
					num = ((Keyframe)(ref val2)).get_time();
				}
			}
			return num;
		}
	}
}
