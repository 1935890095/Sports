using System;
using XFX.Core.Logging;

namespace XFX.Core.Network
{
	public interface INetwork : IDisposable
	{
		string name { get; }

		float connect_timeout { get; set; }

		float ping_interval { get; set; }

		bool auto_connect { get; set; }

		bool verbose { get; set; }

		ILogger logger { get; set; }

		int recv_buffer_size { get; set; }

		int send_buffer_size { get; set; }

		IPacketSerializer serializer { get; set; }

		Action NetworkConnected { get; set; }

		Action<NetState> NetworkDisconnected { get; set; }

		Action NetworkAutoConnected { get; set; }

		Action<NetState> NetworkConnectFailed { get; set; }

		Action<int> NetworkLag { get; set; }

		bool Connect(string ip, ushort port);

		void Disconnect();

		bool Send(IPacket packet);

		void Breath();
	}
}
