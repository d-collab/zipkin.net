namespace Zipkin.Codecs.Thrift
{
	using System;
	using System.Buffers;
	using System.IO;
	using Thrift.TypeSystem;

	public partial class ThriftProtocol
	{
		private const int MaxRecursionDepth = 64;
		private const uint VersionMask = 0xffff0000;
		private const uint Version1 = 0x80010000;
		private const bool StrictRead = false;
		private const bool StrictWrite = true;

		private readonly ArrayPool<byte> _bufferPool;

		private const int TempArrayLen = 1024;
		private readonly byte[] _temp = new byte[TempArrayLen];

		private readonly TTransport _stream;

		private int _currentRecursionDepth;


		public ThriftProtocol(TTransport stream)
		{
			_stream = stream;

			_bufferPool = ArrayPool<byte>.Shared;
		}

		public void IncrementRecursionDepth()
		{
			if (_currentRecursionDepth < MaxRecursionDepth)
				_currentRecursionDepth++;
			else 
				throw new Exception("Depth limit exceeded");
		}

		public void DecrementRecursionDepth()
		{
			_currentRecursionDepth--;
		}

		public void Flush()
		{
			_stream.Flush();
		}

		internal void SkipAll(int len)
		{
			var consumed = 0;

			while (consumed < len)
			{
				if (_stream.CanSeek)
				{
					consumed += (int) _stream.Seek(len, SeekOrigin.Current);
				}
				else
				{
					consumed += _stream.Read(_temp, 0, Math.Min( len - consumed, TempArrayLen));
				}
			}
		}

		internal void ReadAll(byte[] buffer, int offset, int len)
		{
			var consumed = 0;

			while (consumed < len)
			{
				consumed += _stream.Read(buffer, offset + consumed, len - consumed);
			}
		}

		public void Skip(TType type)
		{
			this.IncrementRecursionDepth();
			try
			{
				switch (type)
				{
					case TType.Bool:
					case TType.Byte:
						SkipAll(1);
						break;
					case TType.I16:
						SkipAll(2);
						break;
					case TType.I32:
						SkipAll(4);
						break;
					case TType.I64:
						SkipAll(8);
						break;
					case TType.Double:
						SkipAll(8);
						break;
					case TType.String:
						// Don't try to decode the string, just skip it.
						this.ReadString(skipit: true);
						break;
					case TType.Struct:
						this.ReadStructBegin();
						while (true)
						{
							TField field = this.ReadFieldBegin();
							if (field.Type == TType.Stop)
							{
								break;
							}
							Skip(field.Type);
							this.ReadFieldEnd();
						}
						this.ReadStructEnd();
						break;
//					case TType.Map:
//						TMap map = this.ReadMapBegin();
//						for (int i = 0; i < map.Count; i++)
//						{
//							Skip(map.KeyType);
//							Skip(map.ValueType);
//						}
//						this.ReadMapEnd();
//						break;
//					case TType.Set:
//						TSet set = this.ReadSetBegin();
//						for (int i = 0; i < set.Count; i++)
//						{
//							Skip(set.ElementType);
//						}
//						this.ReadSetEnd();
//						break;
					case TType.List:
						TList list = this.ReadListBegin();
						var count = list.Count;
						for (int i = 0; i < count; i++)
						{
							Skip(list.ElementType);
						}
						this.ReadListEnd();
						break;
				}
			}
			finally
			{
				this.DecrementRecursionDepth();
			}
		}
	}
}