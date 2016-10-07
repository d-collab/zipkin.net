namespace Zipkin
{
	using System.Collections.Generic;

	public class RestSpanDispatcher : SpanDispatcher
	{
		public RestSpanDispatcher(string endpoint, int port, string path = "v")
		{
		}

		public override void DispatchSpans(IList<Span> spans)
		{
		}

		public override void Dispose()
		{
		}
	}
}