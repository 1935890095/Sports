using UnityEngine;
using XFX.Asset.Properties;
using XFX.Core.Render;

namespace XFX.Asset
{
	public class View : Depends, IView
	{
		protected ViewProperty property;

		public bool isShow => base.active;

		protected override void OnCreate(IRenderResource resource)
		{
			if (!(resource.asset is GameObject))
			{
				Debug.LogError((object)"empty view resource");
				Destroy();
			}
			else
			{
				Object obj = Object.Instantiate(resource.asset);
				base.gameObject = (GameObject)(object)((obj is GameObject) ? obj : null);
				property = base.gameObject.GetComponent<ViewProperty>();
			}
		}

		protected override void OnDestroy()
		{
			if ((Object)(object)base.gameObject != (Object)null)
			{
				Object.Destroy((Object)(object)base.gameObject);
				base.gameObject = null;
			}
		}

		public void Show()
		{
			base.active = true;
		}

		public void Hide()
		{
			base.active = false;
		}

		void IView.Destroy()
		{
			Destroy();
		}
	}
}
