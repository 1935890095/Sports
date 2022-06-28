using ZF.Core.Render;

namespace ZF.Asset
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
