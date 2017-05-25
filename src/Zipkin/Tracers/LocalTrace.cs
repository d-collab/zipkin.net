namespace Zipkin
{
	using Utils;

	/// <summary>
	/// Represents a local (within service) trace of an activity. 
	/// </summary>
	public struct LocalTrace : ITrace
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
			var parentSpan = TraceContextPropagation.CurrentSpan;
//			if (TraceContextPropagation.IsWithinTrace)
			if (parentSpan != null)
			{
				// var parentSpan = TraceContextPropagation.CurrentSpan;

				this.Span = new Span(parentSpan.TraceId, name, RandomHelper.NewId())
				{
					ParentId = parentSpan.Id
				};

				this._start = TickClock.Start();

				TraceContextPropagation.PushSpan(this.Span);
			}
			else
			{
				this.Span = null;
				this._start = 0;
			}
		}

		public void Dispose()
		{
			if (Span != null)
			{
				Span.DurationInMicroseconds = TickClock.GetDuration(_start);

				TraceContextPropagation.PopSpan();

				ZipkinConfig.Record(Span);

				Span = null;
			}
		}
	}
}