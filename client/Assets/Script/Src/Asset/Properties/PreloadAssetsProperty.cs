using UnityEngine;

namespace XFX.Asset.Properties {

    [CreateAssetMenu]
    public class PreloadAssetsProperty : ScriptableObject, IAssetProperty {

        public string[] assets = new string[0];

        public bool Validate(IAssetValidator validator = null) {
            if (validator != null) return validator.Validate(this);
            return false;
        }
    }
}