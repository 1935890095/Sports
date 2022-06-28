using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZF.Core.Render
{
	public abstract class RenderObject : IRenderObject
	{
		private IRenderResource resource;

		private Action<IRenderObject> onComplete_;

		private string name_ = string.Empty;

		private bool dontDestroyOnLoad_;

		private int layer_;

		private int order_;

		private int sortingLayer_;

		private string tag_ = string.Empty;

		private Vector3 scale_ = Vector3.get_one();

		private Vector3 position_ = Vector3.get_zero();

		private Quaternion rotation_ = Quaternion.get_identity();

		private bool active_ = true;

		private bool visible_ = true;

		private IRenderObject parent_;

		private List<IRenderComponent> components;

		private List<IRenderObject> children;

		private IEnumerator createitr;

		public bool complete { get; private set; }

		public bool destroyed { get; private set; }

		public GameObject gameObject { get; protected set; }

		public bool destroyChildrenOnDestroy { get; set; }

		protected Renderer[] renderers { get; set; }

		public bool dontDestroyOnLoad
		{
			get
			{
				return dontDestroyOnLoad_;
			}
			set
			{
				dontDestroyOnLoad_ = value;
				if (value && Object.op_Implicit((Object)(object)gameObject) && (Object)(object)gameObject.get_transform().get_parent() == (Object)null)
				{
					Object.DontDestroyOnLoad((Object)(object)gameObject);
				}
			}
		}

		public Action<IRenderObject> onComplete
		{
			set
			{
				if (complete)
				{
					value(this);
				}
				else
				{
					onComplete_ = value;
				}
			}
		}

		public string name
		{
			get
			{
				return name_;
			}
			set
			{
				name_ = value;
				if (!string.IsNullOrEmpty(value) && Object.op_Implicit((Object)(object)gameObject))
				{
					((Object)gameObject).set_name(value);
				}
			}
		}

		public int layer
		{
			get
			{
				return layer_;
			}
			set
			{
				if (value != 0)
				{
					layer_ = value;
					if (complete)
					{
						_ApplyLayer();
					}
					OnSetLayer(value);
				}
			}
		}

		public int order
		{
			get
			{
				return order_;
			}
			set
			{
				if (value != 0)
				{
					order_ = value;
					if (complete)
					{
						_ApplyOrder();
					}
				}
			}
		}

		public int sortingLayer
		{
			get
			{
				return sortingLayer_;
			}
			set
			{
				if (value != 0)
				{
					sortingLayer_ = value;
					if (complete)
					{
						_ApplySortingLayer();
					}
				}
			}
		}

		public string tag
		{
			get
			{
				return tag_;
			}
			set
			{
				if (value == null)
				{
					value = string.Empty;
				}
				tag_ = value;
				if ((Object)(object)gameObject != (Object)null)
				{
					gameObject.set_tag(tag_);
				}
			}
		}

		public Vector3 scale
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return scale_;
			}
			set
			{
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_0039: Unknown result type (might be due to invalid IL or missing references)
				//IL_003a: Unknown result type (might be due to invalid IL or missing references)
				if ((Object)(object)gameObject != (Object)null && (Object)(object)gameObject.get_transform() != (Object)null)
				{
					gameObject.get_transform().set_localScale(value);
				}
				scale_ = value;
				OnScale();
			}
		}

		public Vector3 position
		{
			get
			{
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_0039: Unknown result type (might be due to invalid IL or missing references)
				if ((Object)(object)gameObject != (Object)null && (Object)(object)gameObject.get_transform() != (Object)null)
				{
					return gameObject.get_transform().get_position();
				}
				return position_;
			}
			set
			{
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_0039: Unknown result type (might be due to invalid IL or missing references)
				//IL_003a: Unknown result type (might be due to invalid IL or missing references)
				if ((Object)(object)gameObject != (Object)null && (Object)(object)gameObject.get_transform() != (Object)null)
				{
					gameObject.get_transform().set_localPosition(value);
				}
				position_ = value;
			}
		}

		public Vector3 forward
		{
			get
			{
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_003e: Unknown result type (might be due to invalid IL or missing references)
				if ((Object)(object)gameObject != (Object)null && (Object)(object)gameObject.get_transform() != (Object)null)
				{
					return gameObject.get_transform().get_forward();
				}
				return ((Quaternion)(ref rotation_)).get_eulerAngles();
			}
			set
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				Quaternion val2 = (rotation = Quaternion.LookRotation(value));
			}
		}

		public Quaternion rotation
		{
			get
			{
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_0039: Unknown result type (might be due to invalid IL or missing references)
				if ((Object)(object)gameObject != (Object)null && (Object)(object)gameObject.get_transform() != (Object)null)
				{
					return gameObject.get_transform().get_rotation();
				}
				return rotation_;
			}
			set
			{
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_0039: Unknown result type (might be due to invalid IL or missing references)
				//IL_003a: Unknown result type (might be due to invalid IL or missing references)
				if ((Object)(object)gameObject != (Object)null && (Object)(object)gameObject.get_transform() != (Object)null)
				{
					gameObject.get_transform().set_localRotation(value);
				}
				rotation_ = value;
			}
		}

		public Vector3 euler
		{
			get
			{
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_0037: Unknown result type (might be due to invalid IL or missing references)
				//IL_003a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0046: Unknown result type (might be due to invalid IL or missing references)
				if ((Object)(object)gameObject != (Object)null && (Object)(object)gameObject.get_transform() != (Object)null)
				{
					Quaternion val = gameObject.get_transform().get_rotation();
					return ((Quaternion)(ref val)).get_eulerAngles();
				}
				return ((Quaternion)(ref rotation_)).get_eulerAngles();
			}
			set
			{
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_0033: Unknown result type (might be due to invalid IL or missing references)
				//IL_003e: Unknown result type (might be due to invalid IL or missing references)
				//IL_003f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				if ((Object)(object)gameObject != (Object)null && (Object)(object)gameObject.get_transform() != (Object)null)
				{
					gameObject.get_transform().set_localRotation(Quaternion.Euler(value));
				}
				rotation_ = Quaternion.Euler(value);
			}
		}

		public bool active
		{
			get
			{
				return active_;
			}
			set
			{
				if (active_ != value)
				{
					active_ = value;
					if (complete)
					{
						_ApplyActive();
					}
				}
			}
		}

		public bool visible
		{
			get
			{
				return visible_;
			}
			set
			{
				if (visible_ != value)
				{
					visible_ = value;
					if (complete)
					{
						_ApplyVisible();
					}
				}
			}
		}

		public IRenderObject parent
		{
			get
			{
				return parent_;
			}
			set
			{
				if (parent_ != null)
				{
					parent_.RemoveChild(this);
				}
				parent_ = value;
				if (parent_ != null)
				{
					parent_.AddChild(this);
				}
				if (complete)
				{
					_ApplyParent();
					_ApplyPosition();
				}
			}
		}

		public bool oncmd { get; set; }

		public T AddComponent<T>() where T : IRenderComponent
		{
			return (T)AddComponent(typeof(T));
		}

		public void RemoveComponent<T>() where T : IRenderComponent
		{
			RemoveComponent(typeof(T));
		}

		public T GetComponent<T>() where T : IRenderComponent
		{
			return (T)GetComponent(typeof(T));
		}

		public IRenderComponent AddComponent(Type type)
		{
			if (components == null)
			{
				components = new List<IRenderComponent>();
			}
			IRenderComponent renderComponent = (IRenderComponent)Activator.CreateInstance(type);
			components.Add(renderComponent);
			return renderComponent;
		}

		public void RemoveComponent(Type type)
		{
			if (components == null)
			{
				return;
			}
			for (int i = 0; i < components.Count; i++)
			{
				if (components[i].GetType() == type)
				{
					components[i] = null;
					break;
				}
			}
		}

		public IRenderComponent GetComponent(Type type)
		{
			if (components == null)
			{
				return null;
			}
			IRenderComponent result = null;
			for (int i = 0; i < components.Count; i++)
			{
				IRenderComponent renderComponent = components[i];
				if (renderComponent != null)
				{
					Type type2 = renderComponent.GetType();
					if (type2.IsSubclassOf(type))
					{
						result = renderComponent;
					}
					if (type2 == type)
					{
						result = renderComponent;
						break;
					}
				}
			}
			return result;
		}

		void IRenderObject.AddChild(IRenderObject child)
		{
			if (children == null)
			{
				children = new List<IRenderObject>();
			}
			children.Add(child);
			if ((Object)(object)gameObject != (Object)null && (Object)(object)child.gameObject != (Object)null)
			{
				child.gameObject.get_transform().SetParent(gameObject.get_transform());
			}
		}

		void IRenderObject.RemoveChild(IRenderObject child)
		{
			if (children == null)
			{
				return;
			}
			int num = children.IndexOf(child);
			if (num != -1)
			{
				children[num] = null;
				if ((Object)(object)gameObject != (Object)null && (Object)(object)child.gameObject != (Object)null && (Object)(object)gameObject.get_transform() == (Object)(object)child.gameObject.get_transform().get_parent())
				{
					child.gameObject.get_transform().SetParent((Transform)null);
				}
			}
		}

		void IRenderObject.Create(IRenderResource resource)
		{
			if (destroyed)
			{
				return;
			}
			this.resource = resource;
			if (createitr == null)
			{
				IEnumerator enumerator = OnCreateAs(resource);
				if (enumerator != null && enumerator.MoveNext())
				{
					createitr = enumerator;
					return;
				}
			}
			OnCreate(resource);
			if (RenderFactory.Settings.ReplaceShaderInEditorMode && Application.get_isEditor())
			{
				ReplaceShader();
			}
			createitr = null;
			complete = true;
			if (onComplete_ != null)
			{
				onComplete_(this);
				onComplete_ = null;
			}
			if (Object.op_Implicit((Object)(object)gameObject))
			{
				if (dontDestroyOnLoad)
				{
					Object.DontDestroyOnLoad((Object)(object)gameObject);
				}
				if (!string.IsNullOrEmpty(name_))
				{
					((Object)gameObject).set_name(name_);
				}
				if (!string.IsNullOrEmpty(tag_))
				{
					gameObject.set_tag(tag_);
				}
			}
			_ApplyParent();
			_ApplyLayer();
			_ApplyOrder();
			_ApplySortingLayer();
			_ApplyPosition();
			_ApplyVisible();
			int num = 0;
			while (components != null && num < components.Count)
			{
				IRenderComponent renderComponent = components[num];
				if (renderComponent != null)
				{
					try
					{
						renderComponent.Create(this);
					}
					catch (Exception ex)
					{
						Debug.LogError((object)ex.ToString());
					}
				}
				num++;
			}
			_ApplyActive();
		}

		public void Destroy()
		{
			if (destroyed)
			{
				return;
			}
			int num = 0;
			while (children != null && num < children.Count)
			{
				IRenderObject renderObject = children[num];
				if (renderObject != null)
				{
					renderObject.parent = null;
					if (destroyChildrenOnDestroy)
					{
						try
						{
							renderObject.Destroy();
						}
						catch (Exception ex)
						{
							Debug.LogError((object)ex.ToString());
						}
					}
				}
				num++;
			}
			children = null;
			int num2 = 0;
			while (components != null && num2 < components.Count)
			{
				IRenderComponent renderComponent = components[num2];
				if (renderComponent != null)
				{
					try
					{
						renderComponent.Destroy();
					}
					catch (Exception ex2)
					{
						Debug.LogError((object)ex2.ToString());
					}
				}
				num2++;
			}
			parent = null;
			try
			{
				OnDestroy();
			}
			catch (Exception ex3)
			{
				Debug.LogError((object)ex3.ToString());
			}
			if (resource != null)
			{
				resource.RemoveInstance(this);
				resource = null;
			}
			createitr = null;
			components = null;
			destroyed = true;
		}

		public void Update()
		{
			if (!active || destroyed)
			{
				return;
			}
			if (createitr != null)
			{
				try
				{
					if (!createitr.MoveNext())
					{
						((IRenderObject)this).Create(resource);
						return;
					}
				}
				catch (Exception ex)
				{
					Debug.LogError((object)ex.ToString());
				}
			}
			if (complete)
			{
				int num = -1;
				int num2 = 0;
				while (components != null && num2 < components.Count)
				{
					IRenderComponent renderComponent = components[num2];
					if (renderComponent == null)
					{
						if (num == -1)
						{
							num = num2;
						}
					}
					else
					{
						try
						{
							if (renderComponent.renderObject == null)
							{
								renderComponent.Create(this);
							}
							if (renderComponent.enabled)
							{
								renderComponent.Update();
							}
						}
						catch (Exception ex2)
						{
							Debug.LogError((object)ex2.ToString());
						}
						if (components != null && num != -1)
						{
							components[num] = renderComponent;
							components[num2] = null;
							num++;
						}
					}
					num2++;
				}
				if (components != null && num != -1)
				{
					components.RemoveRange(num, components.Count - num);
				}
			}
			OnUpdate();
			int num3 = -1;
			int num4 = 0;
			while (children != null && num4 < children.Count)
			{
				IRenderObject renderObject = children[num4];
				if (renderObject == null)
				{
					if (num3 == -1)
					{
						num3 = num4;
					}
				}
				else
				{
					try
					{
						renderObject.Update();
					}
					catch (Exception ex3)
					{
						Debug.LogError((object)ex3.ToString());
					}
					if (children != null && num3 != -1)
					{
						children[num3] = renderObject;
						children[num4] = null;
						num3++;
					}
				}
				num4++;
			}
			if (children != null && num3 != -1)
			{
				children.RemoveRange(num3, children.Count - num3);
			}
		}

		public void LateUpdate()
		{
			if (!active || destroyed)
			{
				return;
			}
			int num = 0;
			while (components != null && num < components.Count)
			{
				IRenderComponent renderComponent = components[num];
				if (renderComponent != null)
				{
					try
					{
						if (renderComponent.enabled)
						{
							renderComponent.LateUpdate();
						}
					}
					catch (Exception ex)
					{
						Debug.LogError((object)ex.ToString());
					}
				}
				num++;
			}
			OnLateUpdate();
			int num2 = 0;
			while (children != null && num2 < children.Count)
			{
				IRenderObject renderObject = children[num2];
				if (renderObject != null)
				{
					try
					{
						renderObject.LateUpdate();
					}
					catch (Exception ex2)
					{
						Debug.LogError((object)ex2.ToString());
					}
				}
				num2++;
			}
		}

		public void DestroyChildren()
		{
			int num = 0;
			while (children != null && num < children.Count)
			{
				IRenderObject renderObject = children[num];
				if (renderObject != null)
				{
					renderObject.parent = null;
					renderObject.Destroy();
				}
				num++;
			}
			children = null;
		}

		public void Command(string cmd, params object[] args)
		{
			if (!oncmd || !active)
			{
				return;
			}
			int num = 0;
			while (components != null && num < components.Count)
			{
				IRenderComponent renderComponent = components[num];
				if (renderComponent != null)
				{
					try
					{
						renderComponent.Command(cmd, args);
					}
					catch (Exception ex)
					{
						Debug.LogError((object)ex.ToString());
					}
				}
				num++;
			}
			int num2 = 0;
			while (children != null && num2 < children.Count)
			{
				IRenderObject renderObject = children[num2];
				if (renderObject != null)
				{
					try
					{
						renderObject.Command(cmd, args);
					}
					catch (Exception ex2)
					{
						Debug.LogError((object)ex2.ToString());
					}
				}
				num2++;
			}
		}

		public void ApplyParticleScale(float fscale)
		{
			if (!((Object)(object)gameObject == (Object)null))
			{
				ApplyParticleScale(gameObject.get_transform(), fscale);
			}
		}

		private void ApplyParticleScale(Transform go, float fscale)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			fscale = Mathf.Abs(fscale);
			if (!Object.op_Implicit((Object)(object)go) || Mathf.Approximately(fscale, 1f))
			{
				return;
			}
			ParticleSystem[] componentsInChildren = ((Component)go).GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem val in componentsInChildren)
			{
				MainModule main = val.get_main();
				MainModule main2 = val.get_main();
				MinMaxCurve startSizeX = ((MainModule)(ref main2)).get_startSizeX();
				((MainModule)(ref main)).set_startSizeX(new MinMaxCurve(((MinMaxCurve)(ref startSizeX)).get_constant() * fscale));
				MainModule main3 = val.get_main();
				if (((MainModule)(ref main3)).get_startSize3D())
				{
					MainModule main4 = val.get_main();
					MinMaxCurve startSizeY = ((MainModule)(ref main4)).get_startSizeY();
					((MainModule)(ref main)).set_startSizeY(new MinMaxCurve(((MinMaxCurve)(ref startSizeY)).get_constant() * fscale));
					MainModule main5 = val.get_main();
					MinMaxCurve startSizeZ = ((MainModule)(ref main5)).get_startSizeZ();
					((MainModule)(ref main)).set_startSizeZ(new MinMaxCurve(((MinMaxCurve)(ref startSizeZ)).get_constant() * fscale));
				}
				ShapeModule shape = val.get_shape();
				if (((ShapeModule)(ref shape)).get_enabled())
				{
					ShapeModule shape2 = val.get_shape();
					ShapeModule shape3 = val.get_shape();
					((ShapeModule)(ref shape2)).set_radius(((ShapeModule)(ref shape3)).get_radius() * fscale);
					ShapeModule shape4 = val.get_shape();
					((ShapeModule)(ref shape2)).set_scale(((ShapeModule)(ref shape4)).get_scale() * fscale);
				}
				((Component)val).get_transform().set_localScale(Vector3.get_one());
			}
		}

		protected static void DestroyObject(GameObject go)
		{
			if (Application.get_isEditor())
			{
				Object.DestroyImmediate((Object)(object)go, true);
			}
			else
			{
				Object.Destroy((Object)(object)go);
			}
		}

		protected virtual void OnCreate(IRenderResource resource)
		{
		}

		protected virtual IEnumerator OnCreateAs(IRenderResource resource)
		{
			return null;
		}

		protected virtual void OnDestroy()
		{
		}

		protected virtual void OnUpdate()
		{
		}

		protected virtual void OnLateUpdate()
		{
		}

		protected virtual void OnVisible(bool visible)
		{
		}

		protected virtual void OnScale()
		{
		}

		protected virtual void OnSetLayer(int layer)
		{
		}

		private void _ApplyPosition()
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)gameObject != (Object)null)
			{
				gameObject.get_transform().set_localScale(scale_);
				gameObject.get_transform().set_localPosition(position_);
				gameObject.get_transform().set_localRotation(rotation_);
			}
			int num = 0;
			while (children != null && num < children.Count)
			{
				IRenderObject renderObject = children[num];
				if (renderObject != null && (Object)(object)gameObject != (Object)null && (Object)(object)renderObject.gameObject != (Object)null && (Object)(object)gameObject.get_transform() == (Object)(object)renderObject.gameObject.get_transform().get_parent())
				{
					renderObject.gameObject.get_transform().SetParent(gameObject.get_transform());
					if (renderObject is RenderObject)
					{
						RenderObject renderObject2 = renderObject as RenderObject;
						renderObject2._ApplyPosition();
					}
				}
				num++;
			}
		}

		private void _ApplyActive()
		{
			if ((Object)(object)gameObject != (Object)null)
			{
				gameObject.SetActive(active_);
			}
			int num = 0;
			while (components != null && num < components.Count)
			{
				IRenderComponent renderComponent = components[num];
				if (renderComponent != null)
				{
					try
					{
						renderComponent.enabled = active;
					}
					catch (Exception ex)
					{
						Debug.LogError((object)ex.ToString());
					}
				}
				num++;
			}
		}

		private void _ApplyParent()
		{
			if ((Object)(object)gameObject == (Object)null)
			{
				return;
			}
			if (parent_ == null || (Object)(object)parent_.gameObject == (Object)null)
			{
				if (dontDestroyOnLoad)
				{
					Object.DontDestroyOnLoad((Object)(object)gameObject);
				}
			}
			else
			{
				gameObject.get_transform().SetParent(parent_.gameObject.get_transform());
			}
		}

		private void _ApplyLayer()
		{
			if (parent_ != null && layer_ == 0 && parent.layer != 0)
			{
				layer_ = parent.layer;
			}
			if (layer_ == 0)
			{
				return;
			}
			int num = 0;
			while (children != null && num < children.Count)
			{
				IRenderObject renderObject = children[num];
				if (renderObject != null && renderObject.layer == 0)
				{
					renderObject.layer = layer_;
				}
				num++;
			}
			if (renderers == null && Object.op_Implicit((Object)(object)gameObject))
			{
				renderers = gameObject.GetComponentsInChildren<Renderer>(true);
			}
			if (renderers != null)
			{
				for (int i = 0; i < renderers.Length; i++)
				{
					Renderer val = renderers[i];
					if ((Object)(object)val != (Object)null)
					{
						((Component)val).get_gameObject().set_layer(layer_);
					}
				}
			}
			if (Object.op_Implicit((Object)(object)gameObject))
			{
				gameObject.set_layer(layer_);
			}
		}

		private void _ApplyOrder()
		{
			if (order_ == 0)
			{
				return;
			}
			if (renderers == null && Object.op_Implicit((Object)(object)gameObject))
			{
				renderers = gameObject.GetComponentsInChildren<Renderer>(true);
			}
			if (renderers == null)
			{
				return;
			}
			for (int i = 0; i < renderers.Length; i++)
			{
				Renderer val = renderers[i];
				if ((Object)(object)val != (Object)null)
				{
					val.set_sortingOrder(order_);
				}
			}
		}

		private void _ApplySortingLayer()
		{
			if (sortingLayer_ == 0)
			{
				return;
			}
			if (renderers == null && Object.op_Implicit((Object)(object)gameObject))
			{
				renderers = gameObject.GetComponentsInChildren<Renderer>(true);
			}
			if (renderers == null)
			{
				return;
			}
			for (int i = 0; i < renderers.Length; i++)
			{
				Renderer val = renderers[i];
				if ((Object)(object)val != (Object)null)
				{
					val.set_sortingLayerID(sortingLayer_);
				}
			}
		}

		private void _ApplyVisible()
		{
			if (renderers == null && Object.op_Implicit((Object)(object)gameObject))
			{
				renderers = gameObject.GetComponentsInChildren<Renderer>(true);
			}
			if (renderers != null)
			{
				for (int i = 0; i < renderers.Length; i++)
				{
					if (renderers[i].get_enabled() != visible_)
					{
						renderers[i].set_enabled(visible_);
					}
				}
			}
			OnVisible(visible_);
		}

		private void ReplaceShader()
		{
			if (renderers == null && Object.op_Implicit((Object)(object)gameObject))
			{
				renderers = gameObject.GetComponentsInChildren<Renderer>(true);
			}
			if (renderers == null)
			{
				return;
			}
			Renderer[] array = renderers;
			foreach (Renderer val in array)
			{
				Material[] sharedMaterials = val.get_sharedMaterials();
				foreach (Material val2 in sharedMaterials)
				{
					if (Object.op_Implicit((Object)(object)val2) && Object.op_Implicit((Object)(object)val2.get_shader()))
					{
						Shader val3 = Shader.Find(((Object)val2.get_shader()).get_name());
						if (Object.op_Implicit((Object)(object)val3))
						{
							val2.set_shader(val3);
						}
					}
				}
			}
		}
	}
}
