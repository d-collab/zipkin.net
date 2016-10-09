namespace Zipkin.Tests.Tracers
{
	using System.Collections.Generic;

	class StubRecorder : Recorder
	{
		public List<Span> Recorded = new List<Span>();

		public override void Record(params Span[] spans)
		{
			Recorded.AddRange(spans);
		}

		public override void Dispose()
		{
		}
	}
}