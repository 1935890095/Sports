using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using ZF.Asset.Properties;

public static class ResourceCheck
{
	private static readonly HashSet<string> IgnoreTextures = new HashSet<string>()
	{
		"test.tex",
		"rateroom_board1.tex",
		"rateroom_board2.tex",
		"rateroom_board3.tex",
		"rateroom_board4.tex",
		"yx_logo_en.tex",
		"yx_logo_ru.tex",
		"logo_en.tex",
		"logo_ru.tex",
		"xs_congratulation_en.tex",
		"xs_congratulation_ru.tex",
		"doubleroom_vs_en.tex",
		"doubleroom_vs_ru.tex",
		"hengtu_en.tex",
		"hengtu_ru.tex",
		"xs_congratulation_en.tex",
		"xs_congratulation_ru.tex",
		"js_0.tex",
		"js_1.tex",
		"js_2.tex",
		"js_3.tex",
		"js_4.tex",
		"js_5.tex",
		"js_6.tex",
		"js_7.tex",
		"head_ty_001.tex",
		"head_ty_002.tex",
		"head_ty_003.tex",
		"head_ty_004.tex",
		"head_ty_005.tex",
		"head_ty_006.tex",
		"head_ty_007.tex",
		"js_dikuang.tex",
	};

	private static readonly HashSet<string> IgnoreView = new HashSet<string>()
	{
		"LeaderboardView",
		"ExplainRuleView",
		"MissionAwardView",
		"PlayView",
		"RuleItem",
		"CDKEYAwardItem",
		"CDKEYAwardView",
		"InviteView",
		"MailView",
		"NoviceRewardView",
		"NoticeView",
		"ShopView",
		"LoginView",
		"SettingView",
	};


	[MenuItem("ZF/UI/检查Images", false)]
	public static void CheckImages()
	{
		StringBuilder sb = new StringBuilder();
		StringBuilder sb2 = new StringBuilder();
		foreach (var asset in Selection.objects)
		{
			GameObject go = asset as GameObject;
			if (go == null)
			{
				continue;
			}

			if (IgnoreView.Contains(go.name))
			{
				continue;
			}

			var images = go.GetComponentsInChildren<Image>();
			if (images == null || images.Length <= 0)
			{
				continue;
			}

			foreach (var image in images)
			{
				if (image.sprite == null)
				{
					sb.AppendLine(GetNodePath(image.gameObject));
					Debug.LogError($"{go.name} image {GetNodePath(image.gameObject)} sprite is null");
					continue;
				}
				string path = AssetDatabase.GetAssetPath(image.sprite);
				if (path.Contains("Texture"))
				{
					sb2.AppendLine(GetNodePath(image.gameObject));
					Debug.LogError($"{go.name} image {GetNodePath(image.gameObject)} use texture as sprite");
				}
			}
		}

		Debug.LogError($"======= image is null \n{sb}");
		Debug.LogError($"======= image use texture \n{sb2}");
	}

	[MenuItem("ZF/UI/检查未使用的Textures", false)]
	public static void CheckTextures()
	{
		HashSet<string> names = new HashSet<string>();
		foreach (var asset in Selection.objects)
		{
			GameObject go = asset as GameObject;
			if (go == null)
			{
				continue;
			}
			var property = go.GetComponent<ViewProperty>();
			if (property == null)
			{
				continue;
			}
			foreach (var dep in property.dependencies)
			{
				if (dep.path.Contains("res/ui/tex"))
				{
					names.Add(Path.GetFileName(dep.path));
				}
			}
		}

		StringBuilder sb = new StringBuilder();
		string texDir = $"{Application.dataPath}/StreamingAssets/res/ui/tex";
		string[] files = Directory.GetFiles(texDir, "*.tex");
		foreach (var file in files)
		{
			var name = Path.GetFileName(file);
			if (IgnoreTextures.Contains(name))
			{
				continue;
			}
			if (!names.Contains(name))
			{
				sb.AppendLine(name);
				Debug.LogError($"The texture {name} is not in use");
			}
		}
		Debug.LogError(sb.ToString());
	}

	private static string GetNodePath(GameObject go)
	{
		if (go == null)
		{
			return "";
		}
		string path = go.name;
		Transform parent = go.transform.parent;
		while (parent != null)
		{
			path = $"{parent.name}/{path}";
			parent = parent.parent;
		}
		return path;
	}
}