namespace Zipkin
{
	using System;
	using Model;

	// thrift:9410
	// : Tomcat started on port(s): 9411 (http)

	public abstract class Recorder : IDisposable
	{
		public abstract void Record(params Span[] spans);

		public abstract void Dispose();
	}
}
