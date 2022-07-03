using UnityEngine;

namespace XFX.Asset.Attributes
{
	public abstract class DrawerAttribute : PropertyAttribute, IPropertyAttribute
	{
		public string label { get; private set; }

		public string tooltip { get; private set; }

		public DrawerAttribute(string label, string tooltip)
		{
			this.label = label;
			this.tooltip = tooltip;
		}
	}
}
