namespace Zipkin.Codecs.Thrift
{
	using System.IO;

	public class TStreamTransport : TTransport
	{
		private readonly Stream _stream;

		public TStreamTransport(Stream stream)
		{
			_stream = stream;
		}

		public override int Read(byte[] buf, int off, int len)
		{
			return _stream.Read(buf, off, len);
		}

		public override void Write(byte[] buf, int off, int len)
		{
			_stream.Write(buf, off, len);
		}

		protected override void Dispose(bool disposing)
		{
			_stream.Dispose();
		}
	}
}