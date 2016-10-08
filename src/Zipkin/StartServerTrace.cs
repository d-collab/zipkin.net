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
			if (TraceContextPropagation.TryObtainTraceIdFrom(crossProcessBag))
			{
				_watch = Stopwatch.StartNew();
				Span = new Span(TraceContextPropagation.CurrentTraceId.Value, name, RandomHelper.NewId());
				Span.AnnotateWithTag(PredefinedTag.ServerRecv);

				TraceContextPropagation.SetRootTrace(Span);
			}
		}

		public Span Span;

		public void Dispose()
		{
			if (Span != null)
			{
				Span.DurationInMicroseconds = _watch.ElapsedMilliseconds * 1000;

				TraceContextPropagation.RemoveRootTrace(Span);

				ZipkinConfig.Record(Span);
			}
			Span = null;
		}
	}
}