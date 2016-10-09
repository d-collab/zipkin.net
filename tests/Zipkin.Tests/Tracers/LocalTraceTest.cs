namespace Zipkin.Tests.Tracers
{
	using FluentAssertions;
	using NUnit.Framework;

	[TestFixture]
	public class LocalTraceTest
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
			using (var parent = new StartClientTrace("name"))
			using (var trace = new LocalTrace("name"))
			{
				trace.Span.Should().NotBeNull();
				trace.Span.Name.Should().Be("name");

				_recorder.Recorded.Should().HaveCount(0);

				TraceContextPropagation.CurrentSpan.Should().Be(trace.Span);
			}

			_recorder.Recorded.Should().HaveCount(2);

			TraceContextPropagation.CurrentSpan.Should().BeNull();
		}
	}
}
