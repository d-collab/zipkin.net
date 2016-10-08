namespace Zipkin
{
	using System;
	using System.Diagnostics;
	using Utils;

	/// <summary>
	/// Starts a trace. 
	/// 
	/// <para>The trace is subject to the sampling rate configured for the process.</para>
	/// 
	/// The Span is also annotated with <see cref="AnnotationConstants.CLIENT_SEND"/> as per guidelines.
	/// </summary>
	public class StartClientTrace : ITrace
	{
		private readonly long _start;
		private readonly bool _sampling;

		public Span Span { get; private set; }

		/// <summary>
		/// Give it a short lower-case description of the activity
		/// </summary>
		public StartClientTrace(string name)
		{
			_sampling = ZipkinConfig.ShouldSample();

			if (_sampling)
			{
				_start = NanoClock.Start();

				Span = new Span(RandomHelper.NewId(), name, RandomHelper.NewId());
				this.AnnotateWithTag(PredefinedTag.ClientSend);

				TraceContextPropagation.PushSpan(Span);
			}
		}

		public void Dispose()
		{
			if (_sampling)
			{
				Span.DurationInMicroseconds = NanoClock.GetDuration(_start);

				TraceContextPropagation.PopSpan(Span);

				ZipkinConfig.Record(Span);

				Span = null;
			}
		}
	}
}