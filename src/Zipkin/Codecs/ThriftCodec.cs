namespace Zipkin
{
	using System.Collections.Generic;
	using System.IO;
	using Codecs.Thrift;
	using Codecs.Thrift.ModelSerializer;


	public class ThriftCodec : Codec
	{
		public ThriftCodec() : base("application/x-thrift")
		{
		}

		public override void WriteSpans(IList<Span> spans, Stream stream)
		{
			var protocol = new ThriftProtocol(new TStreamTransport(stream));

			SpanSerializer.WriteList(spans, protocol);

			protocol.Flush();
		}

		public override void WriteSpan(Span span, Stream stream)
		{
			var protocol = new ThriftProtocol(new TStreamTransport(stream));

			SpanSerializer.Write(span, protocol);

			protocol.Flush();
		}

		public override IList<Span> ReadSpans(Stream stream)
		{
			var protocol = new ThriftProtocol(new TStreamTransport(stream));

			return SpanSerializer.ReadList(protocol);
		}

		public override Span ReadSpan(Stream stream)
		{
			var protocol = new ThriftProtocol(new TStreamTransport(stream));

			return SpanSerializer.Read(protocol);
		}
	}
}