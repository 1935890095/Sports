namespace XFX.Game {
    public static class GameEvent {
        public const int UNITY_LEVEL_WAS_LOADED = 1;    // unity内部场景加载完成事件
        public const int UNITY_APPLICATION_QUIT = 2;    // unity内部应用退出事件
        public const int UNITY_APPLICATION_PAUSE = 3;   // unity内部应用暂停事件

        public const int RELOAD = 9;                    // 重新加新Lua

        public const int SCENE_LOAD = 11;               // 加载场景
        public const int SCENE_UNLOAD = 12;             // 卸载场景
        public const int SCENE_UNLOADALL = 13;          // 卸载所有场景

        public const int SEND_PROTO = 14;               // 发送网络消息
        public const int FIRE_PROTO = 15;               // 分发网络消息
        public const int CONNECT = 16;                  // 连接服务器
        public const int DISCONNECT = 17;               // 主动断开连接服务器
        public const int ON_CONNECTED = 18;             // 网络连接成功
        public const int ON_CONNECT_FAILED = 19;        // 网络连接失败
        public const int ON_DISCONNECTED = 20;          // 网络断开连接

    }
}