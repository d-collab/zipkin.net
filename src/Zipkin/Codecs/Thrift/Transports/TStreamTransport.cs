namespace Zipkin.Codecs.Thrift
{
	using System.IO;

	public class TStreamTransport : TTransport
	{
		private readonly Stream _stream;

		public TStreamTransport(Stream stream)
		{
			_stream = stream;
			this.CanSeek = _stream.CanSeek;
		}

		public override int Read(byte[] buf, int off, int len)
		{
			return _stream.Read(buf, off, len);
		}

		public override void Write(byte[] buf, int off, int len)
		{
			_stream.Write(buf, off, len);
		}

		public override void Flush()
		{
			_stream.Flush();
		}

		public override long Seek(long len, SeekOrigin current)
		{
			return _stream.Seek(len, current);
		}

		protected override void Dispose(bool disposing)
		{
			_stream.Dispose();
		}
	}
}