namespace Zipkin
{
	using System;
	using System.Diagnostics;


	public class TraceChild : IDisposable
	{
		private readonly Stopwatch _watch;

		public TraceChild(string name)
		{
			if (TraceContextPropagation.IsWithinTrace)
			{
				_watch = Stopwatch.StartNew();

				var parentSpan = TraceContextPropagation.CurrentSpan;

				Span = new Span(parentSpan.TraceId, name, RandomHelper.NewId())
				{
					ParentId = parentSpan.Id
				};

				TraceContextPropagation.PushSpan(this.Span);

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