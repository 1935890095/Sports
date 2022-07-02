using System.IO;
using UnityEngine;

namespace ZF.Core.Util
{
	public static class PathExt
	{
		private static string[] path_prefix;

		static PathExt()
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Invalid comparison between Unknown and I4
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Invalid comparison between Unknown and I4
			path_prefix = new string[3];
			RuntimePlatform platform = Application.platform;
			if ((int)platform != 11)
			{
				if ((int)platform == 8)
				{
					path_prefix[0] = Application.persistentDataPath + "/";
					path_prefix[2] = Application.dataPath + "/Raw/";
				}
				else if (Application.isEditor)
				{
					path_prefix[0] = Application.dataPath + "/../main.dir/";
					path_prefix[1] = Application.dataPath + "/";
					path_prefix[2] = Application.streamingAssetsPath + "/";
				}
				else
				{
					path_prefix[0] = Application.dataPath + "/main.dir/";
					path_prefix[2] = Application.streamingAssetsPath + "/";
				}
			}
			else
			{
				path_prefix[0] = Application.persistentDataPath + "/";
				path_prefix[2] = Application.dataPath + "!assets/";
			}
		}

		public static string MakeLoadPath(string name)
		{
			string text = string.Empty;
			for (int i = 0; i < 3; i++)
			{
				if (!string.IsNullOrEmpty(path_prefix[i]))
				{
					text = path_prefix[i] + name;
					if (File.Exists(text))
					{
						break;
					}
				}
			}
			return text;
		}

		public static string MakeWWWPath(string name)
		{
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Expected I4, but got Unknown
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			string text = string.Empty;
			for (num = 0; num < 3; num++)
			{
				if (!string.IsNullOrEmpty(path_prefix[num]))
				{
					text = path_prefix[num] + name;
					if (File.Exists(text))
					{
						break;
					}
				}
			}
			if (num != 2)
			{
				return "file://" + text;
			}
			RuntimePlatform platform = Application.platform;
			switch (platform - 8)
			{
			case RuntimePlatform.OSXEditor:
				return "file://" + text;
			case RuntimePlatform.OSXPlayer:
				return "jar:file://" + text;
			default:
				if ((int)platform != 0)
				{
					return "file:///" + text;
				}
				goto case 0;
			}
		}

		public static string MakeCachePath(string name)
		{
			if (!string.IsNullOrEmpty(path_prefix[0]))
			{
				return path_prefix[0] + name;
			}
			return name;
		}

		public static string ChangeExtension(string path, string extension)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Invalid comparison between Unknown and I4
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Invalid comparison between Unknown and I4
			if ((int)Application.platform == 7 || (int)Application.platform == 2)
			{
				return Path.ChangeExtension(path, extension).Replace("\\", "/");
			}
			return Path.ChangeExtension(path, extension);
		}

		public static string Combine(string path1, string path2)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Invalid comparison between Unknown and I4
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Invalid comparison between Unknown and I4
			if ((int)Application.platform == 7 || (int)Application.platform == 2)
			{
				return Path.Combine(path1, path2).Replace("\\", "/");
			}
			return Path.Combine(path1, path2);
		}

		public static string GetDirectoryName(string path)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Invalid comparison between Unknown and I4
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Invalid comparison between Unknown and I4
			if ((int)Application.platform == 7 || (int)Application.platform == 2)
			{
				return Path.GetDirectoryName(path).Replace("\\", "/");
			}
			return Path.GetDirectoryName(path);
		}

		public static string GetExtension(string path)
		{
			return Path.GetExtension(path);
		}

		public static string GetFileName(string path)
		{
			return Path.GetFileName(path);
		}

		public static string GetFileNameWithoutExtension(string path)
		{
			return Path.GetFileNameWithoutExtension(path);
		}
	}
}
