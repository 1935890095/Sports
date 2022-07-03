using XFX.Core.Render;

namespace XFX.Asset
{
	public class Text : RenderObject, IText, IRenderObject
	{
		public string text { get; private set; }

		protected override void OnCreate(IRenderResource resource)
		{
			text = resource.text;
		}
	}
}
