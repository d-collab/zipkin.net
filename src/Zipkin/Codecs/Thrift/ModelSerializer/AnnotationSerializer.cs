namespace Zipkin.Codecs.Thrift.ModelSerializer
{
	using System;
	using TypeSystem;

	public static class AnnotationSerializer
	{
		public static Annotation Read(ThriftProtocol iprot)
		{
			iprot.IncrementRecursionDepth();
			var annotation = new Annotation();
			try
			{
				iprot.ReadStructBegin();
				while (true)
				{
					var field = iprot.ReadFieldBegin();
					if (field.Type == TType.Stop)
					{
						break;
					}
					switch (field.ID)
					{
						case 1:
							if (field.Type == TType.I64)
							{
								annotation.Timestamp = DateTimeOffsetExtensions.FromLong(iprot.ReadI64());
							}
							else
							{
								iprot.Skip(field.Type);
							}
							break;
						case 2:
							if (field.Type == TType.String)
							{
								annotation.Value = iprot.ReadString();
							}
							else
							{
								iprot.Skip(field.Type);
							}
							break;
						case 3:
							if (field.Type == TType.Struct)
							{
//								Host = new Endpoint();
//								Host.Read(iprot);
								annotation.Host = EndpointSerializer.Read(iprot);
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
			return annotation;
		}

		public static void Write(Annotation annotation, ThriftProtocol oprot)
		{
			oprot.IncrementRecursionDepth();
			try
			{
				TStruct struc = new TStruct(/*"Annotation"*/);
				oprot.WriteStructBegin(struc);
				TField field = new TField();
				// if (annotation.Timestamp != DateTimeOffset.MinValue)
				{
//					field.Name = "timestamp";
					field.Type = TType.I64;
					field.ID = 1;
					oprot.WriteFieldBegin(field);
					oprot.WriteI64(annotation.Timestamp.ToNixTimeMicro());
					oprot.WriteFieldEnd();
				}
				if (annotation.Value != null)
				{
//					field.Name = "value";
					field.Type = TType.String;
					field.ID = 2;
					oprot.WriteFieldBegin(field);
					oprot.WriteString(annotation.Value);
					oprot.WriteFieldEnd();
				}
				if (annotation.Host != null)
				{
//					field.Name = "host";
					field.Type = TType.Struct;
					field.ID = 3;
					oprot.WriteFieldBegin(field);
					// annotation.Host.Write(oprot);
					EndpointSerializer.Write(annotation.Host, oprot);
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