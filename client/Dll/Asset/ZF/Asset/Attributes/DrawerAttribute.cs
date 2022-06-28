using UnityEngine;

namespace ZF.Asset.Attributes
{
	public abstract class DrawerAttribute : PropertyAttribute, IPropertyAttribute
	{
		public string label { get; private set; }

		public string tooltip { get; private set; }

		public DrawerAttribute(string label, string tooltip)
			: this()
		{
			this.label = label;
			this.tooltip = tooltip;
		}
	}
}
