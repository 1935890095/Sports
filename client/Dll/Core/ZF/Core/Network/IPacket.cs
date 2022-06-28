namespace ZF.Core.Network
{
	public interface IPacket
	{
		int id { get; }

		bool Parse(byte[] buf, int offset, int count);

		int Serialize(byte[] buf, int offset, int count);

		void Process();
	}
}
