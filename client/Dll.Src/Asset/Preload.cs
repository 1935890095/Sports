using System.Collections;
using UnityEngine;
using XFX.Asset.Properties;
using XFX.Core.Render;

namespace XFX.Asset
{
	public class Preload : RenderObject, IPreload, IEnumerator
	{
		private PreloadProperty property;

		private float progress_;

		object IEnumerator.Current => null;

		bool IPreload.isDone => base.complete;

		float IPreload.progress => progress_;

		protected override IEnumerator OnCreateAs(IRenderResource resource)
		{
			property = resource.asset as PreloadProperty;
			yield return null;
			int i = 0;
			int num = property.shaders.Length;
			if ((Object)(object)property != (Object)null && property.shaders.Length > 0)
			{
				base.destroyChildrenOnDestroy = true;
				for (int j = 0; j < property.shaders.Length; j++)
				{
					Empty empty = RenderInstance.Create<Empty>(property.shaders[j], this, 0, string.Empty);
					empty.onComplete = delegate
					{
						i++;
					};
				}
				while (i < num)
				{
					progress_ = (float)i / (float)num;
					yield return null;
				}
			}
			else
			{
				progress_ = 1f;
			}
		}

		bool IEnumerator.MoveNext()
		{
			return !base.complete;
		}

		void IEnumerator.Reset()
		{
		}
	}
}
