namespace Zipkin.Tests.Tracers
{
	using FluentAssertions;
	using NUnit.Framework;

	[TestFixture]
	public class StartClientTraceTest
	{
		private StubRecorder _recorder;

		[SetUp]
		public void Init()
		{
			_recorder = new StubRecorder();

			ZipkinConfig._sampleRate = 1.0;
			ZipkinConfig.Recorder = _recorder;
		}

		[Test]
		public void Basic_Trace_config()
		{
			using (var trace = new StartClientTrace("name"))
			{
				trace.Span.Should().NotBeNull();
				trace.Span.Name.Should().Be("name");

				trace.Span.Annotations[0].Value.Should().Be("cs");

				_recorder.Recorded.Should().HaveCount(0);

				TraceContextPropagation.CurrentSpan.Should().Be(trace.Span);
			}

			_recorder.Recorded.Should().HaveCount(1);

			TraceContextPropagation.CurrentSpan.Should().BeNull();
		}
	}
}