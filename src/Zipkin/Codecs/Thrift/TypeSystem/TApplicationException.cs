namespace Zipkin.Codecs.Thrift.TypeSystem
{
	using System;

	public class TApplicationException : TException
	{
		protected ExceptionType type;

		public TApplicationException()
		{
		}

		public TApplicationException(ExceptionType type)
		{
			this.type = type;
		}

		public TApplicationException(ExceptionType type, string message) : base(message)
		{
			this.type = type;
		}

		public static TApplicationException Read(ThriftProtocol iprot)
		{
			TField field;

			string message = null;
			ExceptionType type = ExceptionType.Unknown;

			iprot.ReadStructBegin();
			while (true)
			{
				field = iprot.ReadFieldBegin();
				if (field.Type == TType.Stop)
				{
					break;
				}

				switch (field.ID)
				{
					case 1:
						if (field.Type == TType.String)
						{
							message = iprot.ReadString();
						}
						else
						{
							iprot.Skip(field.Type);
						}
						break;
					case 2:
						if (field.Type == TType.I32)
						{
							type = (ExceptionType)iprot.ReadI32();
						}
						else
						{
							iprot.Skip(field.Type);
						}
						break;
					default:
						iprot.Skip(field.Type);
						break;
				}

				iprot.ReadFieldEnd();
			}

			iprot.ReadStructEnd();

			return new TApplicationException(type, message);
		}

		public void Write(ThriftProtocol oprot)
		{
			TStruct struc = new TStruct(/*"TApplicationException"*/);
			TField field = new TField();

			oprot.WriteStructBegin(struc);

			if (!string.IsNullOrEmpty(Message))
			{
//				field.Name = "message";
				field.Type = TType.String;
				field.ID = 1;
				oprot.WriteFieldBegin(field);
				oprot.WriteString(Message);
				oprot.WriteFieldEnd();
			}

//			field.Name = "type";
			field.Type = TType.I32;
			field.ID = 2;
			oprot.WriteFieldBegin(field);
			oprot.WriteI32((int)type);
			oprot.WriteFieldEnd();

			oprot.WriteFieldStop();
			oprot.WriteStructEnd();
		}

		public enum ExceptionType
		{
			Unknown,
			UnknownMethod,
			InvalidMessageType,
			WrongMethodName,
			BadSequenceID,
			MissingResult,
			InternalError,
			ProtocolError,
			InvalidTransform,
			InvalidProtocol,
			UnsupportedClientType
		}
	}
}