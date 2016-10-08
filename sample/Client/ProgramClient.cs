namespace Client
{
	using System;
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;
	using RabbitMqNext;
	using Zipkin;

	class ProgramClient
	{
		static void Main(string[] args)
		{
			new Zipkin.FluentZipkinBootstrapper("client_sample")
				.ZipkinAt("localhost")
				.WithSampleRate(1.0) // means log everything
				.Start();

			const string rpc_exchange = "zipkin_test_exchange";

			var task = new Task<Task<bool>>(async (_) =>
			{
				var conn = await RabbitMqNext.ConnectionFactory.Connect("localhost");
				var channel = await conn.CreateChannel();

				await channel.ExchangeDeclare(rpc_exchange, "direct", durable: false, autoDelete: false, arguments: null, waitConfirmation: true);

				var rpc = await channel.CreateRpcHelper(ConsumeMode.ParallelWithBufferCopy, 1000 * 60 * 5);

				var prop = channel.RentBasicProperties();

				using (new StartClientTrace("client:op")) // Starts a root trace + span
				{
					// Trace id set to be passed to the server side
					TraceContextPropagation.PropagateTraceIdOnto(prop.Headers);

					Console.WriteLine("Sending RPC call");

					var result = await rpc.Call(rpc_exchange, "rpc", prop, Encoding.UTF8.GetBytes("hello world!"));

					Console.WriteLine("Reply received");
				}

				conn.Dispose();

				return true;
			}, null);
			task.Start();
			task.Result.Wait();

			Thread.Sleep(1000);

			Console.WriteLine("Goodbye from client!");
		}
	}
}
