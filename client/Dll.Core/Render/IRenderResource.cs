using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace XFX.Core.Render
{
	public interface IRenderResource
	{
		string name { get; }

		bool complete { get; }

		int priority { get; }

		bool loading { get; }

		Object asset { get; }

		string text { get; }

		IEnumerator Load();

		void Destroy();

		IRenderObject CreateInstance(Type type, IRenderObject parent);

		void RemoveInstance(IRenderObject obj);
	}
}
