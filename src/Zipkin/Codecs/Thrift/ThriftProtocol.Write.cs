namespace Zipkin.Codecs.Thrift
{
	using System;
	using System.Runtime.CompilerServices;
	using System.Text;
	using Thrift.TypeSystem;

	public partial class ThriftProtocol
	{
		private readonly byte[] _bout = new byte[1];
		private readonly byte[] _i16out = new byte[2];
		private readonly byte[] _i32out = new byte[4];
		private readonly byte[] _i64out = new byte[8];

		public void WriteMessageBegin(TMessage message)
		{
			if (_strictWrite)
			{
				uint version = Version1 | (uint)(message.type);
				WriteI32((int)version);
				WriteString(message.name);
				WriteI32(message.seqID);
			}
//			else
//			{
//				WriteString(message.name);
//				WriteByte((sbyte)message.type);
//				WriteI32(message.seqID);
//			}
		}

		public void WriteMessageEnd()
		{
		}

		public void WriteListBegin(TList list)
		{
			WriteByte((sbyte)list.ElementType);
			WriteI32(list.Count);
		}

		public void WriteListEnd()
		{
		}

		public void WriteStructBegin(TStruct struc)
		{
		}

		public void WriteStructEnd()
		{
		}

		public void WriteFieldBegin(TField field)
		{
			WriteByte((sbyte)field.Type);
			WriteI16(field.ID);
		}
		public void WriteFieldEnd()
		{
		}
		public void WriteFieldStop()
		{
			WriteByte((sbyte)TType.Stop);
		}

		public void WriteBool(bool b)
		{
			WriteByte(b ? (sbyte)1 : (sbyte)0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteByte(sbyte b)
		{
			_bout[0] = (byte)b;
			_stream.Write(_bout, 0, 1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteI16(short s)
		{
			_i16out[0] = (byte)(0xff & (s >> 8));
			_i16out[1] = (byte)(0xff & s);
			_stream.Write(_i16out, 0, 2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteI32(int i32)
		{
			_i32out[0] = (byte)(0xff & (i32 >> 24));
			_i32out[1] = (byte)(0xff & (i32 >> 16));
			_i32out[2] = (byte)(0xff & (i32 >> 8));
			_i32out[3] = (byte)(0xff & i32);
			_stream.Write(_i32out, 0, 4);
		}

		public void WriteI64(long i64)
		{
			_i64out[0] = (byte)(0xff & (i64 >> 56));
			_i64out[1] = (byte)(0xff & (i64 >> 48));
			_i64out[2] = (byte)(0xff & (i64 >> 40));
			_i64out[3] = (byte)(0xff & (i64 >> 32));
			_i64out[4] = (byte)(0xff & (i64 >> 24));
			_i64out[5] = (byte)(0xff & (i64 >> 16));
			_i64out[6] = (byte)(0xff & (i64 >> 8));
			_i64out[7] = (byte)(0xff & i64);
			_stream.Write(_i64out, 0, 8);
		}

		public void WriteDouble(double d)
		{
			WriteI64(BitConverter.DoubleToInt64Bits(d));
		}

		public void WriteBinary(byte[] b, int offset, int count)
		{
			WriteI32(count - offset);
			_stream.Write(b, offset, count);
		}

		public void WriteString(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				WriteI32(0); // size 0
				return;
			}

			var size = Encoding.UTF8.GetByteCount(value);

			var buf = _bufferPool.Rent(size);
			try
			{
				var len = Encoding.UTF8.GetBytes(value, 0, value.Length, buf, 0);

				WriteBinary(buf, 0, len);
			}
			finally
			{
				_bufferPool.Return(buf);
			}
		}
	}
}