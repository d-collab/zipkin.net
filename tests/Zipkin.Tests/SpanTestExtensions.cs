namespace Zipkin.Tests
{
	using FluentAssertions;

	public static class SpanTestExtensions
	{
		public static void AssertEqualTo(this Span recovered, Span span)
		{
			recovered.TraceId.Should().Be(span.TraceId);
			recovered.Id.Should().Be(span.Id);
			recovered.ParentId.Should().Be(span.ParentId);
			recovered.IsDebug.Should().Be(span.IsDebug);

			if (span.BinaryAnnotations == null)
			{
				recovered.BinaryAnnotations.Should().BeNull();
			}
			else
			{
				recovered.BinaryAnnotations.Should().HaveCount(span.BinaryAnnotations.Count);
				recovered.BinaryAnnotations.Should().ContainInOrder(span.BinaryAnnotations);
			}

			if (span.Annotations == null)
			{
				recovered.Annotations.Should().BeNull();
			}
			else
			{
				recovered.Annotations.Should().HaveCount(span.Annotations.Count);
				recovered.Annotations.Should().ContainInOrder(span.Annotations);
			}
		}
	}
}