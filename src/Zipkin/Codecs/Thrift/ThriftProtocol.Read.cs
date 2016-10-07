namespace Zipkin.Codecs.Thrift
{
	using System;
	using System.Runtime.CompilerServices;
	using System.Text;
	using Thrift.TypeSystem;

	public partial class ThriftProtocol
	{
		private readonly byte[] _bin = new byte[1];
		private readonly byte[] _i16in = new byte[2];
		private readonly byte[] _i32in = new byte[4];
		private readonly byte[] _i64in = new byte[8];

		public TMessage ReadMessageBegin()
		{
			var message = new TMessage();
			int size = ReadI32();
			if (size < 0)
			{
				uint version = (uint) size & VersionMask;
				if (version != Version1) throw new TProtocolException(TProtocolException.BAD_VERSION, "Bad version in ReadMessageBegin: " + version);
				message.type = (TMessageType)(size & 0x000000ff);
				message.name = ReadString();
				message.seqID = ReadI32();
			}
			else
			{
				if (StrictRead) throw new TProtocolException(TProtocolException.BAD_VERSION, "Missing version in readMessageBegin, old client?");
				message.name = ReadStringBody(size);
				message.type = (TMessageType)ReadByte();
				message.seqID = ReadI32();
			}
			return message;
		}

		public void ReadMessageEnd()
		{
		}

		public TList ReadListBegin()
		{
			var list = new TList
			{
				ElementType = (TType) ReadByte(),
				Count = ReadI32()
			};
			return list;
		}

		public void ReadListEnd()
		{
		}

//		public TStruct ReadStructBegin()
//		{
//			return new TStruct();
//		}

		public void ReadStructBegin()
		{
		}

		public void ReadStructEnd()
		{
		}

		public TField ReadFieldBegin()
		{
			var field = new TField
			{
				Type = (TType) ReadByte()
			};

			if (field.Type != TType.Stop)
			{
				field.ID = ReadI16();
			}

			return field;
		}

		public void ReadFieldEnd()
		{
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool ReadBool()
		{
			return ReadByte() == 1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public sbyte ReadByte()
		{
			ReadAll(_bin, 0, 1);
			return (sbyte)_bin[0];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public short ReadI16()
		{
			ReadAll(_i16in, 0, 2);
			return (short)(((_i16in[0] & 0xff) << 8) | 
						  ((_i16in[1] & 0xff)));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int ReadI32()
		{
			ReadAll(_i32in, 0, 4);
			return (int)(((_i32in[0] & 0xff) << 24) | 
						((_i32in[1] & 0xff) << 16) | 
						((_i32in[2] & 0xff) << 8) | 
						((_i32in[3] & 0xff)));
		}

		public long ReadI64()
		{
			ReadAll(_i64in, 0, 8);
			unchecked
			{
				return (long)(
					((long)(_i64in[0] & 0xff) << 56) |
					((long)(_i64in[1] & 0xff) << 48) |
					((long)(_i64in[2] & 0xff) << 40) |
					((long)(_i64in[3] & 0xff) << 32) |
					((long)(_i64in[4] & 0xff) << 24) |
					((long)(_i64in[5] & 0xff) << 16) |
					((long)(_i64in[6] & 0xff) << 8) |
					((long)(_i64in[7] & 0xff)));
			}
		}

		public double ReadDouble()
		{
			return BitConverter.Int64BitsToDouble(ReadI64());
		}

		public byte[] ReadBinary(bool skipit = false)
		{
			int size = ReadI32();
			if (skipit)
			{
				this.SkipAll(size);
				return null;
			}

			var buf = new byte[size];
			ReadAll(buf, 0, size);
			return buf;
		}

		public string ReadString(bool skipit = false)
		{
			int size = ReadI32();
			if (skipit)
			{
				SkipAll(size);
				return null;
			}

			var buf = _bufferPool.Rent(size);
			try
			{
				ReadAll(buf, 0, size);
				return Encoding.UTF8.GetString(buf, 0, size);
			}
			finally
			{
				_bufferPool.Return(buf);
			}
		}
		
		private string ReadStringBody(int size)
		{
			var buf = _bufferPool.Rent(size);

			try
			{
				ReadAll(buf, 0, size);
				return Encoding.UTF8.GetString(buf, 0, size);
			}
			finally
			{
				_bufferPool.Return(buf);
			}
		}
	}
}