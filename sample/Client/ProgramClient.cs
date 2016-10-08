namespace Client
{
	using System;
	using System.Collections.Generic;
	using System.Net;
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;
	using RabbitMqNext;
	using Zipkin;

	class ProgramClient
	{
		static void Main(string[] args)
		{
			new Zipkin.ZipkinBootstrapper("client-sample", IPAddress.Loopback, 1234)
				.ZipkinAt("localhost")
				.WithSampleRate(1.0) // means log everything
				.Start();

//			const string rpc_exchange = "zipkin_test_exchange";
//
//			var task = new Task<Task<bool>>(async (_) =>
//			{
//				var conn = await RabbitMqNext.ConnectionFactory.Connect("localhost");
//				var channel = await conn.CreateChannel();
//
//				await channel.ExchangeDeclare(rpc_exchange, "direct", durable: false, autoDelete: false, arguments: null, waitConfirmation: true);
//
//				var rpc = await channel.CreateRpcHelper(ConsumeMode.ParallelWithBufferCopy, 1000 * 60 * 5);
//
//				var prop = channel.RentBasicProperties();
//
//				using (new StartClientTrace("client-op")) // Starts a root trace + span
//				{
//					// Trace id set to be passed to the server side
//					TraceContextPropagation.PropagateTraceIdOnto(prop.Headers);
//
//					Console.WriteLine("Sending RPC call");
//
//					var result = await rpc.Call(rpc_exchange, "rpc", prop, Encoding.UTF8.GetBytes("hello world!"));
//
//					Console.WriteLine("Reply received");
//				}
//
//				conn.Dispose();
//
//				return true;
//			}, null);
//			task.Start();
//			task.Result.Wait();


			using (new StartClientTrace("client-op")) // Starts a root trace + span
			{
				var crossProcessBag = new Dictionary<string,object>();
				TraceContextPropagation.PropagateTraceIdOnto(crossProcessBag);

				Thread.Sleep(20);

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

					using (new LocalTrace("op2").AnnotateWithTag(PredefinedTag.ServerSend))
					{
						Thread.Sleep(90);
					}

				}
			}

			// Thread.Sleep(1000);
			Thread.CurrentThread.Join();

			Console.WriteLine("Goodbye from client!");
		}
	}
}
