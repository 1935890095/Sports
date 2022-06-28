using System;
using UnityEngine;

namespace ZF.Core.Render
{
	public interface IRenderObject
	{
		string name { get; set; }

		bool complete { get; }

		bool destroyed { get; }

		GameObject gameObject { get; }

		bool dontDestroyOnLoad { get; set; }

		Action<IRenderObject> onComplete { set; }

		int layer { get; set; }

		int order { get; set; }

		int sortingLayer { get; set; }

		string tag { get; set; }

		Vector3 scale { get; set; }

		Vector3 position { get; set; }

		Vector3 forward { get; set; }

		Quaternion rotation { get; set; }

		Vector3 euler { get; set; }

		bool active { get; set; }

		bool visible { get; set; }

		IRenderObject parent { get; set; }

		bool destroyChildrenOnDestroy { get; set; }

		bool oncmd { get; set; }

		T AddComponent<T>() where T : IRenderComponent;

		void RemoveComponent<T>() where T : IRenderComponent;

		T GetComponent<T>() where T : IRenderComponent;

		IRenderComponent AddComponent(Type type);

		void RemoveComponent(Type type);

		IRenderComponent GetComponent(Type type);

		void Create(IRenderResource resource);

		void Destroy();

		void AddChild(IRenderObject child);

		void RemoveChild(IRenderObject child);

		void Update();

		void LateUpdate();

		void DestroyChildren();

		void Command(string cmd, params object[] args);
	}
}
