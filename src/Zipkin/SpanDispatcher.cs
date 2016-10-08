namespace Zipkin
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;

	/// <summary>
	/// Abstraction on the method used to send the Span information to zipkin server.
	/// </summary>
	public abstract class SpanDispatcher : IDisposable
	{
		public abstract Task DispatchSpans(IList<Span> spans);

		public abstract void Dispose();
	}
}