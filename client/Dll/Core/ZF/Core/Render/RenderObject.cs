using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

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

		private Vector3 scale_ = Vector3.one;

		private Vector3 position_ = Vector3.zero;

		private Quaternion rotation_ = Quaternion.identity;

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
				if (value && gameObject && gameObject.transform.parent == null)
				{
					Object.DontDestroyOnLoad(gameObject);
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
				if (!string.IsNullOrEmpty(value) && gameObject)
				{
					gameObject.name = value;
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
				if (gameObject != null)
				{
					gameObject.tag = tag_;
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
				if (gameObject != null && gameObject.transform != null)
				{
					gameObject.transform.localScale = value;
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
				if (gameObject != null && gameObject.transform != null)
				{
					return gameObject.transform.position;
				}
				return position_;
			}
			set
			{
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_0039: Unknown result type (might be due to invalid IL or missing references)
				//IL_003a: Unknown result type (might be due to invalid IL or missing references)
				if (gameObject != null && gameObject.transform != null)
				{
					gameObject.transform.localPosition = value;
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
				if (gameObject != null && gameObject.transform != null)
				{
					return gameObject.transform.forward;
				}
				return rotation_.eulerAngles;
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
				if (gameObject != null && gameObject.transform != null)
				{
					return gameObject.transform.rotation;
				}
				return rotation_;
			}
			set
			{
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_0039: Unknown result type (might be due to invalid IL or missing references)
				//IL_003a: Unknown result type (might be due to invalid IL or missing references)
				if (gameObject != null && gameObject.transform != null)
				{
					gameObject.transform.localRotation = value;
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
				if (gameObject != null && gameObject.transform != null)
				{
					Quaternion val = gameObject.transform.rotation;
					return val.eulerAngles;
				}
				return rotation_.eulerAngles;
			}
			set
			{
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_0033: Unknown result type (might be due to invalid IL or missing references)
				//IL_003e: Unknown result type (might be due to invalid IL or missing references)
				//IL_003f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				if (gameObject != null && gameObject.transform != null)
				{
					gameObject.transform.localRotation = Quaternion.Euler(value);
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
			if (gameObject != null && child.gameObject != null)
			{
				child.gameObject.transform.SetParent(gameObject.transform);
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
				if (gameObject != null && child.gameObject != null && gameObject.transform == child.gameObject.transform.parent)
				{
					child.gameObject.transform.SetParent((Transform)null);
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
			if (RenderFactory.Settings.ReplaceShaderInEditorMode && Application.isEditor)
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
			if (gameObject)
			{
				if (dontDestroyOnLoad)
				{
					Object.DontDestroyOnLoad(gameObject);
				}
				if (!string.IsNullOrEmpty(name_))
				{
					gameObject.name = name_;
				}
				if (!string.IsNullOrEmpty(tag_))
				{
					gameObject.tag = tag_;
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
						Debug.LogError(ex.ToString());
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
							Debug.LogError(ex.ToString());
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
						Debug.LogError(ex2.ToString());
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
				Debug.LogError(ex3.ToString());
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
					Debug.LogError(ex.ToString());
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
							Debug.LogError(ex2.ToString());
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
						Debug.LogError(ex3.ToString());
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
						Debug.LogError(ex.ToString());
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
						Debug.LogError(ex2.ToString());
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
						Debug.LogError(ex.ToString());
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
						Debug.LogError(ex2.ToString());
					}
				}
				num2++;
			}
		}

		public void ApplyParticleScale(float fscale)
		{
			if (!(gameObject == null))
			{
				ApplyParticleScale(gameObject.transform, fscale);
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
			if (!(Object)(go) || Mathf.Approximately(fscale, 1f))
			{
				return;
			}
			ParticleSystem[] componentsInChildren = ((Component)go).GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem val in componentsInChildren)
			{
				ParticleSystem.MainModule main = val.main;
				ParticleSystem.MainModule main2 = val.main;
				ParticleSystem.MinMaxCurve startSizeX = ((ParticleSystem.MainModule)main2).startSizeX;
				main.startSizeX = new ParticleSystem.MinMaxCurve(startSizeX.constant * fscale);
				ParticleSystem.MainModule main3 = val.main;
				if (((ParticleSystem.MainModule)main3).startSize3D)
				{
					ParticleSystem.MainModule main4 = val.main;
					ParticleSystem.MinMaxCurve startSizeY = ((ParticleSystem.MainModule)main4).startSizeY;
					main.startSizeY = (new ParticleSystem.MinMaxCurve(((ParticleSystem.MinMaxCurve)startSizeY).constant * fscale));
					ParticleSystem.MainModule main5 = val.main;
					ParticleSystem.MinMaxCurve startSizeZ = ((ParticleSystem.MainModule)main5).startSizeZ;
					main.startSizeZ = (new ParticleSystem.MinMaxCurve(((ParticleSystem.MinMaxCurve)startSizeZ).constant * fscale));
				}
				ParticleSystem.ShapeModule shape = val.shape;
				if (((ParticleSystem.ShapeModule)shape).enabled)
				{
					ParticleSystem.ShapeModule shape2 = val.shape;
					ParticleSystem.ShapeModule shape3 = val.shape;
					shape2.radius = (((ParticleSystem.ShapeModule)shape3).radius * fscale);
					ParticleSystem.ShapeModule shape4 = val.shape;
					shape2.scale = (((ParticleSystem.ShapeModule)shape4).scale * fscale);
				}
				((Component)val).transform.localScale = Vector3.one;
			}
		}

		protected static void DestroyObject(GameObject go)
		{
			if (Application.isEditor)
			{
				Object.DestroyImmediate(go, true);
			}
			else
			{
				Object.Destroy(go);
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
			if (gameObject != null)
			{
				gameObject.transform.localScale = scale_;
				gameObject.transform.localPosition = position_;
				gameObject.transform.localRotation = rotation_;
			}
			int num = 0;
			while (children != null && num < children.Count)
			{
				IRenderObject renderObject = children[num];
				if (renderObject != null && gameObject != null && renderObject.gameObject != null && gameObject.transform == renderObject.gameObject.transform.parent)
				{
					renderObject.gameObject.transform.SetParent(gameObject.transform);
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
			if (gameObject != null)
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
						Debug.LogError(ex.ToString());
					}
				}
				num++;
			}
		}

		private void _ApplyParent()
		{
			if (gameObject == null)
			{
				return;
			}
			if (parent_ == null || parent_.gameObject == null)
			{
				if (dontDestroyOnLoad)
				{
					Object.DontDestroyOnLoad(gameObject);
				}
			}
			else
			{
				gameObject.transform.SetParent(parent_.gameObject.transform);
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
			if (renderers == null && gameObject)
			{
				renderers = gameObject.GetComponentsInChildren<Renderer>(true);
			}
			if (renderers != null)
			{
				for (int i = 0; i < renderers.Length; i++)
				{
					Renderer val = renderers[i];
					if (val != null)
					{
						((Component)val).gameObject.layer = layer_;
					}
				}
			}
			if (gameObject)
			{
				gameObject.layer = layer_;
			}
		}

		private void _ApplyOrder()
		{
			if (order_ == 0)
			{
				return;
			}
			if (renderers == null && gameObject)
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
				if (val != null)
				{
					val.sortingOrder = order_;
				}
			}
		}

		private void _ApplySortingLayer()
		{
			if (sortingLayer_ == 0)
			{
				return;
			}
			if (renderers == null && gameObject)
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
				if (val != null)
				{
					val.sortingLayerID = sortingLayer_;
				}
			}
		}

		private void _ApplyVisible()
		{
			if (renderers == null && gameObject)
			{
				renderers = gameObject.GetComponentsInChildren<Renderer>(true);
			}
			if (renderers != null)
			{
				for (int i = 0; i < renderers.Length; i++)
				{
					if (renderers[i].enabled != visible_)
					{
						renderers[i].enabled = visible_;
					}
				}
			}
			OnVisible(visible_);
		}

		private void ReplaceShader()
		{
			if (renderers == null && gameObject)
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
				Material[] sharedMaterials = val.sharedMaterials;
				foreach (Material val2 in sharedMaterials)
				{
					if (val2 && val2.shader)
					{
						Shader val3 = Shader.Find(val2.shader.name);
						if (val3)
						{
							val2.shader = val3;
						}
					}
				}
			}
		}
	}
}
