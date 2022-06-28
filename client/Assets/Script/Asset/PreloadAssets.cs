using System.Collections;
using ZF.Asset.Properties;
using ZF.Core.Render;

namespace ZF.Asset
{
	public interface IPreloadAssets : IRenderObject, IEnumerator
	{
		bool isDone { get; }
		float progress { get; }
	}

	public class PreloadAssets : RenderObject, IPreloadAssets
	{
		private PreloadAssetsProperty property;
		private float progress_ = 0f;
		protected override IEnumerator OnCreateAs(IRenderResource resource)
		{
			this.property = resource.asset as PreloadAssetsProperty;
			if (this.property == null || this.property.assets == null || this.property.assets.Length <= 0) {
				yield break;
			}
			yield return null;
			int n = 0, num = this.property.assets.Length;
			if (this.property != null && this.property.assets.Length > 0)
			{
				this.destroyChildrenOnDestroy = true; // important
				for (int i = 0; i < this.property.assets.Length; ++i)
				{
					var dep = RenderInstance.Create<Empty>(this.property.assets[i], this);
					dep.onComplete = (_) => { ++n; };
				}
				while (n < num)
				{
					progress_ = (float)n / num;
					yield return null;
				}
			}
			else
			{
				progress_ = 1f;
			}
		}

		object IEnumerator.Current { get { return (object)null; } }
		bool IEnumerator.MoveNext() { return !this.complete; }
		void IEnumerator.Reset() { }

		bool IPreloadAssets.isDone { get { return this.complete; } }

		float IPreloadAssets.progress { get { return this.progress_; } }
	}
}