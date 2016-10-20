namespace Zipkin
{
	/// <summary>
	/// Gives you access to the current contextual span (if any)
	/// so annotations can be added
	/// </summary>
	public class TraceAnnotate : ITrace
	{
		public Span Span { get; private set; }

		public TraceAnnotate()
		{
			Span = TraceContextPropagation.CurrentSpan;
		}

		public void Dispose()
		{
		}
	}
}