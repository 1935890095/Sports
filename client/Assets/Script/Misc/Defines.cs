/********************************************************
    id: Defines.cs
    desc: 游戏中一些常量定义
    author: figo
    date: 2018/09/20 12:19:00

    Copyright (C) 2018 zwwx Ltd. All rights reserved.
*********************************************************/
using UnityEngine;

namespace ZF.Misc {

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