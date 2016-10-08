namespace Zipkin
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;


	public class StartServerTrace : IDisposable
	{
		private readonly Stopwatch _watch;

		public StartServerTrace(string name, IDictionary<string, object> crossProcessBag)
		{
			long traceId, parentSpanId;

			if (TraceContextPropagation.TryObtainTraceIdFrom(crossProcessBag, out traceId, out parentSpanId))
			{
				_watch = Stopwatch.StartNew();

				Span = new Span(traceId, name, RandomHelper.NewId())
				{
					ParentId = parentSpanId
				}
				.AnnotateWith(PredefinedTag.ServerRecv, name)
				;

				TraceContextPropagation.PushSpan(Span);

				ZipkinConfig.Record(Span);
			}
		}

		public Span Span;

		public void Dispose()
		{
			if (Span != null)
			{
				Span.DurationInMicroseconds = _watch.ElapsedMilliseconds * 1000;

				TraceContextPropagation.PopSpan(this.Span);

				ZipkinConfig.Record(Span);
			}
			Span = null;
		}
	}
}