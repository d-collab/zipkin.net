namespace Zipkin.Tests
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Net;
	using System.Text;
	using Codecs;
	using Model;
	using NUnit.Framework;


	[TestFixture]
    public class JsonCodecTest
	{
		private static DateTimeOffset FixedTime = new DateTimeOffset(2016, 10, 4, 22, 49, 12, TimeSpan.Zero);

		[Test]
		public void SimpleSpan_to_json()
		{
			// Arrange
			var stream = new MemoryStream();

			// Act
			new JsonCodec().Encode(new Span(traceId: 10951, name: "root", id: 15910)
			{
				Timestamp = new DateTimeOffset(2016, 7, 16, 4, 40, 1, TimeSpan.Zero),
			}, stream);

			// Assert
			Console.WriteLine(Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Length));
		}

		[Test]
		public void Annotations()
		{
			// Arrange
			var stream = new MemoryStream();

			// Act
			new JsonCodec().Encode(new Span(traceId: 10951, name: "root", id: 15910)
			{
				Timestamp = new DateTimeOffset(2016, 7, 16, 4, 40, 1, TimeSpan.Zero),

				Annotations = new List<Annotation>()
				{
					new Annotation() { Host = new Endpoint { IPAddress = IPAddress.Loopback, ServiceName = "hellosvc" }, Timestamp = FixedTime, Value = "something" },
					new Annotation() { Timestamp = FixedTime.AddSeconds(1), Value = "no host" }
				}
			}, stream);

			// Assert
			Console.WriteLine(Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Length));

			// Assert.AreEqual(@"﻿", Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Length));
		}

		[Test]
		public void BinaryAnnotations()
		{
			// Arrange
			var stream = new MemoryStream();

			// Act
			new JsonCodec().Encode(new Span(traceId: 10951, name: "root", id: 15910)
			{
				IsDebug = true,
				ParentId = 10,
				Timestamp = FixedTime,

				BinaryAnnotations = new List<BinaryAnnotation>()
				{
					new BinaryAnnotation(AnnotationConstants.CLIENT_SEND, int.MaxValue - 10)
					{
						Host = new Endpoint() { IPAddress = IPAddress.Loopback, ServiceName = "hellosvc" },
					},
					new BinaryAnnotation(AnnotationConstants.CLIENT_RECV, 12345678910L),
					new BinaryAnnotation(AnnotationConstants.CLIENT_SEND, "this is a string"),
					new BinaryAnnotation(AnnotationConstants.CLIENT_SEND, true),
					new BinaryAnnotation(AnnotationConstants.CLIENT_ADDR, false),
					new BinaryAnnotation(AnnotationConstants.CLIENT_RECV_FRAGMENT, new byte[] { 1,2,3,4,5 }),
					new BinaryAnnotation(AnnotationConstants.SERVER_ADDR, 1234.421),
				}
			}, stream);

			// Assert
			// Console.WriteLine(Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Length));
			Assert.AreEqual("﻿{\"traceId\":\"0000000000002ac7\",\"id\":\"0000000000003e26\",\"name\":\"root\",\"parentId\":\"000000000000000a\",\"timestamp\":1475621352000000,\"binaryAnnotations\":[{\"key\":\"cs\",\"value\":2147483637,\"type\":\"I32\",\"endpoint\":{\"serviceName\":\"hellosvc\",\"ipv4\":\"127.0.0.1\"}},{\"key\":\"cr\",\"value\":12345678910,\"type\":\"I64\"},{\"key\":\"cs\",\"value\":\"this is a string\"},{\"key\":\"cs\",\"value\":true},{\"key\":\"ca\",\"value\":false},{\"key\":\"crf\",\"value\":\"AQIDBAU=\",\"type\":\"BYTES\"},{\"key\":\"sa\",\"value\":1234.421,\"type\":\"DOUBLE\"}],\"debug\":true}", 
				Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Length));
		}
    }
}
