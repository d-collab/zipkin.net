namespace Zipkin
{
	using System;
	using System.Diagnostics;

	public struct StartClientTrace : IDisposable
	{
		private readonly Stopwatch _watch;
		private readonly bool _sampling;

		public StartClientTrace(string name)
		{
			_watch = Stopwatch.StartNew();
			_sampling = ZipkinConfig.ShouldSample();

			if (_sampling)
			{
				Span = new Span(RandomHelper.NewId(), name, RandomHelper.NewId());
				Span.AnnotateWithTag(PredefinedTag.ClientSend);

				TraceContextPropagation.PushSpan(Span);
			}
			else
			{
				Span = new Span(1, string.Empty, 1);
			}
		}

		public Span Span;

		public void Dispose()
		{
			if (_sampling)
			{
				Span.DurationInMicroseconds = _watch.ElapsedMilliseconds * 1000;

				TraceContextPropagation.PopSpan(Span);

				ZipkinConfig.Record(Span);
			}
			Span = null;
		}
	}
}