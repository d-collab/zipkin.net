namespace Zipkin.Codecs.Thrift.ModelSerializer
{
	using System;
	using System.Text;
	using TypeSystem;

	public static class BinaryAnnotationSerializer
	{
		public static BinaryAnnotation Read(ThriftProtocol iprot)
		{
			iprot.IncrementRecursionDepth();
			var annotation = new BinaryAnnotation();
			byte[] value = null;
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
							if (field.Type == TType.String)
							{
								annotation.Key = iprot.ReadString();
							}
							else
							{
								iprot.Skip(field.Type);
							}
							break;
						case 2:
							if (field.Type == TType.String)
							{
								value = iprot.ReadBinary();
							}
							else
							{
								iprot.Skip(field.Type);
							}
							break;
						case 3:
							if (field.Type == TType.I32)
							{
								annotation.AnnotationType = (AnnotationType)iprot.ReadI32();
							}
							else
							{
								iprot.Skip(field.Type);
							}
							break;
						case 4:
							if (field.Type == TType.Struct)
							{
								// Host = new Endpoint();
								// Host.Read(iprot);
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
			// annotation.InitWith(value);

			if (value != null)
			{
				switch (annotation.AnnotationType)
				{
					case AnnotationType.BOOL:
						annotation.ValAsBool = value.Length == 1 && value[0] == 1;
						break;
					case AnnotationType.I16:
						Array.Reverse(value);
						annotation.ValAsI16 = BitConverter.ToInt16(value, 0);
						break;
					case AnnotationType.I32:
						Array.Reverse(value);
						annotation.ValAsI32 = BitConverter.ToInt32(value, 0);
						break;
					case AnnotationType.I64:
						Array.Reverse(value);
						annotation.ValAsI64 = BitConverter.ToInt64(value, 0);
						break;
					case AnnotationType.DOUBLE:
						Array.Reverse(value);
						annotation.ValAsDouble = BitConverter.ToDouble(value, 0);
						break;
					case AnnotationType.BYTES:
						Array.Reverse(value);
						annotation.ValAsBArray = value;
						break;
					case AnnotationType.STRING:
						annotation.ValAsString = Encoding.UTF8.GetString(value, 0, value.Length);
						break;
				}
			}

			return annotation;
		}

		public static void Write(BinaryAnnotation annotation, ThriftProtocol oprot)
		{
			oprot.IncrementRecursionDepth();
			try
			{
				var struc = new TStruct(/*"BinaryAnnotation"*/);
				oprot.WriteStructBegin(struc);
				var field = new TField();
				if (annotation.Key != null)
				{
//					field.Name = "key";
					field.Type = TType.String;
					field.ID = 1;
					oprot.WriteFieldBegin(field);
					oprot.WriteString(annotation.Key);
					oprot.WriteFieldEnd();
				}
				// if (annotation.Value != null)
				{
//					field.Name = "value";
					field.Type = TType.String;
					field.ID = 2;
					oprot.WriteFieldBegin(field);

					switch (annotation.AnnotationType)
					{
						case AnnotationType.BOOL:
							oprot.WriteI32(1);
							oprot.WriteByte((sbyte) (annotation.ValAsBool ? 1 : 0));
							break;
						case AnnotationType.I16:
							oprot.WriteI32(2);
							oprot.WriteI16(annotation.ValAsI16);
							break;
						case AnnotationType.I32:
							oprot.WriteI32(4);
							oprot.WriteI32(annotation.ValAsI32);
							break;
						case AnnotationType.I64:
							oprot.WriteI32(8);
							oprot.WriteI64(annotation.ValAsI64);
							break;
						case AnnotationType.DOUBLE:
							oprot.WriteI32(8);
							oprot.WriteDouble(annotation.ValAsDouble);
							break;
						case AnnotationType.BYTES:
							oprot.WriteBinary(annotation.ValAsBArray, 0, annotation.ValAsBArray.Length);
							break;
						case AnnotationType.STRING:
							oprot.WriteString(annotation.ValAsString);
							break;
					}

					// oprot.WriteBinary(annotation.Value);
					oprot.WriteFieldEnd();
				}
				// if (annotation.AnnotationType != null)
				{
//					field.Name = "annotation_type";
					field.Type = TType.I32;
					field.ID = 3;
					oprot.WriteFieldBegin(field);
					oprot.WriteI32((int)annotation.AnnotationType);
					oprot.WriteFieldEnd();
				}
				if (annotation.Host != null)
				{
//					field.Name = "host";
					field.Type = TType.Struct;
					field.ID = 4;
					oprot.WriteFieldBegin(field);
					EndpointSerializer.Write(annotation.Host, oprot);
					//annotation.Host.Write(oprot);
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