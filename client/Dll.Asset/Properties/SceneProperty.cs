using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using XFX.Asset.Attributes;

namespace XFX.Asset.Properties
{
	public class SceneProperty : DependsProperty, IAssetProperty
	{
		[Description("名称")]
		public string Name = string.Empty;

		public Renderer[] renderers = Array.Empty<Renderer>();

		public bool Validate(IAssetValidator validator)
		{
			if (validator != null)
			{
				return validator.Validate(this);
			}
			UnityEngine.SceneManagement.Scene activeScene = SceneManager.GetActiveScene();
			GameObject[] rootGameObjects = (activeScene).GetRootGameObjects();
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
