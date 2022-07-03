using UnityEngine;

namespace XFX.Misc {

    public static class Defines {

        public static class Layer {
            // 0~7 internal layer
            //----------------------------
            public const int Default = 0;
            public const int IgnoreRaycast = 2;
            public const int Water = 4;
            public const int UI = 5;
        }

        public static class Tag {
            public const string UIRoot = "uiroot";
            public const string MapRoot = "maproot";
            public const string SceneCamera = "scenecamera";
        }
    }
}