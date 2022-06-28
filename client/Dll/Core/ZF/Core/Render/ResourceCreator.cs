namespace ZF.Core.Render
{
	public delegate IRenderResource ResourceCreator(string filename, IRenderFactory factory, int priority);
}
