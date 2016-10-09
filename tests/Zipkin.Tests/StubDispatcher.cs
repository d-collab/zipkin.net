namespace Zipkin.Tests
{
	using System.Collections.Generic;
	using System.Threading.Tasks;

	class StubDispatcher : SpanDispatcher
	{
		public List<Span> Dispatched = new List<Span>();
		public bool Disposed = false;

		public override Task DispatchSpans(IList<Span> spans)
		{
			Dispatched.AddRange(spans);

			return Task.CompletedTask;
		}

		public override void Dispose()
		{
			Disposed = true;
		}
	}
}