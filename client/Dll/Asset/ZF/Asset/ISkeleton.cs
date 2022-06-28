using UnityEngine;
using ZF.Core.Render;

namespace ZF.Asset
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
