
using XLua;

namespace XFX.Game {
    public partial class LuaApi {
        [LuaCallCSharp]
        public static class Net {
            public static void Send(int pid, byte[] data) {
                game.router.Event(GameEvent.SEND_PROTO, pid, data);
            }

            public static void Connect(string ip, int port) {
                game.router.Event(GameEvent.CONNECT, ip, port);
            }

            public static void Disconnect() {
                game.router.Event(GameEvent.DISCONNECT);
            }
        }
    }
}