namespace Zipkin
{
	using System;
	using Model;

	/// <summary>
	/// Abstraction on strategy to record the Spans collected. 
	/// </summary>
	public abstract class Recorder : IDisposable
	{
		public abstract void Record(params Span[] spans);

		public abstract void Dispose();
	}
}
