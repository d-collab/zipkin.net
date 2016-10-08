namespace Server
{
	using System;
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;
	using RabbitMqNext;
	using Zipkin;

	class ProgramServer
	{
		static void Main(string[] args)
		{
			new Zipkin.FluentZipkinBootstrapper("server_sample")
				.ZipkinAt("localhost")
				.WithSampleRate(1.0) // means log everything
				.Start();

			const string rpc_exchange = "zipkin_test_exchange";
			const string message_q = "zipkin_queue";

			var task = new Task(async () =>
			{
				var conn = await RabbitMqNext.ConnectionFactory.Connect("localhost");
				var channel = await conn.CreateChannel();

				await channel.ExchangeDeclare(rpc_exchange, "direct", durable: false, autoDelete: false, arguments: null, waitConfirmation: true);
				await channel.QueueDeclare(message_q, false, durable: false, autoDelete: false, arguments: null, waitConfirmation: true, exclusive: false);
				await channel.QueueBind(message_q, rpc_exchange, "rpc", null, true);


				await channel.BasicConsume(ConsumeMode.ParallelWithBufferCopy, (delivery) =>
				{
					using (new StartServerTrace("rpc", delivery.properties.Headers))
					{
						Console.WriteLine("Received call");

						var buffer = new byte[delivery.bodySize];
						delivery.stream.Read(buffer, 0, delivery.bodySize);
						var content = Encoding.UTF8.GetString(buffer);

						var prop = channel.RentBasicProperties();
						prop.CorrelationId = delivery.properties.CorrelationId;

						channel.BasicPublishFast("", delivery.properties.ReplyTo, false, prop, Encoding.UTF8.GetBytes(content + " back to you!"));

						Console.WriteLine("Reply sent");
					}

					return Task.CompletedTask;

				}, message_q, null, true, false, null, true);

				Console.CancelKeyPress += (sender, eventArgs) =>
				{
					channel.Dispose();
					conn.Dispose();
					Console.WriteLine("Goodbye from server!");
				};

			});
			task.Start();

			task.Wait();

			Thread.CurrentThread.Join();
		}
	}
}
