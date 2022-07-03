namespace XFX.Core.Instance
{
	public interface IInstanceManager : IInstance
	{
		bool RemoveInstance(IInstance obj, string name);
	}
}
