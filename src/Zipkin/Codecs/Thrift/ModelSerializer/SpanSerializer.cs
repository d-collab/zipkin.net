namespace Zipkin.Codecs.Thrift.ModelSerializer
{
	using System.Collections.Generic;
	using System.Diagnostics;
	using Json;
	using Model;
	using TypeSystem;

	public static class SpanSerializer
	{
		public static IList<Span> ReadList(ThriftProtocol iprot)
		{
			iprot.IncrementRecursionDepth();
			var list = new List<Span>();
			try
			{
				var listInfo = iprot.ReadListBegin();
				for (int i = 0; i < listInfo.Count; i++)
				{
					list.Add( SpanSerializer.Read(iprot) );
				}
				iprot.ReadListEnd();
			}
			finally
			{
				iprot.DecrementRecursionDepth();
			}
			return list;
		}

		public static Span Read(ThriftProtocol iprot)
		{
			iprot.IncrementRecursionDepth();
			var span = new Span();
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
							if (field.Type == TType.I64)
							{
								span.TraceId = iprot.ReadI64();
							}
							else
							{
								iprot.Skip(field.Type);
							}
							break;
						case 3:
							if (field.Type == TType.String)
							{
								span.Name = iprot.ReadString();
							}
							else
							{
								iprot.Skip(field.Type);
							}
							break;
						case 4:
							if (field.Type == TType.I64)
							{
								span.Id = iprot.ReadI64();
							}
							else
							{
								iprot.Skip(field.Type);
							}
							break;
						case 5:
							if (field.Type == TType.I64)
							{
								span.ParentId = iprot.ReadI64();
							}
							else
							{
								iprot.Skip(field.Type);
							}
							break;
						case 6:
							if (field.Type == TType.List)
							{
								{
									span.Annotations = new List<Annotation>();
									var _list0 = iprot.ReadListBegin();
									for (int _i1 = 0; _i1 < _list0.Count; ++_i1)
									{
										var annotation = AnnotationSerializer.Read(iprot);
										span.Annotations.Add(annotation);
									}
									iprot.ReadListEnd();
								}
							}
							else
							{
								iprot.Skip(field.Type);
							}
							break;
						case 8:
							if (field.Type == TType.List)
							{
								{
									span.BinaryAnnotations = new List<BinaryAnnotation>();
									var _list3 = iprot.ReadListBegin();
									for (int _i4 = 0; _i4 < _list3.Count; ++_i4)
									{
//										BinaryAnnotation _elem5;
//										_elem5 = new BinaryAnnotation();
//										_elem5.Read(iprot);
//										Binary_annotations.Add(_elem5);
										var annotation = BinaryAnnotationSerializer.Read(iprot);
										span.BinaryAnnotations.Add(annotation);
									}
									iprot.ReadListEnd();
								}
							}
							else
							{
								iprot.Skip(field.Type);
							}
							break;
						case 9:
							if (field.Type == TType.Bool)
							{
								span.IsDebug = iprot.ReadBool();
							}
							else
							{
								iprot.Skip(field.Type);
							}
							break;
						case 10:
							if (field.Type == TType.I64)
							{
								span.Timestamp = DateTimeOffsetExtensions.FromLong(iprot.ReadI64());
							}
							else
							{
								iprot.Skip(field.Type);
							}
							break;
						case 11:
							if (field.Type == TType.I64)
							{
								span.DurationInMicroseconds = iprot.ReadI64();
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
			return span;
		}

		public static void WriteList(IList<Span> spans, ThriftProtocol oprot)
		{
			oprot.IncrementRecursionDepth();
			try
			{
				var count = spans.Count;
				oprot.WriteListBegin(new TList { Count = count, ElementType = TType.Struct });

				for (int i = 0; i < count; i++)
				{
					Write(spans[i], oprot);
				}

				oprot.WriteListEnd();
			}
			finally
			{
				oprot.DecrementRecursionDepth();
			}
		}

		public static void Write(Span span, ThriftProtocol oprot)
		{
			oprot.IncrementRecursionDepth();
			try
			{
				TStruct struc = new TStruct(/*"Span"*/);
				oprot.WriteStructBegin(struc);
				TField field = new TField();
				if (span.TraceId != 0L)
				{
//					field.Name = "trace_id";
					field.Type = TType.I64;
					field.ID = 1;
					oprot.WriteFieldBegin(field);
					oprot.WriteI64(span.TraceId);
					oprot.WriteFieldEnd();
				}
				if (span.Name != null)
				{
//					field.Name = "name";
					field.Type = TType.String;
					field.ID = 3;
					oprot.WriteFieldBegin(field);
					oprot.WriteString(span.Name);
					oprot.WriteFieldEnd();
				}
				if (span.Id != 0)
				{
//					field.Name = "id";
					field.Type = TType.I64;
					field.ID = 4;
					oprot.WriteFieldBegin(field);
					oprot.WriteI64(span.Id);
					oprot.WriteFieldEnd();
				}
				if (span.ParentId.HasValue)
				{
//					field.Name = "parent_id";
					field.Type = TType.I64;
					field.ID = 5;
					oprot.WriteFieldBegin(field);
					oprot.WriteI64(span.ParentId.Value);
					oprot.WriteFieldEnd();
				}
				if (span.Annotations != null && span.Annotations.Count != 0)
				{
//					field.Name = "annotations";
					field.Type = TType.List;
					field.ID = 6;
					oprot.WriteFieldBegin(field);
					{
						oprot.WriteListBegin(new TList(TType.Struct, span.Annotations.Count));
						foreach (Annotation annotation in span.Annotations)
						{
							AnnotationSerializer.Write(annotation, oprot);
							// _iter6.Write(oprot);
						}
						oprot.WriteListEnd();
					}
					oprot.WriteFieldEnd();
				}
				if (span.BinaryAnnotations != null && span.BinaryAnnotations.Count != 0)
				{
//					field.Name = "binary_annotations";
					field.Type = TType.List;
					field.ID = 8;
					oprot.WriteFieldBegin(field);
					{
						oprot.WriteListBegin(new TList(TType.Struct, span.BinaryAnnotations.Count));
						foreach (BinaryAnnotation annotation in span.BinaryAnnotations)
						{
							BinaryAnnotationSerializer.Write(annotation, oprot);
							// _iter7.Write(oprot);
						}
						oprot.WriteListEnd();
					}
					oprot.WriteFieldEnd();
				}
				if (span.IsDebug)
				{
//					field.Name = "debug";
					field.Type = TType.Bool;
					field.ID = 9;
					oprot.WriteFieldBegin(field);
					oprot.WriteBool(span.IsDebug);
					oprot.WriteFieldEnd();
				}
				if (span.Timestamp != null)
				{
//					field.Name = "timestamp";
					field.Type = TType.I64;
					field.ID = 10;
					oprot.WriteFieldBegin(field);
					oprot.WriteI64(span.Timestamp.Value.ToNixTimeMicro());
					oprot.WriteFieldEnd();
				}
				if (span.DurationInMicroseconds != null)
				{
//					field.Name = "duration";
					field.Type = TType.I64;
					field.ID = 11;
					oprot.WriteFieldBegin(field);
					oprot.WriteI64(span.DurationInMicroseconds.Value);
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