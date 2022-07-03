using System;
using UnityEditor;
using XFX.Core.Network;
using XFX.Core.Router;

// 待整理
namespace XFX.Game {
    class NetworkMgr : Instance {

        public const int PROTOID_PING = 10000;
        public const int PROTOID_PONG = 10001;
        public const int PROTOID_HANG = 10006;

        private INetwork _network;

        protected override void OnInit() {
            base.OnInit();

            _network = new Network("net", new PacketSerializer(router)) {
                verbose = true,
                connect_timeout = 3,
                NetworkConnected = OnNetworkConnected,
                NetworkConnectFailed = OnNetworkConnectFailed,
                NetworkDisconnected = OnNetworkDisconnected,
                logger = Log.ForkChild("Net")
            };
            _network.recv_buffer_size = 4*1024*1024;
            _network.send_buffer_size = 64*1024;
            router.On<int, byte[]>(GameEvent.SEND_PROTO, OnSend);
            router.On<string, int>(GameEvent.CONNECT, OnConnect);
            router.On(GameEvent.DISCONNECT, OnDisconnect);
            router.On<bool>(GameEvent.UNITY_APPLICATION_PAUSE, OnApplicationPause);
        }

        private void OnApplicationPause(bool status) {
            if (status) {
                OnSend(PROTOID_HANG, new byte[0]);
            }
        }

        private void OnNetworkDisconnected(NetState obj) {
            router.Event(GameEvent.ON_DISCONNECTED);
        }

        private void OnNetworkConnectFailed(NetState obj) {
            router.Event(GameEvent.ON_CONNECT_FAILED);
        }

        private void OnNetworkConnected() {
            router.Event(GameEvent.ON_CONNECTED);
        }

        private void OnConnect(string ip, int port) {
            _network.Disconnect();
            _network.Connect(ip, (ushort) port);
        }

        private void OnDisconnect() {
            _network.Disconnect();
            router.Event(GameEvent.ON_DISCONNECTED);
        }

        private void OnSend(int id, byte[] data) {
            IPacket packet = new Packet(id, this.router, data);
            _network.Send(packet);
        }

        protected override void OnUpdate() {
            base.OnUpdate();
            _network?.Breath();
        }

        protected override void OnDestroy() {
            base.OnDestroy();
            _network.Dispose();
        }
    }

    class Packet : IPacket {

        private readonly IRouter _router;
        private byte[] _data;

        public Packet(int id, IRouter router, byte[] data) {
            this.id = id;
            _router = router;
            _data = data;
        }

        public int id { get; private set; }

        public bool Parse(byte[] buf, int offset, int count) {
            _data = new byte[count];
            Array.Copy(buf, offset, _data, 0, count);
            return true;
        }

        public void Process() {
            _router.Event(GameEvent.FIRE_PROTO, id, _data);
        }

        public int Serialize(byte[] buf, int offset, int count) {
            if (_data == null) {
                Log.Error($"packet serialize error {id}");
                return 0;
            }
            Array.Copy(_data, 0, buf, offset, _data.Length);
            return _data.Length;
        }
    }

    class PacketSerializer : IPacketSerializer {

        private readonly IRouter _router;
        private int _pingTime = 0;

        public PacketSerializer(IRouter router) {
            _router = router;
        }

        public bool CheckPong(IPacket packet, out int time) {
            // if (packet.id == NetworkMgr.PROTOID_PONG) {
            //     time = _pingTime;
            //     return true;
            // }
            time = 0;
            return false;
        }

        public IPacket CreatePing(int time) {
            // _pingTime = (int) (UnityEngine.Time.realtimeSinceStartup * 1000);
            // return new Packet(NetworkMgr.PROTOID_PING, _router, new byte[0]);
            return null;
        }

        public int Parse(byte[] buf, int offset, int count, out IPacket packet) {
            const int headsize = 8;
            packet = null;
            if (4 > count) {
                return 0;
            }
            int size = buf[offset] << 24 | buf[offset + 1] << 16 | buf[offset + 2] << 8 | buf[offset + 3];
            int pid = buf[offset + 4] << 24 | buf[offset + 5] << 16 | buf[offset + 6] << 8 | buf[offset + 7];

            try {
                packet = new Packet(pid, _router, null);
                if (!packet.Parse(buf, offset + headsize, size)) {
                    Log.Error($"parse packet error {pid}");
                    packet = null;
                    return 0;
                }
                return size + headsize;
            } catch (Exception ex) {
                Log.Error($"parse proto error {ex.Message}");
                return 0;
            }
        }

        public int Serialize(IPacket packet, int nonce, byte[] buf, int offset, int count) {
            const int headsize = 8;
            int size = packet.Serialize(buf, offset + headsize, count - headsize);
            if (size >= 0) {
                // size += headsize;

                buf[offset] = (byte) (size >> 24 & 0xff);
                buf[offset + 1] = (byte) (size >> 16 & 0xff);
                buf[offset + 2] = (byte) (size >> 8 & 0xff);
                buf[offset + 3] = (byte) (size & 0xff);

                buf[offset + 4] = (byte) (packet.id >> 24 & 0xff);
                buf[offset + 5] = (byte) (packet.id >> 16 & 0xff);
                buf[offset + 6] = (byte) (packet.id >> 8 & 0xff);
                buf[offset + 7] = (byte) (packet.id & 0xff);
            }
            return size + headsize;
        }
    }
}