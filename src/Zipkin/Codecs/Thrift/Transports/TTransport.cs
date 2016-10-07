namespace Zipkin.Codecs.Thrift
{
	using System;
	using System.IO;

	public abstract class TTransport : IDisposable
	{
		public abstract int Read(byte[] buffer, int offset, int count);

		public abstract void Write(byte[] buffer, int offset, int count);

		public void ReadAll(byte[] buffer, int offset, int count)
		{
			var consumed = 0;

			while (consumed < count)
			{
				consumed += Read(buffer, offset + consumed, count - consumed);
			}
		}

		public virtual void Flush()
		{
		}

		public bool CanSeek { get; protected set; }

		public virtual long Seek(long len, SeekOrigin current)
		{
			throw new NotImplementedException();
		}

		// IDisposable
		protected abstract void Dispose(bool disposing);

		public void Dispose()
		{
			// Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}