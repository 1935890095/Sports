namespace ZF.Core.Network
{
	public interface IPacketSerializer
	{
		int Serialize(IPacket packet, int nonce, byte[] buf, int offset, int count);

		int Parse(byte[] buf, int offset, int count, out IPacket packet);

		IPacket CreatePing(int time);

		bool CheckPong(IPacket packet, out int time);
	}
}
