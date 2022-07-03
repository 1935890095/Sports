using System;
using UnityEditor;

public class BuildConfig
{
	public bool Export { get; set; }
	public string OutPath { get; set; }
	public string Version { get; set; }
	public int VersionCode { get; set; }
}

public class Build
{
	public static void BuildAndroid()
	{
		var args = Environment.GetCommandLineArgs();
		BuildConfig config = Newtonsoft.Json.JsonConvert.DeserializeObject<BuildConfig>(args[args.Length - 1]);
		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
		EditorUserBuildSettings.exportAsGoogleAndroidProject = config.Export;
		PlayerSettings.bundleVersion = config.Version;
		PlayerSettings.Android.bundleVersionCode = config.VersionCode;
		PlayerSettings.Android.useCustomKeystore = false;

		// build lua 资源
		XFX.Misc.Editor.Menu.Build.BuildLua();
		XFX.Misc.Editor.Menu.Build.BuildLuaRes();

		// TODO 其他设置项
		BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, config.OutPath, BuildTarget.Android, BuildOptions.None);
	}
}