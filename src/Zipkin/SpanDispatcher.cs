namespace Zipkin
{
	using System;
	using System.Collections.Generic;

	public abstract class SpanDispatcher : IDisposable
	{
		public abstract void DispatchSpans(IList<Span> spans);

		public abstract void Dispose();
	}
}