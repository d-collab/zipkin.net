namespace Zipkin
{
	using Utils;

	/// <summary>
	/// Starts a trace. 
	/// 
	/// <para>The trace is subject to the sampling rate configured for the process.</para>
	/// 
	/// The Span is also annotated with <see cref="StandardAnnotationKeys.ClientSend"/> as per guidelines.
	/// </summary>
	public class StartClientTrace : ITrace
	{
		private readonly long _start;
		private readonly bool _isSampling;
		private bool _skipDuration;

		public Span Span { get; private set; }

		/// <summary>
		/// Starts a client trace. 
		/// The <see cref="StandardAnnotationKeys.ClientSend"/> annotation is added by default.
		/// Give it a short lower-case description of the activity
		/// </summary>
		public StartClientTrace(string name)
		{
			_isSampling = ZipkinConfig.ShouldSample();

			if (_isSampling)
			{
				_start = TickClock.Start();

				Span = new Span(RandomHelper.NewId(), name, RandomHelper.NewId());
				this.TimeAnnotateWith(PredefinedTag.ClientSend);

				TraceContextPropagation.PushSpan(Span);
			}
		}

		public void SkipDuration()
		{
			_skipDuration = true;
		}

		public void Dispose()
		{
			if (_isSampling)
			{
				if (!_skipDuration)
					Span.DurationInMicroseconds = TickClock.GetDuration(_start);

				TraceContextPropagation.PopSpan(Span);

				ZipkinConfig.Record(Span);

				Span = null;
			}
		}
	}
}