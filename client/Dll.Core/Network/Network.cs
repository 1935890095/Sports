using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using XFX.Core.Logging;
using ILogger = XFX.Core.Logging.ILogger;
using XFX.Core.Util;

namespace XFX.Core.Network
{
	public class Network : INetwork, IDisposable
	{
		private class Net
		{
			private Thread thread;

			private volatile bool run;

			public readonly int id;

			private Network network;

			private Socket socket;

			private bool connect;

			private string ip;

			private ushort port;

			private NetState state;

			private NetBuffer recv_buf;

			private NetBuffer send_buf;

			private int send_count;

			public Net(int id, Network network, int recv_buf_size, int send_buf_size)
			{
				this.id = id;
				this.network = network;
				state = NetState.UNAVAILABLE;
				recv_buf = new NetBuffer(recv_buf_size);
				send_buf = new NetBuffer(send_buf_size);
				send_count = 0;
				run = true;
				thread = new Thread(Run);
				if (network.verbose)
				{
					network.logger.Info("net create, id: {0}", this.id);
				}
			}

			public int GetSendCount()
			{
				return send_count;
			}

			public bool Connect(string ip, ushort port)
			{
				connect = true;
				this.ip = ip;
				this.port = port;
				run = true;
				thread.Start();
				return true;
			}

			public void Disconnect()
			{
				run = false;
			}

			public bool IsConnecting()
			{
				return state == NetState.CONNECTING;
			}

			public bool IsConnected()
			{
				return state == NetState.CONNECTED;
			}

			public bool Send(byte[] buf, int offset, int count)
			{
				if (!IsConnected())
				{
					network.logger.Warn("net send packet on disconnect");
					return false;
				}
				if (send_buf.WriteAvailable < count)
				{
					network.logger.Error("net send buffer is not enough: {0} < {1}", send_buf.WriteAvailable, count);
					OnDisconnect(NetState.DISCONNECT_SEND_1, null);
					return true;
				}
				send_buf.Write(buf, offset, count);
				send_count++;
				return true;
			}

			private void Run()
			{
				while (run)
				{
					if (connect)
					{
						connect = false;
						if (PostConnect(ip, port))
						{
							OnConnected(success: true);
						}
						else
						{
							OnConnected(success: false);
						}
					}
					if (state == NetState.CONNECTED && socket != null && socket.Connected)
					{
						PostRecv();
						PostSend();
					}
					Thread.Sleep(1);
				}
				Release();
			}

			private string GetIPv6(string ip, ushort port)
			{
				return string.Empty;
			}

			private bool PostConnect(string ip, ushort port)
			{
				if (network.verbose)
				{
					network.logger.Info("net connecting, {0}:{1} id: {2}", ip, port, id);
				}
				if (socket != null)
				{
					return false;
				}
				try
				{
					string iPv = GetIPv6(ip, port);
					if (!string.IsNullOrEmpty(iPv))
					{
						socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
						ip = iPv;
					}
					else
					{
						socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					}
					if (!IPAddress.TryParse(ip, out var address))
					{
						address = Dns.GetHostEntry(ip).AddressList[0];
					}
					state = NetState.CONNECTING;
					send_count = 0;
					socket.Connect(address, port);
					socket.Blocking = false;
					int num = (int)socket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer);
					socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, 524288);
					if (network.verbose)
					{
						network.logger.Info("net change socket recv buffer size {0} to {1}, id: {2}", num, 524288, id);
					}
					if (socket.Connected)
					{
						return true;
					}
					socket.Close();
					socket = null;
					return false;
				}
				catch (Exception exception)
				{
					network.logger.Fatal(exception, "net connect exception, id: {0}", id);
					socket.Close();
					socket = null;
					return false;
				}
			}

			private void PostRecv()
			{
				try
				{
					int num = 0;
					while (true)
					{
						if (socket.Available == 0 || !recv_buf.GetWriteAvailableBuffer(out var buffer, out var offset, out var count))
						{
							return;
						}
						num = socket.Receive(buffer, offset, count, SocketFlags.None);
						if (num <= 0)
						{
							break;
						}
						recv_buf.CommitWrite(num);
						network.DoRecv(recv_buf);
					}
					if (num == 0)
					{
						OnDisconnect(NetState.DISCONNECT_RECV_0, null);
					}
					else
					{
						OnDisconnect(NetState.DISCONNECT_RECV_1, null);
					}
				}
				catch (SocketException ex)
				{
					if (ex.SocketErrorCode != SocketError.WouldBlock)
					{
						OnDisconnect(NetState.DISCONNECT_RECV_1, ex);
					}
				}
				catch (Exception exception)
				{
					OnDisconnect(NetState.DISCONNECT_RECV_1, exception);
				}
			}

			private void PostSend()
			{
				try
				{
					int num = 0;
					byte[] buffer;
					int offset;
					int count;
					while (send_buf.GetReadAvailableBuffer(out buffer, out offset, out count))
					{
						num = socket.Send(buffer, offset, count, SocketFlags.None);
						if (num <= 0)
						{
							if (num == 0)
							{
								OnDisconnect(NetState.DISCONNECT_SEND_0, null);
							}
							else
							{
								OnDisconnect(NetState.DISCONNECT_SEND_1, null);
							}
						}
						send_buf.CommitRead(num);
					}
				}
				catch (SocketException ex)
				{
					if (ex.SocketErrorCode != SocketError.WouldBlock)
					{
						OnDisconnect(NetState.DISCONNECT_SEND_1, ex);
					}
				}
				catch (Exception exception)
				{
					OnDisconnect(NetState.DISCONNECT_SEND_1, exception);
				}
			}

			private void Release()
			{
				if (socket != null)
				{
					if (socket.Connected)
					{
						try
						{
							socket.Shutdown(SocketShutdown.Both);
						}
						catch
						{
						}
						finally
						{
							socket.Close();
							socket = null;
						}
					}
					else
					{
						socket = null;
					}
				}
				state = NetState.UNAVAILABLE;
				recv_buf.Clear();
				send_buf.Clear();
				if (network.verbose)
				{
					network.logger.Info("net release, id: {0}", id);
				}
			}

			private void OnConnected(bool success)
			{
				state = (success ? NetState.CONNECTED : NetState.CONNECT_FAILED);
				network.OnNetStateChange(id, state);
				if (network.verbose)
				{
					network.logger.Info("net connect {0}, id: {1}", (!success) ? "failed" : "ok", id);
				}
			}

			private void OnDisconnect(NetState state, Exception exception)
			{
				this.state = state;
				network.OnNetStateChange(id, this.state);
				if (network.verbose)
				{
					if (exception != null)
					{
						network.logger.Info(exception, "net disconnect, state: {0}, id: {1}", state.ToString(), id);
					}
					else
					{
						network.logger.Info("net disconnect, state: {0}, id: {1}", state.ToString(), id);
					}
				}
			}
		}

		private class NetBuffer : INetBuffer
		{
			private const int DEFAULT_SIZE = 4096;

			private readonly int size;

			private byte[] buffer;

			private volatile uint read_position;

			private volatile uint write_position;

			public int ReadAvailable => (int)(write_position - read_position);

			public int WriteAvailable => size - (int)(write_position - read_position);

			public NetBuffer()
				: this(4096)
			{
			}

			public NetBuffer(int size)
			{
				this.size = size;
				buffer = new byte[size];
				read_position = 0u;
				write_position = 0u;
			}

			public void Clear()
			{
				read_position = 0u;
				write_position = 0u;
			}

			public bool TryWrite(byte[] buffer, int offset, int count)
			{
				uint num = write_position;
				int writeAvailable = WriteAvailable;
				if (writeAvailable < count)
				{
					return false;
				}
				int num2 = count;
				int num3 = (int)(num % (uint)size);
				writeAvailable = size - num3;
				writeAvailable = ((writeAvailable < num2) ? writeAvailable : num2);
				Buffer.BlockCopy(buffer, offset, this.buffer, num3, writeAvailable);
				num2 -= writeAvailable;
				if (num2 > 0)
				{
					Buffer.BlockCopy(buffer, offset + writeAvailable, this.buffer, 0, num2);
				}
				return true;
			}

			public bool TryRead(byte[] buffer, int offset, int count)
			{
				uint num = read_position;
				int readAvailable = ReadAvailable;
				if (readAvailable < count)
				{
					return false;
				}
				int num2 = count;
				int num3 = (int)(num % (uint)size);
				readAvailable = size - num3;
				readAvailable = ((readAvailable < num2) ? readAvailable : num2);
				Buffer.BlockCopy(this.buffer, num3, buffer, offset, readAvailable);
				num2 -= readAvailable;
				if (num2 > 0)
				{
					Buffer.BlockCopy(this.buffer, 0, buffer, offset + readAvailable, num2);
				}
				return true;
			}

			public bool GetReadAvailableBuffer(out byte[] buffer, out int offset, out int count)
			{
				buffer = this.buffer;
				offset = (count = 0);
				uint num = read_position;
				int readAvailable = ReadAvailable;
				if (readAvailable <= 0)
				{
					return false;
				}
				offset = (int)(num % (uint)size);
				count = size - offset;
				if (count > readAvailable)
				{
					count = readAvailable;
				}
				return true;
			}

			public bool GetWriteAvailableBuffer(out byte[] buffer, out int offset, out int count)
			{
				buffer = this.buffer;
				count = (offset = 0);
				uint num = write_position;
				int writeAvailable = WriteAvailable;
				if (writeAvailable <= 0)
				{
					return false;
				}
				offset = (int)(num % (uint)size);
				count = size - offset;
				if (count > writeAvailable)
				{
					count = writeAvailable;
				}
				return true;
			}

			public bool Write(byte[] buffer, int offset, int count)
			{
				uint num = write_position;
				int writeAvailable = WriteAvailable;
				if (writeAvailable < count)
				{
					return false;
				}
				int num2 = count;
				int num3 = (int)(num % (uint)size);
				writeAvailable = size - num3;
				writeAvailable = ((writeAvailable < num2) ? writeAvailable : num2);
				Buffer.BlockCopy(buffer, offset, this.buffer, num3, writeAvailable);
				num2 -= writeAvailable;
				if (num2 > 0)
				{
					Buffer.BlockCopy(buffer, offset + writeAvailable, this.buffer, 0, num2);
				}
				Thread.MemoryBarrier();
				write_position = num + (uint)count;
				return true;
			}

			public int Read(byte[] buffer, int offset, int count)
			{
				uint num = read_position;
				int readAvailable = ReadAvailable;
				if (readAvailable < count)
				{
					return 0;
				}
				int num2 = count;
				int num3 = (int)(num % (uint)size);
				readAvailable = size - num3;
				readAvailable = ((readAvailable < num2) ? readAvailable : num2);
				Buffer.BlockCopy(this.buffer, num3, buffer, offset, readAvailable);
				num2 -= readAvailable;
				if (num2 > 0)
				{
					Buffer.BlockCopy(this.buffer, 0, buffer, offset + readAvailable, num2);
				}
				Thread.MemoryBarrier();
				read_position += (uint)count;
				return count;
			}

			public void CommitRead(int count)
			{
				uint num = read_position;
				read_position = num + (uint)count;
			}

			public void CommitWrite(int count)
			{
				uint num = write_position;
				write_position = num + (uint)count;
			}
		}

		private struct NetStateEvent
		{
			public int netid;

			public NetState state;
		}

		private const float DEFAULT_CONNECT_TIMEOUT = 5f;

		private const float DEFAULT_PING_INTERVAL = 3f;

		private const float STATE_WAIT_TIMEOUT = 180f;

		private const float STATE_RECONNECT_TIMEOUT = 30f;

		private const float WAIT_CONNECT_INTERVAL = 30f;

		private const float RECONNECT_INTERVAL = 5f;

		private const int DEFAULT_RECV_BUFFER_SIZE = 65536;

		private const int DEFAULT_SEND_BUFFER_SIZE = 32768;

		private Queue<NetStateEvent> event_queue = new Queue<NetStateEvent>();

		private Net net;

		private string ip = string.Empty;

		private ushort port;

		private int connect_count;

		private float connect_time;

		private const int STATE_START = 0;

		private const int STATE_RECON = 1;

		private const int STATE_WAIT = 2;

		private const int STATE_END = 3;

		private int state;

		private float state_time;

		private FixedQueue<IPacket> packet_queue = new FixedQueue<IPacket>(512);

		private FixedQueue<int> pong_queue = new FixedQueue<int>(32);

		private byte[] send_data;

		private byte[] recv_data;

		private const int PING_COUNT_LOST = 10;

		private int ping_count;

		private float ping_time;

		private int lag_time;

		private ILogger logger_ = NullLogger.Instance;

		private Stopwatch stopwatch = new Stopwatch();

		private float ulimit_ms;

		public string name { get; private set; }

		public float connect_timeout { get; set; }

		public float ping_interval { get; set; }

		public bool auto_connect { get; set; }

		public bool verbose { get; set; }

		public int recv_buffer_size { get; set; }

		public int send_buffer_size { get; set; }

		public IPacketSerializer serializer { get; set; }

		public Action NetworkConnected { get; set; }

		public Action<NetState> NetworkDisconnected { get; set; }

		public Action NetworkAutoConnected { get; set; }

		public Action<NetState> NetworkConnectFailed { get; set; }

		public Action<int> NetworkLag { get; set; }

		public ILogger logger
		{
			get
			{
				return logger_;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("logger can't be null");
				}
				logger_ = value;
			}
		}

		public Network(string name)
			: this(name, null)
		{
		}

		public Network(string name, IPacketSerializer serializer)
		{
			this.name = name;
			this.serializer = serializer;
			auto_connect = false;
			connect_timeout = 5f;
			ping_interval = 3f;
			recv_buffer_size = 65536;
			send_buffer_size = 32768;
			connect_count = 0;
		}

		public bool Connect(string ip, ushort port)
		{
			SafeDestroy(ref net);
			ClearPing();
			if (recv_data == null)
			{
				recv_data = new byte[recv_buffer_size];
			}
			if (send_data == null)
			{
				send_data = new byte[send_buffer_size];
			}
			this.ip = ip;
			this.port = port;
			connect_count++;
			connect_time = Time.realtimeSinceStartup;
			state = 0;
			net = new Net(connect_count, this, recv_buffer_size, send_buffer_size);
			return net.Connect(ip, port);
		}

		public void Disconnect()
		{
			SafeDestroy(ref net);
			packet_queue.Clear();
			pong_queue.Clear();
		}

		public bool IsConnected()
		{
			if (net != null)
			{
				return net.IsConnected();
			}
			return false;
		}

		public bool IsConnecting()
		{
			if (net != null)
			{
				return net.IsConnecting();
			}
			return false;
		}

		public bool Send(IPacket packet)
		{
			if (!IsConnected())
			{
				return false;
			}
			if (serializer == null)
			{
				return false;
			}
			try
			{
				int count = serializer.Serialize(packet, net.GetSendCount(), send_data, 0, send_data.Length);
				net.Send(send_data, 0, count);
			}
			catch (Exception exception)
			{
				logger.Fatal(exception, "send packet error {0}", packet.id);
			}
			return true;
		}

		public void Breath()
		{
			if (ProcessPacket())
			{
				ProcessStateEvent();
			}
			switch (state)
			{
			case 0:
				ProcessPing();
				break;
			case 1:
				ProcessRecon();
				break;
			case 2:
				ProcessWait();
				break;
			}
			CheckConnectTimeout();
		}

		public void Dispose()
		{
			Disconnect();
		}

		private bool Connect()
		{
			SafeDestroy(ref net);
			if (string.IsNullOrEmpty(ip) || port == 0)
			{
				return false;
			}
			ClearPing();
			connect_count++;
			connect_time = Time.realtimeSinceStartup;
			net = new Net(connect_count, this, recv_buffer_size, send_buffer_size);
			return net.Connect(ip, port);
		}

		private void SafeDestroy(ref Net net)
		{
			if (net != null)
			{
				net.Disconnect();
				net = null;
			}
		}

		private void OnNetStateChange(int netid, NetState state)
		{
			NetStateEvent netStateEvent = default(NetStateEvent);
			netStateEvent.netid = netid;
			netStateEvent.state = state;
			NetStateEvent item = netStateEvent;
			lock (event_queue)
			{
				event_queue.Enqueue(item);
			}
		}

		private void ProcessStateEvent()
		{
			lock (event_queue)
			{
				while (event_queue.Count > 0)
				{
					NetStateEvent netStateEvent = event_queue.Dequeue();
					if (net != null && net.id == netStateEvent.netid)
					{
						switch (netStateEvent.state)
						{
						case NetState.CONNECTED:
							OnConnected();
							break;
						case NetState.CONNECT_FAILED:
							OnConnectFail(netStateEvent.state);
							break;
						case NetState.DISCONNECT_RECV_0:
						case NetState.DISCONNECT_RECV_1:
						case NetState.DISCONNECT_SEND_0:
						case NetState.DISCONNECT_SEND_1:
						case NetState.DISCONNECT_DISC:
							OnDisconnect(netStateEvent.state);
							break;
						}
					}
				}
			}
		}

		private void ProcessWait()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			if ((int)Application.internetReachability == 0)
			{
				float realtimeSinceStartup = Time.realtimeSinceStartup;
				if (realtimeSinceStartup - state_time >= 180f)
				{
					if (!IsConnecting())
					{
						OnDisconnect(NetState.CONNECT_TIMEOUT);
					}
				}
				else if (realtimeSinceStartup - connect_time >= 30f && !IsConnecting())
				{
					Connect();
				}
			}
			else
			{
				ChangeState(1);
			}
		}

		private void ProcessRecon()
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			if (realtimeSinceStartup - state_time >= 30f)
			{
				if (!IsConnecting())
				{
					OnDisconnect(NetState.CONNECT_TIMEOUT);
				}
			}
			else if (realtimeSinceStartup - connect_time >= 5f && !IsConnecting())
			{
				Connect();
			}
		}

		private void ChangeState(int state)
		{
			this.state = state;
			state_time = Time.realtimeSinceStartup;
			int num = this.state;
			if ((num == 2 || num == 1) && lag_time != 999999)
			{
				lag_time = 999999;
				if (NetworkLag != null)
				{
					NetworkLag(lag_time);
				}
			}
			if (verbose)
			{
				logger.Info("change state: {0}", state);
			}
		}

		private bool ProcessPacket()
		{
			if (ulimit_ms == 0f)
			{
				ulimit_ms = 800f / (float)Application.targetFrameRate;
			}
			stopwatch.Start();
			int num = 0;
			while (packet_queue.Count != 0)
			{
				IPacket packet = packet_queue.Dequeue();
				try
				{
					num++;
					packet.Process();
				}
				catch (Exception exception)
				{
					logger.Fatal(exception, "process packet error, id: {0}", packet.id);
				}
				if ((float)stopwatch.ElapsedMilliseconds > ulimit_ms)
				{
					break;
				}
			}
			stopwatch.Reset();
			return packet_queue.Count == 0;
		}

		private bool DoRecv(INetBuffer buffer)
		{
			int num = buffer.ReadAvailable;
			if (num <= 0)
			{
				return false;
			}
			if (num > recv_data.Length)
			{
				num = recv_data.Length;
			}
			if (!buffer.TryRead(recv_data, 0, num))
			{
				throw new Exception("DoRecv read packet head error");
			}
			if (serializer == null)
			{
				return false;
			}
			int num2 = 0;
			bool result = false;
			while (true)
			{
				IPacket packet = null;
				int num3 = serializer.Parse(recv_data, num2, num, out packet);
				if (num3 <= 0)
				{
					break;
				}
				buffer.CommitRead(num3);
				num2 += num3;
				num -= num3;
				if (packet != null)
				{
					result = true;
					if (serializer.CheckPong(packet, out var time))
					{
						pong_queue.Enqueue(time);
					}
					else if (!packet_queue.Enqueue(packet))
					{
						throw new Exception("DoRecv packet queue is full");
					}
				}
			}
			return result;
		}

		private void OnConnected()
		{
			if (state == 2 || state == 1)
			{
				if (NetworkAutoConnected != null)
				{
					NetworkAutoConnected();
				}
				if (verbose)
				{
					logger.Info("reconnect ok, connect count {0}", connect_count);
				}
			}
			ChangeState(0);
			ping_time = Time.realtimeSinceStartup;
			if (NetworkConnected != null)
			{
				NetworkConnected();
			}
			if (verbose)
			{
				logger.Info("connect ok");
			}
		}

		private void OnConnectFail(NetState ns)
		{
			Disconnect();
			switch (state)
			{
			case 2:
				if (verbose)
				{
					logger.Info("connect failed in wait");
				}
				return;
			case 1:
				if (verbose)
				{
					logger.Info("connect failed in reconnect");
				}
				return;
			}
			if (verbose)
			{
				logger.Info("connect failed, state: {0}", ns.ToString());
			}
			if (NetworkConnectFailed != null)
			{
				NetworkConnectFailed(ns);
			}
		}

		private void OnDisconnect(NetState ns)
		{
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			ChangeState(3);
			switch (ns)
			{
			case NetState.DISCONNECT_RECV_1:
			case NetState.DISCONNECT_SEND_1:
				if (auto_connect)
				{
					ChangeState(2);
				}
				break;
			case NetState.DISCONNECT_PING:
			case NetState.DISCONNECT_DISC:
				if (auto_connect)
				{
					if ((int)Application.internetReachability == 0)
					{
						ChangeState(2);
					}
					else
					{
						ChangeState(1);
					}
				}
				break;
			}
			switch (state)
			{
			case 2:
				if (verbose)
				{
					logger.Info("disconnect, change to wait state");
				}
				break;
			case 1:
				if (verbose)
				{
					logger.Info("disconnect, change to recon state");
				}
				break;
			case 3:
				if (verbose)
				{
					logger.Info("disconnect, change to end state");
				}
				if (NetworkDisconnected != null)
				{
					NetworkDisconnected(ns);
				}
				break;
			}
			Disconnect();
		}

		private void CheckConnectTimeout()
		{
			if (net != null && net.IsConnecting() && connect_time > 0f)
			{
				float realtimeSinceStartup = Time.realtimeSinceStartup;
				if (realtimeSinceStartup - connect_time >= connect_timeout)
				{
					OnConnectFail(NetState.CONNECT_TIMEOUT);
				}
			}
		}

		private void ProcessPing()
		{
			if (!IsConnected())
			{
				return;
			}
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			int num = (int)(realtimeSinceStartup * 1000f);
			int num2 = int.MaxValue;
			while (pong_queue.Count > 0)
			{
				int num3 = pong_queue.Dequeue();
				int num4 = num - num3;
				if (num4 < num2)
				{
					num2 = num4;
				}
			}
			if (num2 < int.MaxValue && num2 > 0)
			{
				lag_time = num2 / 2;
				if (NetworkLag != null)
				{
					NetworkLag(lag_time);
				}
				ping_count = 0;
			}
			if (!(realtimeSinceStartup - ping_time >= ping_interval))
			{
				return;
			}
			ping_time = realtimeSinceStartup;
			if (ping_count >= 10)
			{
				OnDisconnect(NetState.DISCONNECT_PING);
				return;
			}
			if (ping_count > 0)
			{
				lag_time = (int)(ping_interval * 1000f) * ping_count;
				if (NetworkLag != null)
				{
					NetworkLag(lag_time);
				}
			}
			Ping(num);
		}

		private void Ping(int time)
		{
			if (serializer != null)
			{
				IPacket packet = serializer.CreatePing(time);
				if (packet != null)
				{
					Send(packet);
					ping_count++;
				}
			}
		}

		private void ClearPing()
		{
			ping_time = 0f;
			ping_count = 0;
			lag_time = 0;
		}
	}
}
