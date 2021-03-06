namespace XFX.Core.Network
{
	public enum NetState
	{
		UNAVAILABLE = -1,
		CONNECTING = 0,
		CONNECTED = 1,
		CONNECT_FAILED = 100,
		CONNECT_TIMEOUT = 101,
		DISCONNECT_RECV_0 = 200,
		DISCONNECT_RECV_1 = 201,
		DISCONNECT_SEND_0 = 202,
		DISCONNECT_SEND_1 = 203,
		DISCONNECT_PING = 204,
		DISCONNECT_DISC = 205
	}
}
