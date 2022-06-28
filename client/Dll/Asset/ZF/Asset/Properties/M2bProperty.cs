using System;
using UnityEngine;

namespace ZF.Asset.Properties
{
	public class M2bProperty : MonoBehaviour, IAssetProperty
	{
		public string[] bones = Array.Empty<string>();

		public M2bProperty()
			: this()
		{
		}

		public bool Validate(IAssetValidator validator)
		{
			if (validator != null)
			{
				return validator.Validate(this);
			}
			SkinnedMeshRenderer component = ((Component)this).get_gameObject().GetComponent<SkinnedMeshRenderer>();
			if ((Object)(object)component == (Object)null)
			{
				return false;
			}
			int num = component.get_bones().Length;
			bones = new string[num];
			for (int i = 0; i < num; i++)
			{
				bones[i] = ((Object)component.get_bones()[i]).get_name();
			}
			Debug.Log((object)("***** m2b count ***** " + num));
			return true;
		}
	}
}
