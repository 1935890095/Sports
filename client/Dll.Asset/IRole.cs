using UnityEngine;
using XFX.Core.Render;

namespace XFX.Asset
{
	public interface IRole : IRenderObject
	{
		Bounds bounds { get; }

		void Play(string name, float speed = 1f, string parameter = "");

		bool IsPlaying(string name);

		void Attach(IRenderObject robj, string point = "");

		void Detach(IRenderObject robj, string point = "");
	}
}
