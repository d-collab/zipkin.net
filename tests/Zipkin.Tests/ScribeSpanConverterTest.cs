namespace Zipkin.Tests
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using FluentAssertions;
	using NUnit.Framework;

	[TestFixture]
	public class ScribeSpanConverterTest
	{
		private readonly ScribeSpanDispatcher.ScribeSpanConverter _converter = new ScribeSpanDispatcher.ScribeSpanConverter();

		[Test]
		public void Converts_Span_correctly()
		{
			// Arrange
			var span = new Span(1, "test", 3);
			var spans = new List<Span>() { span  };

			// Act
			var entries = _converter.ToLogEntries(spans);
			
			// Assert
			entries.Should().HaveCount(1);
			var logentry = entries[0];
			logentry.category.Should().Be("zipkin");
			var buffer = Convert.FromBase64String(logentry.message);
			var recovered = new ThriftCodec().ReadSpan(new MemoryStream(buffer));

			recovered.AssertEqualTo(span);
		}
	}
}