namespace Zipkin
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;

	public abstract class SpanDispatcher : IDisposable
	{
		public abstract Task DispatchSpans(IList<Span> spans);

		public abstract void Dispose();
	}
}