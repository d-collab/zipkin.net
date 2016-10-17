namespace Zipkin
{
	using System;
	using System.Collections.Generic;
	using Utils;

	/// <summary>
	/// Starts a server trace, which is contingent on an existing client trace. 
	/// 
	/// The Span is also annotated with <see cref="StandardAnnotationKeys.ServerRecv"/> as per guidelines.
	/// </summary>
	public struct StartServerTrace : ITrace
	{
		private readonly long _start;

		public Span Span { get; private set; }

		/// <summary>
		/// Give it a short lower-case description of the activity
		/// </summary>
		public StartServerTrace(string name, IDictionary<string, object> crossProcessContext)
		{
			long traceId, parentSpanId;

			if (TraceContextPropagation.TryObtainTraceIdFrom(crossProcessContext, out traceId, out parentSpanId))
			{
				this._start = TickClock.Start();

				this.Span = new Span(traceId, name, RandomHelper.NewId())
				{
					ParentId = parentSpanId
				};
				this.TimeAnnotateWith(PredefinedTag.ServerRecv);

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