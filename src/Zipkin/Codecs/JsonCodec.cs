namespace Zipkin
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using System.Text;
	using Codecs;
	using Codecs.Json;


	public class JsonCodec : Codec
	{
		public JsonCodec() : base("application/json")
		{
		}

		public override void WriteSpans(IList<Span> spans, Stream stream)
		{
			using (var w = new StreamWriter(stream, Encoding.UTF8, 128, leaveOpen: true))
			{
				foreach (var span in spans)
				{
					if (span == null) continue;

					InternalWriteSpan(w, span);
				}	
			}
		}

		public override void WriteSpan(Span span, Stream stream)
		{
			if (span == null) return;

			using (var w = new StreamWriter(stream, Encoding.UTF8, 128, leaveOpen: true))
			{
				InternalWriteSpan(w, span);
			}
		}

		public override IList<Span> ReadSpans(Stream stream)
		{
			throw new NotImplementedException();
		}

		public override Span ReadSpan(Stream stream)
		{
			throw new NotImplementedException();
		}

		private void InternalWriteSpan(StreamWriter w, Span span)
		{
			w.Write("{\"traceId\":\"");
			w.WriteLowerHex(span.TraceId);
			w.Write("\",\"id\":\"");
			w.WriteLowerHex(span.Id);
			w.Write("\",\"name\":\"");
			w.WriteJsonEscaped(span.Name);
			w.Write('"');

			if (span.ParentId.HasValue)
			{
				w.Write(",\"parentId\":\"");
				w.WriteLowerHex(span.ParentId.Value);
				w.Write('"');
			}
			if (span.Timestamp.HasValue)
			{
				w.Write(",\"timestamp\":");
				w.Write(span.Timestamp.Value.ToNixTimeMicro());
			}
			if (span.DurationInMicroseconds.HasValue)
			{
				w.Write(",\"duration\":");
				w.Write(span.DurationInMicroseconds.Value);
			}
			if (span.Annotations != null && span.Annotations.Count != 0)
			{
				w.Write(",\"annotations\":");
				WriteAnnotations(span.Annotations, w);
			}
			if (span.BinaryAnnotations != null && span.BinaryAnnotations.Count != 0)
			{
				w.Write(",\"binaryAnnotations\":");
				WriteBinaryAnnotations(span.BinaryAnnotations, w);
			}
			if (span.IsDebug)
			{
				w.Write(",\"debug\":true");
			}
			w.Write('}');
		}

		private static void WriteBinaryAnnotations(IList<BinaryAnnotation> binaryAnnotations, StreamWriter w)
		{
			w.Write("[");
			bool appendComma = false;

			foreach (var annotation in binaryAnnotations)
			{
				if (appendComma) w.Write(",");
				appendComma = true;

				w.Write("{\"key\":\"");
				w.WriteJsonEscaped(annotation.Key);
				w.Write("\",\"value\":");

				switch (annotation.AnnotationType)
				{
					case AnnotationType.BOOL:
						w.Write(annotation.ValAsBool ? "true" : "false");
						break;
					case AnnotationType.STRING:
						w.Write('"');
						w.WriteJsonEscaped(annotation.ValAsString);
						w.Write('"');
						break;
					case AnnotationType.BYTES:
						w.Write('"');
						w.WriteBase64Url(annotation.ValAsBArray);
						w.Write('"');
						break;
					case AnnotationType.I16:
						w.Write(annotation.ValAsI16.ToString());
						break;
					case AnnotationType.I32:
						w.Write(annotation.ValAsI32.ToString());
						break;
					case AnnotationType.I64:
						w.Write(annotation.ValAsI64.ToString());
						break;
					case AnnotationType.DOUBLE:
						var d = annotation.ValAsDouble;
						// double wrapped = Double.longBitsToDouble(ByteBuffer.wrap(value.value).getLong());
						w.Write(d.ToString(CultureInfo.InvariantCulture));
						break;
				}
				if (annotation.AnnotationType != AnnotationType.STRING && annotation.AnnotationType != AnnotationType.BOOL)
				{
					w.Write(",\"type\":\"");
					// w.Write(value.type.name());
					w.Write(annotation.AnnotationType.ToString("G"));
					w.Write('"');
				}
				if (annotation.Host != null)
				{
					w.Write(",\"endpoint\":");
					// ENDPOINT_ADAPTER.write(value.endpoint, b);
					WriteHost(annotation.Host, w);
				}
				w.Write('}');
			}
			w.Write("]");
		}

		private static void WriteAnnotations(IList<Annotation> annotations, StreamWriter w)
		{
			w.Write("[");
			bool appendComma = false;
			foreach (var annotation in annotations)
			{
				if (appendComma) w.Write(",");
				appendComma = true;

				w.Write("{\"timestamp\":");
				w.Write(annotation.Timestamp.ToNixTimeMicro());

				w.Write(",\"value\":\"");
				w.WriteJsonEscaped(annotation.Value);
				w.Write('"');

				if (annotation.Host != null)
				{
					w.Write(",\"endpoint\":");
					WriteHost(annotation.Host, w);
				}
				w.Write('}');
			}
			w.Write("]");
		}

		private static void WriteHost(Endpoint endpoint, StreamWriter w)
		{
			w.Write("{\"serviceName\":\"");
			w.WriteJsonEscaped(endpoint.ServiceName);
			w.Write('"');

			var ipv4 = endpoint.IPAddress.GetAddressBytes();

			if (ipv4.Length != 0)
			{
				w.Write(",\"ipv4\":\"");
				w.Write(ipv4[0]); w.Write('.');
				w.Write(ipv4[1]); w.Write('.');
				w.Write(ipv4[2]); w.Write('.');
				w.Write(ipv4[3]); w.Write('"');
			}
			if (endpoint.Port.HasValue)
			{
				w.Write(",\"port\":");
				w.Write(endpoint.Port.Value);
			}
//			if (value.ipv6 != null)
//			{
//				b.writeAscii(",\"ipv6\":\"").writeIpV6(value.ipv6).writeByte('"');
//			}
			w.Write('}');
		}
	}
}