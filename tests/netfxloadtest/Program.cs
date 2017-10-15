namespace netfxloadtest
{
	using System;
	using System.Diagnostics;
	using System.Threading;
	using System.Threading.Tasks;
	using FluentAssertions;
	using Zipkin;

	class Program
	{
		private static ManualResetEventSlim KickOffEvent = new ManualResetEventSlim(false);

		static void Main(string[] args)
		{
			// Console.WriteLine("Hello World!");

			var howManyThreads = 9;
			var threads = new Thread[howManyThreads];

			for (int i = 0; i < howManyThreads; i++)
			{
				threads[i] = new Thread(DoProcess)
				{
					IsBackground = true,
				};
				threads[i].Start(i);
			}

			// Start 

			var started = Stopwatch.StartNew();
			KickOffEvent.Set();

			// Wait

			for (int i = 0; i < howManyThreads; i++)
			{
				threads[i].Join();
			}

			// Done

			started.Stop();
			Console.WriteLine("Done - took " + started.Elapsed.TotalSeconds + " seconds");
		}

		private static void DoProcess(object obj)
		{
			var threadIndex = (int)obj;

			KickOffEvent.Wait();

			for (int i = 0; i < 100000; i++)
			{
				var result = Task.Run(async () =>
				{
					var rootSpan = new StartClientTrace("root_" + threadIndex, isDebug: true);

					var cur = TraceContextPropagation.CurrentSpan;
					rootSpan.Span.Should().Be(cur);

					await CompleteOnThreadPool();
					await CompleteWithinSpawnedTask();
					await CompleteWithCustomThread();

					rootSpan.Dispose();

					cur = TraceContextPropagation.CurrentSpan;
					cur.Should().BeNull();

					return true;

				}).Result;

				if (!result) break;
			}

			Console.WriteLine("Thread {0} done", threadIndex);
		}

		private static Task<bool> CompleteOnThreadPool()
		{
			var tcs = new TaskCompletionSource<bool>();
			var local = new LocalTrace("childspan1");

			var cur = TraceContextPropagation.CurrentSpan;
			local.Span.Should().Be(cur);

			ThreadPool.QueueUserWorkItem((_) =>
			{
				// Thread.Sleep(20);

				var cur2 = TraceContextPropagation.CurrentSpan;
				cur2.Should().Be(local.Span);

				local.Dispose();

				cur2 = TraceContextPropagation.CurrentSpan;
				// cur2.Should().Be(local.Span);

				tcs.SetResult(true);
			});

			return tcs.Task;
		}

		private static Task<bool> CompleteWithinSpawnedTask()
		{
			var local = new LocalTrace("childspan2");

			var cur = TraceContextPropagation.CurrentSpan;
			local.Span.Should().Be(cur);

			return Task.Run(new Func<bool>(() =>
			{
				// Thread.Sleep(20);

				var cur2 = TraceContextPropagation.CurrentSpan;
				cur2.Should().Be(local.Span);

				local.Dispose();

				cur2 = TraceContextPropagation.CurrentSpan;
				// cur2.Should().Be(local.Span);

				return true;
			}));
		}

		private static Task<bool> CompleteWithCustomThread()
		{
			var tcs = new TaskCompletionSource<bool>();
			var local = new LocalTrace("childspan3");

			var cur = TraceContextPropagation.CurrentSpan;
			local.Span.Should().Be(cur);

			new Thread(() =>
				{
					// Thread.Sleep(1);

					var cur2 = TraceContextPropagation.CurrentSpan;
					cur2.Should().Be(local.Span);

					local.Dispose();
					tcs.SetResult(true);

					cur2 = TraceContextPropagation.CurrentSpan;

				})
				{ IsBackground = true }.Start();

			return tcs.Task;
		}
	}
}
