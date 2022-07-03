using System;
using System.Collections;
using System.Text;
using UnityEngine;
using XFX.Asset.Attributes;

namespace XFX.Asset.Properties
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
			Vector3 max = Vector3.zero;
			Vector3 min = Vector3.zero;
			Renderer component = go.GetComponent<Renderer>();
			if (component != null)
			{
				Bounds val = component.bounds;
				max = val.max;
				min = val.min;
			}
			RecurisionCalcBounds(go.transform, ref max, ref min, ignoreCalcNodeName);
			Vector3 val2 = max - min;
			Vector3 val3 = (max + min) / 2f;
			Bounds result = default(Bounds);
			result.SetMinMax(val3, val2);
			return result;
		}

		private static void RecurisionCalcBounds(Transform ts, ref Vector3 max, ref Vector3 min, string ignoreCalcNodeName)
		{
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
					if (!string.IsNullOrEmpty(ignoreCalcNodeName) && ignoreCalcNodeName == (val).name)
					{
						continue;
					}
					Renderer component = ((Component)val).GetComponent<Renderer>();
					if (component != null)
					{
						Bounds val2 = component.bounds;
						if (max.Equals(Vector3.zero) && min.Equals(Vector3.zero))
						{
							max = val2.max;
							min = val2.min;
						}
						if (val2.max.x > max.x)
						{
							max.x = val2.max.x;
						}
						if (val2.max.y > max.y)
						{
							max.y = val2.max.y;
						}
						if (val2.max.z > max.z)
						{
							max.z = val2.max.z;
						}
						if (val2.min.x < min.x)
						{
							min.x = val2.min.x;
						}
						if (val2.min.y < min.y)
						{
							min.y = val2.min.y;
						}
						if (val2.min.z < min.z)
						{
							min.z = val2.min.z;
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
			while (val != null && !(val == root.transform))
			{
				stringBuilder.Insert(0, "/" + (val).name);
				if (++num == 100)
				{
					val = null;
					break;
				}
				val = val.parent;
			}
			if (val == null)
			{
				return null;
			}
			stringBuilder.Remove(0, 1);
			return stringBuilder.ToString();
		}
	}
}
