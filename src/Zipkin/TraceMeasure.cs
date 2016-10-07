namespace Zipkin
{
	using System;

	public struct StartTrace : IDisposable
	{
		public StartTrace(string name)
		{
		}

		public void Dispose()
		{
		}
	}

//	public struct StartTraceUnbounded : IDisposable
//	{
//		public void Dispose()
//		{
//		}
//	}

	public struct WithinSpan : IDisposable
	{
		public WithinSpan(string activity)
		{
			if (!TraceContextPropagation.HasContext) return;
		}

		public void Dispose()
		{
		}
	}
}