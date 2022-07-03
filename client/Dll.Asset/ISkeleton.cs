using UnityEngine;
using XFX.Core.Render;

namespace XFX.Asset
{
	public interface ISkeleton : IRenderObject
	{
		Transform rootbone { get; }

		Animation animation { get; }

		Transform FindBone(string name);

		void SetRootBoneName(string name);

		void AddModel(IModel model);

		void RemoveModel(IModel model);
	}
}
