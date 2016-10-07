namespace Zipkin.Codecs
{
	using System.IO;
	using Thrift;
	using Thrift.ModelSerializer;

	public class ThriftCodec
	{
		public void Encode(Span span, Stream stream)
		{
			var protocol = new ThriftProtocol(stream);

			SpanSerializer.Write(span, protocol);
		}

		
	}
}