using System.IO;
using UnityEngine;

namespace XFX.Core.Util
{
	public static class PathExt
	{
		private static string[] path_prefix;

		static PathExt()
		{
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
			default:
				if ((int)platform != 0)
				{
					return "file:///" + text;
				}
				goto case 0;
			case 0:
				return "file://" + text;
			case RuntimePlatform.OSXPlayer:
				return "jar:file://" + text;
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
			if ((int)Application.platform == 7 || (int)Application.platform == 2)
			{
				return Path.ChangeExtension(path, extension).Replace("\\", "/");
			}
			return Path.ChangeExtension(path, extension);
		}

		public static string Combine(string path1, string path2)
		{
			if ((int)Application.platform == 7 || (int)Application.platform == 2)
			{
				return Path.Combine(path1, path2).Replace("\\", "/");
			}
			return Path.Combine(path1, path2);
		}

		public static string GetDirectoryName(string path)
		{
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
