namespace Zipkin.Tests
{
	using System.Collections.Generic;
	using System.IO;
	using System.Net;
	using NUnit.Framework;

	[TestFixture]
	public class ThriftCodecTest
	{
		private readonly ThriftCodec _codec = new ThriftCodec();

		[Test]
		public void Simple_span_serializes_fwd_and_back()
		{
			// Arrange
			var stream = new MemoryStream();
			var span = new Span(int.MaxValue + 1L, "name", long.MaxValue - 1);

			// Act
			_codec.WriteSpan(span, stream);

			stream.Position = 0L;
			var recovered = _codec.ReadSpan(stream);

			// Assert
			recovered.AssertEqualTo(span);
		}

		[Test]
		public void Span_with_full_objectmodel_serializes_fwd_and_back()
		{
			// Arrange
			var stream = new MemoryStream();
			var span = new Span(int.MaxValue + 1L, "name", long.MaxValue - 1)
			{
				IsDebug = true,
				ParentId = 123,
				Annotations = new List<Annotation>()
				{
					new Annotation()
					{
						Value = "cs",
						Host = new Endpoint() {IPAddress = IPAddress.Loopback, Port = 123, ServiceName = "the_service "}
					}
				},
				BinaryAnnotations = new List<BinaryAnnotation>()
				{
					new BinaryAnnotation("key", 123.2D),
					new BinaryAnnotation("key2", "stringval"),
					new BinaryAnnotation("key3", (short)123),
					new BinaryAnnotation("key4", 987),
					new BinaryAnnotation("key5", 987L),
					new BinaryAnnotation("key6", true)
				}
			};

			// Act
			_codec.WriteSpan(span, stream);

			stream.Position = 0L;
			var recovered = _codec.ReadSpan(stream);

			// Assert
			recovered.AssertEqualTo(span);
		}
	}
}