namespace Zipkin
{
	using System;

	public interface ITrace : IDisposable
	{
		Span Span { get; }
	}
}