using System;
using System.Collections;
using System.Text;
using UnityEngine;
using ZF.Asset.Attributes;
using Object = UnityEngine.Object;

namespace ZF.Asset.Properties
{
	public class RoleProperty : DependsProperty, IAssetProperty
	{
		[Description("名称")]
		public string Name;

		[Description("包围盒")]
		public Bounds bounds;

		[Description("根骨骼名称")]
		public string rootBone = "Bip01";

		[Description("乘骑骨骼名称")]
		public string cavalryBone = string.Empty;

		[Description("缩放比例")]
		public float scale = 1f;

		[Description("染色参考")]
		public Color32[] colors = Array.Empty<Color32>();

		[Description("测试染色")]
		public Color32 testcolor;

		[HideInInspector]
		public Renderer[] renderers = Array.Empty<Renderer>();

		public bool Validate(IAssetValidator validator)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			if (validator != null)
			{
				return validator.Validate(this);
			}
			Bounds val = CalcBounds(((Component)this).gameObject, "weapon");
			if (val != bounds)
			{
				bounds = val;
			}
			renderers = ((Component)this).gameObject.GetComponentsInChildren<Renderer>(true);
			Collect(DependFlags.Shader);
			return true;
		}

		public static Bounds CalcBounds(GameObject go, string ignoreCalcNodeName)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			Vector3 max = Vector3.zero;
			Vector3 min = Vector3.zero;
			Renderer component = go.GetComponent<Renderer>();
			if ((Object)(object)component != (Object)null)
			{
				Bounds val = component.bounds;
				max = ((Bounds)val).max;
				min = ((Bounds) val).min;
			}
			RecurisionCalcBounds(go.transform(), ref max, ref min, ignoreCalcNodeName);
			Vector3 val2 = max - min;
			Vector3 val3 = (max + min) / 2f;
			Bounds result = default(Bounds);
			((Bounds)result)._002Ector(val3, val2);
			return result;
		}

		private static void RecurisionCalcBounds(Transform ts, ref Vector3 max, ref Vector3 min, string ignoreCalcNodeName)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Expected O, but got Unknown
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			if (ts.childCount <= 0)
			{
				return;
			}
			IEnumerator enumerator = ts.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform val = (Transform)enumerator.Current;
					if (!string.IsNullOrEmpty(ignoreCalcNodeName) && ignoreCalcNodeName == ((Object)val).name)
					{
						continue;
					}
					Renderer component = ((Component)val).GetComponent<Renderer>();
					if ((Object)(object)component != (Object)null)
					{
						Bounds val2 = component.bounds;
						if (((Vector3)max).Equals(Vector3.zero) && ((Vector3)min).Equals(Vector3.zero))
						{
							max = ((Bounds)val2).max;
							min = ((Bounds)val2).min;
						}
						if (((Bounds)val2).max.x > max.x)
						{
							max.x = ((Bounds)val2).max.x;
						}
						if (((Bounds)val2).max.y > max.y)
						{
							max.y = ((Bounds)val2).max.y;
						}
						if (((Bounds)val2).max.z > max.z)
						{
							max.z = ((Bounds)val2).max.z;
						}
						if (((Bounds)val2).min.x < min.x)
						{
							min.x = ((Bounds)val2).min.x;
						}
						if (((Bounds)val2).min.y < min.y)
						{
							min.y = ((Bounds)val2).min.y;
						}
						if (((Bounds)val2).min.z < min.z)
						{
							min.z = ((Bounds)val2).min.z;
						}
					}
					RecurisionCalcBounds(val, ref max, ref min, ignoreCalcNodeName);
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

		private string GetPath(GameObject node, GameObject root)
		{
			int num = 0;
			StringBuilder stringBuilder = new StringBuilder();
			Transform val = node.transform;
			while ((Object)(object)val != (Object)null && !((Object)(object)val == (Object)(object)root.transform))
			{
				stringBuilder.Insert(0, "/" + ((Object)val).name);
				if (++num == 100)
				{
					val = null;
					break;
				}
				val = val.parent;
			}
			if ((Object)(object)val == (Object)null)
			{
				return null;
			}
			stringBuilder.Remove(0, 1);
			return stringBuilder.ToString();
		}
	}
}
