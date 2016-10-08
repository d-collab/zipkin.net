namespace Zipkin
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;

	/// <summary>
	/// Starts a server trace, which is contingent on an existing client trace. 
	/// 
	/// The Span is also annotated with <see cref="AnnotationConstants.SERVER_RECV"/> as per guidelines.
	/// </summary>
	public class StartServerTrace : ITrace
	{
		private readonly Stopwatch _watch;

		public Span Span { get; private set; }

		/// <summary>
		/// Give it a short lower-case description of the activity
		/// </summary>
		public StartServerTrace(string name, IDictionary<string, object> crossProcessContext)
		{
			long traceId, parentSpanId;

			if (TraceContextPropagation.TryObtainTraceIdFrom(crossProcessContext, out traceId, out parentSpanId))
			{
				_watch = Stopwatch.StartNew();

				Span = new Span(traceId, name, RandomHelper.NewId())
				{
					ParentId = parentSpanId
				};
				this.AnnotateWith(PredefinedTag.ServerRecv, name);

				TraceContextPropagation.PushSpan(Span);
			}
		}

		public void Dispose()
		{
			if (Span != null)
			{
				Span.DurationInMicroseconds = _watch.ElapsedMilliseconds * 1000;

				TraceContextPropagation.PopSpan(this.Span);

				ZipkinConfig.Record(Span);

				Span = null;
			}
		}
	}
}