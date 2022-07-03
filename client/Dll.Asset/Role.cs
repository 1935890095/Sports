using System;
using UnityEngine;
using XFX.Asset.Properties;
using XFX.Core.Render;
using Object = UnityEngine.Object;

namespace XFX.Asset
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
				if (property.scale != 1f || base.scale != Vector3.one)
				{
					base.scale *= property.scale;
				}
				base.renderers = property.renderers;
			}
		}

		protected override void OnScale()
		{
			if (!((Object)(object)property == (Object)null))
			{
				Bounds val = property.bounds;
				Vector3 val2 = new Vector3(val.extents.x * base.scale.x, val.extents.y * base.scale.y, val.extents.z * base.scale.z);
				Vector3 val3 = new Vector3(val.center.x, val.center.y - val.extents.y +val2.y, val.center.z);
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
						animatorParameters = animator.parameters;
					}
					for (int i = 0; i < animatorParameters.Length; i++)
					{
						AnimatorControllerParameter val = animatorParameters[i];
						if (val.name != text)
						{
							continue;
						}
						AnimatorControllerParameterType type = val.type;
						switch (type - 1)
						{
						case AnimatorControllerParameterType.Float:
							animator.SetFloat(text, float.Parse(text2));
							continue;
						case AnimatorControllerParameterType.Int:
							animator.SetInteger(text, int.Parse(text2));
							continue;
						case AnimatorControllerParameterType.Bool:
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
				animator.speed = speed;
				animator.Play(name);
			}
		}

		public bool IsPlaying(string name)
		{
			if ((Object)(object)animator == (Object)null)
			{
				return false;
			}
			for (int i = 0; i < animator.layerCount; i++)
			{
				AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(i);
				if ((currentAnimatorStateInfo).IsName(name))
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
