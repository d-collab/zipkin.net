namespace Zipkin
{
	/// <summary>
	/// Helper trace that is either a <see cref="LocalTrace"/> if we are within a trace
	/// or a <see cref="StartClientTrace"/> if we are not. 
	/// </summary>
	public struct StartClientOrContinueTrace : ITrace
	{
		public ITrace Inner;

		public StartClientOrContinueTrace(string name)
		{
			if (TraceContextPropagation.IsWithinTrace)
			{
				this.Inner = new LocalTrace(name);
			}
			else
			{
				this.Inner = new StartClientTrace(name);
			}
		}

		public void SkipDuration()
		{
			(Inner as StartClientTrace?)?.SkipDuration();
		}

		public Span Span => this.Inner?.Span;

		public void Dispose()
		{
			Inner?.Dispose();
		}
	}
}