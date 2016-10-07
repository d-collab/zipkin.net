namespace Zipkin.Codecs.Thrift
{
	using System.IO;
	using System.Runtime.CompilerServices;
	using TypeSystem;


	public class TFramedTransport : TTransport
	{
		private const int HeaderSize = 4;
		private readonly byte[] _headerBuf = new byte[HeaderSize];
		private readonly byte[] _tempFrameSizeBuf = new byte[HeaderSize];

		private readonly MemoryStream _writeBuffer = new MemoryStream(1024);
		private readonly MemoryStream _readBuffer = new MemoryStream(1024);
		private readonly TTransport _innerTransport;


		public TFramedTransport(TTransport innerTransport)
		{
			_innerTransport = innerTransport;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			// if (!IsOpen)
			//		throw new TTransportException(TTransportException.ExceptionType.NotOpen);

			int got = _readBuffer.Read(buffer, offset, count);

			if (got != 0)
				return got;

			ReadNextFrame();

			return _readBuffer.Read(buffer, offset, count);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			// if (!IsOpen)
			//		throw new TTransportException(TTransportException.ExceptionType.NotOpen);

			if (_writeBuffer.Length + count > int.MaxValue)
			{
				Flush();
			}

			_writeBuffer.Write(buffer, offset, count);
		}

		public override void Flush()
		{
			// if (!IsOpen)
			//		throw new TTransportException(TTransportException.ExceptionType.NotOpen);

			byte[] buf = _writeBuffer.GetBuffer();
			int frameSize = (int) _writeBuffer.Position;
//			int data_len = len - HeaderSize;
//			if (data_len < 0)
//				throw new System.InvalidOperationException(); // logic error actually

			WriteFrameSizeToInnerTransport(frameSize);
			
			// Send the entire message at once
			_innerTransport.Write(buf, 0, frameSize);

			_innerTransport.Flush();
		}

		protected override void Dispose(bool disposing)
		{
		}

		private void ReadNextFrame()
		{
			_innerTransport.ReadAll(_headerBuf, 0, HeaderSize);
			int size = DecodeFrameSize();

			// _readBuffer.SetLength(size);
			// _readBuffer.Seek(0, SeekOrigin.Begin);
			_readBuffer.Position = 0;

			byte[] buff = _readBuffer.GetBuffer();
			_innerTransport.ReadAll(buff, 0, size);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int DecodeFrameSize()
		{
			return
				((_headerBuf[0] & 0xff) << 24) |
				((_headerBuf[1] & 0xff) << 16) |
				((_headerBuf[2] & 0xff) << 8) |
				((_headerBuf[3] & 0xff));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void WriteFrameSizeToInnerTransport(int frameSize)
		{
			_tempFrameSizeBuf[0] = (byte)(0xff & (frameSize >> 24));
			_tempFrameSizeBuf[1] = (byte)(0xff & (frameSize >> 16));
			_tempFrameSizeBuf[2] = (byte)(0xff & (frameSize >> 8));
			_tempFrameSizeBuf[3] = (byte)(0xff & (frameSize));

			_innerTransport.Write(_tempFrameSizeBuf, 0, 4);
		}
	}
}