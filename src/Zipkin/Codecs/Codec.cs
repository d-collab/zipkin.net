namespace Zipkin
{
	using System.Collections.Generic;
	using System.IO;

	public abstract class Codec
	{
		protected Codec(string contentType)
		{
			ContentType = contentType;
		}

		public string ContentType { get; private set; }

		public abstract void WriteSpans(IList<Span> spans, Stream stream);

		public abstract void WriteSpan(Span span, Stream stream);

		public abstract IList<Span> ReadSpans(Stream stream);

		public abstract Span ReadSpan(Stream stream);
	}
}