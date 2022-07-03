namespace XFX.Asset
{
	public interface IActionContext
	{
		IRole[] OnSing(string script);

		IRole[] OnFire(string script);

		IRole[] OnHit(string script);
	}
}
