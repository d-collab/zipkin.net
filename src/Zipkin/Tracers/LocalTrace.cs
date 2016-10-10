namespace Zipkin
{
	using Utils;

	/// <summary>
	/// Represents a local (within service) trace of an activity. 
	/// </summary>
	public class LocalTrace : ITrace
	{
		private readonly long _start;

		public Span Span { get; private set; }

		/// <summary>
		/// Creates a local span.
		/// Give it a short lower-case description of the activity
		/// </summary>
		/// <param name="name"></param>
		public LocalTrace(string name)
		{
			if (TraceContextPropagation.IsWithinTrace)
			{
				_start = TickClock.Start();

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
				Span.DurationInMicroseconds = TickClock.GetDuration(_start);

				TraceContextPropagation.PopSpan(this.Span);

				ZipkinConfig.Record(Span);

				Span = null;
			}
		}
	}
}