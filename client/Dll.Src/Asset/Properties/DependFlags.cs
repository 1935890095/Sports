using System;

namespace XFX.Asset.Properties
{
	[Flags]
	public enum DependFlags
	{
		None = 0x0,
		Shader = 0x1,
		Texture = 0x2,
		Font = 0x4,
		Default = 0x3,
		All = 0xFF
	}
}
