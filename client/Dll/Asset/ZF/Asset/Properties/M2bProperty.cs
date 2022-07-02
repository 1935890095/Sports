using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ZF.Asset.Properties
{
	public class M2bProperty : MonoBehaviour, IAssetProperty
	{
		public string[] bones = Array.Empty<string>();

		public M2bProperty()
		{
		}

		public bool Validate(IAssetValidator validator)
		{
			if (validator != null)
			{
				return validator.Validate(this);
			}
			SkinnedMeshRenderer component = ((Component)this).gameObject.GetComponent<SkinnedMeshRenderer>();
			if ((Object)(object)component == (Object)null)
			{
				return false;
			}
			int num = component.bones.Length;
			bones = new string[num];
			for (int i = 0; i < num; i++)
			{
				bones[i] = ((Object)component.bones[i]).name;
			}
			Debug.Log((object)("***** m2b count ***** " + num));
			return true;
		}
	}
}
