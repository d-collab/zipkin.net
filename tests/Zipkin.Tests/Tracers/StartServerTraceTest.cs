namespace Zipkin.Tests.Tracers
{
	using System.Collections.Generic;
	using FluentAssertions;
	using NUnit.Framework;

	[TestFixture]
	public class StartServerTraceTest
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
			using (var parentTrace = new StartClientTrace("parent"))
			{
				var dict = new Dictionary<string,object>();
				TraceContextPropagation.PropagateTraceIdOnto(dict);

				using (var trace = new StartServerTrace("name", dict))
				{
					trace.Span.Should().NotBeNull();
					trace.Span.Name.Should().Be("name");
					trace.Span.TraceId.Should().Be(parentTrace.Span.TraceId);
					trace.Span.ParentId.Should().Be(parentTrace.Span.Id);

					trace.Span.Annotations[0].Value.Should().Be("sr");

					_recorder.Recorded.Should().HaveCount(0);

					TraceContextPropagation.CurrentSpan.Should().Be(trace.Span);
				}
			}
			_recorder.Recorded.Should().HaveCount(2);

			TraceContextPropagation.CurrentSpan.Should().BeNull();
		}
	}
}