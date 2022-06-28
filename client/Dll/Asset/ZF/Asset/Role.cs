using System;
using UnityEngine;
using ZF.Asset.Properties;
using ZF.Core.Render;

namespace ZF.Asset
{
	public class Role : Depends, IRole, IRenderObject
	{
		private RoleProperty property;

		private AnimatorControllerParameter[] animatorParameters;

		public Animator animator { get; protected set; }

		public Bounds bounds { get; private set; }

		public Role()
		{
			base.destroyChildrenOnDestroy = true;
		}

		protected override void OnCreate(IRenderResource resource)
		{
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			if (!(resource.asset is GameObject))
			{
				Debug.LogWarning((object)("empty role resource: " + resource.name));
				Destroy();
				return;
			}
			Object obj = Object.Instantiate(resource.asset);
			base.gameObject = (GameObject)(object)((obj is GameObject) ? obj : null);
			if ((Object)(object)base.gameObject == (Object)null)
			{
				Debug.LogWarning((object)("empty role resource: " + resource.name));
				Destroy();
				return;
			}
			animator = base.gameObject.GetComponent<Animator>();
			property = base.gameObject.GetComponent<RoleProperty>();
			if ((Object)(object)property != (Object)null)
			{
				bounds = property.bounds;
				if (property.scale != 1f || base.scale != Vector3.get_one())
				{
					base.scale *= property.scale;
				}
				base.renderers = property.renderers;
			}
		}

		protected override void OnScale()
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			if (!((Object)(object)property == (Object)null))
			{
				Bounds val = property.bounds;
				Vector3 val2 = default(Vector3);
				((Vector3)(ref val2))._002Ector(((Bounds)(ref val)).get_extents().x * base.scale.x, ((Bounds)(ref val)).get_extents().y * base.scale.y, ((Bounds)(ref val)).get_extents().z * base.scale.z);
				Vector3 val3 = default(Vector3);
				((Vector3)(ref val3))._002Ector(((Bounds)(ref val)).get_center().x, ((Bounds)(ref val)).get_center().y - ((Bounds)(ref val)).get_extents().y + val2.y, ((Bounds)(ref val)).get_center().z);
				bounds = new Bounds(val3, val2 * 2f);
			}
		}

		protected override void OnDestroy()
		{
			RenderObject.DestroyObject(base.gameObject);
			base.gameObject = null;
			animator = null;
			animatorParameters = null;
		}

		public void Play(string name, float speed = 1f, string parameter = "")
		{
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Expected I4, but got Unknown
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Invalid comparison between Unknown and I4
			if ((Object)(object)animator == (Object)null)
			{
				return;
			}
			if (!string.IsNullOrEmpty(parameter))
			{
				try
				{
					string[] array = parameter.Split('=');
					string text = array[0];
					string text2 = null;
					if (array.Length > 1)
					{
						text2 = array[1];
					}
					if (animatorParameters == null)
					{
						animatorParameters = animator.get_parameters();
					}
					for (int i = 0; i < animatorParameters.Length; i++)
					{
						AnimatorControllerParameter val = animatorParameters[i];
						if (val.get_name() != text)
						{
							continue;
						}
						AnimatorControllerParameterType type = val.get_type();
						switch (type - 1)
						{
						case 0:
							animator.SetFloat(text, float.Parse(text2));
							continue;
						case 2:
							animator.SetInteger(text, int.Parse(text2));
							continue;
						case 3:
							animator.SetBool(text, bool.Parse(text2));
							continue;
						}
						if ((int)type == 9)
						{
							animator.SetTrigger(text);
						}
					}
				}
				catch (Exception ex)
				{
					Debug.LogError((object)("SetParameter: " + ex.ToString()));
				}
			}
			if (!string.IsNullOrEmpty(name))
			{
				animator.set_speed(speed);
				animator.Play(name);
			}
		}

		public bool IsPlaying(string name)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)animator == (Object)null)
			{
				return false;
			}
			for (int i = 0; i < animator.get_layerCount(); i++)
			{
				AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(i);
				if (((AnimatorStateInfo)(ref currentAnimatorStateInfo)).IsName(name))
				{
					return true;
				}
			}
			return false;
		}

		public void Attach(IRenderObject robj, string point = "")
		{
			if (point == string.Empty)
			{
				point = "root";
			}
			if (point == "root")
			{
				robj.parent = this;
			}
		}

		public void Detach(IRenderObject robj, string point = "")
		{
			robj.parent = null;
		}
	}
}
