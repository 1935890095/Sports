using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using MainModule = UnityEngine.ParticleSystem.MainModule;
using MinMaxCurve = UnityEngine.ParticleSystem.MinMaxCurve;
using ShapeModule = UnityEngine.ParticleSystem.ShapeModule;

namespace XFX.Core.Render
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
				return scale_;
			}
			set
			{
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
				if (gameObject != null && gameObject.transform != null)
				{
					return gameObject.transform.position;
				}
				return position_;
			}
			set
			{
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
				if (gameObject != null && gameObject.transform != null)
				{
					return gameObject.transform.forward;
				}
				return ((Quaternion)rotation_).eulerAngles;
			}
			set
			{
				Quaternion val2 = (rotation = Quaternion.LookRotation(value));
			}
		}

		public Quaternion rotation
		{
			get
			{
				if (gameObject != null && gameObject.transform != null)
				{
					return gameObject.transform.rotation;
				}
				return rotation_;
			}
			set
			{
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
				if (gameObject != null && gameObject.transform != null)
				{
					Quaternion val = gameObject.transform.rotation;
					return ((Quaternion)val).eulerAngles;
				}
				return ((Quaternion)rotation_).eulerAngles;
			}
			set
			{
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
					((Object)gameObject).name = name_;
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
			if (!(gameObject == null))
			{
				ApplyParticleScale(gameObject.transform, fscale);
			}
		}

		private void ApplyParticleScale(Transform go, float fscale)
		{
			fscale = Mathf.Abs(fscale);
			if (!go || Mathf.Approximately(fscale, 1f))
			{
				return;
			}
			ParticleSystem[] componentsInChildren = ((Component)go).GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem val in componentsInChildren)
			{
				MainModule main = val.main;
				MainModule main2 = val.main;
				MinMaxCurve startSizeX = ((MainModule)main2).startSizeX;
				main.startSizeX = (new MinMaxCurve((startSizeX).constant * fscale));
				MainModule main3 = val.main;
				if (main3.startSize3D)
				{
					MainModule main4 = val.main;
					MinMaxCurve startSizeY = ((MainModule)main4).startSizeY;
					main.startSizeY = (new MinMaxCurve((startSizeY).constant * fscale));
					MainModule main5 = val.main;
					MinMaxCurve startSizeZ = ((MainModule)main5).startSizeZ;
					main.startSizeZ = (new MinMaxCurve((startSizeZ).constant * fscale));
				}
				ShapeModule shape = val.shape;
				if (shape.enabled)
				{
					ShapeModule shape2 = val.shape;
					ShapeModule shape3 = val.shape;
					shape2.radius = (((ShapeModule)shape3).radius * fscale);
					ShapeModule shape4 = val.shape;
					shape2.scale = (((ShapeModule)shape4).scale * fscale);
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
						Debug.LogError((object)ex.ToString());
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
						Shader val3 = Shader.Find(((Object)val2.shader).name);
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
