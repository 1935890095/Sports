using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZF.Asset.Attributes;

namespace ZF.Asset.Properties
{
	public class SceneProperty : DependsProperty, IAssetProperty
	{
		[Description("名称")]
		public string Name = string.Empty;

		public Renderer[] renderers = Array.Empty<Renderer>();

		public bool Validate(IAssetValidator validator)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			if (validator != null)
			{
				return validator.Validate(this);
			}
			Scene activeScene = SceneManager.GetActiveScene();
			GameObject[] rootGameObjects = ((Scene)(ref activeScene)).GetRootGameObjects();
			List<Renderer> renderers = new List<Renderer>();
			Array.ForEach(rootGameObjects, delegate(GameObject root)
			{
				renderers.AddRange(root.GetComponentsInChildren<Renderer>(true));
			});
			this.renderers = renderers.ToArray();
			Collect(rootGameObjects, DependFlags.Shader);
			return true;
		}
	}
}
