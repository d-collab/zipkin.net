namespace ApiUsage
{
	using System.Collections.Generic;
	using System.Net;
	using System.Threading;
	using Zipkin;

	class Program
	{
		static void Main(string[] args)
		{
			new Zipkin.ZipkinBootstrapper("api-sample", IPAddress.Loopback, 1234)
				.ZipkinAt("localhost")
				.WithSampleRate(1.0) // means log everything
				.Start();


			using (var roottrace = new StartClientTrace("client-op")) // Starts a root trace + span
			{
				var crossProcessBag = new Dictionary<string,object>();
				TraceContextPropagation.PropagateTraceIdOnto(crossProcessBag);

				Thread.Sleep(20);

				roottrace.TimeAnnotateWith("custom");

				using (new StartServerTrace("server-op", crossProcessBag).SetLocalComponentName("fake-server"))
				{
					using (new LocalTrace("op1").AnnotateWith(PredefinedTag.SqlQuery, "select * from  ..."))
					{
						Thread.Sleep(70);
					}

					using (var trace = new LocalTrace("op2"))
					{
						Thread.Sleep(90);

						trace.AnnotateWith(PredefinedTag.Error, "error message"); // mark it with an error
					}

					using (new LocalTrace("op3").TimeAnnotateWith(PredefinedTag.ServerSend))
					{
						Thread.Sleep(90);
					}

				}
			}

			Thread.Sleep(1000);
		}
	}
}
