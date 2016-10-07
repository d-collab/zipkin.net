namespace Zipkin.Codecs.Thrift.Scribe
{
	using TypeSystem;
	using Zipkin.Codecs;
	using ThriftProtocol = Thrift.ThriftProtocol;

	public class LogEntrySerializer
	{
		public static LogEntry Read(ThriftProtocol iprot)
		{
			iprot.IncrementRecursionDepth();
			var logEntry = new LogEntry();
			try
			{
				TField field;
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
								/*logEntry.category =*/iprot.ReadString();
							}
							else
							{
								iprot.Skip(field.Type);
							}
							break;
						case 2:
							if (field.Type == TType.String)
							{
								logEntry.message = iprot.ReadString();
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
			}
			finally
			{
				iprot.DecrementRecursionDepth();
			}
			return logEntry;
		}

		public static void Write(ThriftProtocol oprot, LogEntry logEntry)
		{
			oprot.IncrementRecursionDepth();
			try
			{
				TStruct struc = new TStruct(/*"LogEntry"*/);
				oprot.WriteStructBegin(struc);
				TField field = new TField();
				if (logEntry.category != null)
				{
//					field.Name = "category";
					field.Type = TType.String;
					field.ID = 1;
					oprot.WriteFieldBegin(field);
					oprot.WriteString(logEntry.category);
					oprot.WriteFieldEnd();
				}
				if (logEntry.message != null)
				{
//					field.Name = "message";
					field.Type = TType.String;
					field.ID = 2;
					oprot.WriteFieldBegin(field);
					oprot.WriteString(logEntry.message);
					oprot.WriteFieldEnd();
				}
				oprot.WriteFieldStop();
				oprot.WriteStructEnd();
			}
			finally
			{
				oprot.DecrementRecursionDepth();
			}
		}
	}
}