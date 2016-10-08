namespace Zipkin
{
	using System.Diagnostics;

	/// <summary>
	/// Represents a local (within service) trace of an activity. 
	/// </summary>
	public class LocalTrace : ITrace
	{
		private readonly Stopwatch _watch;

		public Span Span { get; private set; }

		/// <summary>
		/// Give it a short lower-case description of the activity
		/// </summary>
		/// <param name="name"></param>
		public LocalTrace(string name)
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

	public static class NanoClock
	{
		
	}
}