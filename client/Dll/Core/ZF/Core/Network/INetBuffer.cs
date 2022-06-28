namespace ZF.Core.Network
{
	public interface INetBuffer
	{
		int ReadAvailable { get; }

		int WriteAvailable { get; }

		bool TryRead(byte[] buffer, int offset, int count);

		bool TryWrite(byte[] buffer, int offset, int count);

		void CommitRead(int count);

		void CommitWrite(int count);
	}
}
