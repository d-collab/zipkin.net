namespace Zipkin.Tests
{
	using System;
	using System.Threading;
	using System.Threading.Tasks;
	using FluentAssertions;
	using NUnit.Framework;

	[TestFixture]
	public class TraceContextPropagation_PushPop
	{
		[SetUp]
		public void Init()
		{
			TraceContextPropagation.Reset();
		}

		[Test]
		public async Task PushPop_no_thread_switch()
		{
			var span = new StartClientTrace("span1", isDebug: true);

			// TraceContextPropagation.IsWithinTrace.Should().BeTrue();
			var current = TraceContextPropagation.CurrentSpan;
			current.Should().Be(span.Span);

			await ImmediateRet();

			span.Dispose();

			// TraceContextPropagation.IsWithinTrace.Should().BeFalse();
			current = TraceContextPropagation.CurrentSpan;
			current.Should().BeNull();
		}

		[Test]
		public async Task PushPop_pop_on_thread_poll()
		{
			var span = new StartClientTrace("span2", isDebug: true);

			// TraceContextPropagation.IsWithinTrace.Should().BeTrue();
			var current = TraceContextPropagation.CurrentSpan;
			current.Should().Be(span.Span);

			await CompleteOnThreadPool();

			span.Dispose();

			// TraceContextPropagation.IsWithinTrace.Should().BeFalse();
			current = TraceContextPropagation.CurrentSpan;
			current.Should().BeNull();
		}

		[Test]
		public async Task PushPop_pop_on_thread_poll2()
		{
			var span = new StartClientTrace("span3", isDebug: true);

			// TraceContextPropagation.IsWithinTrace.Should().BeTrue();
			var current = TraceContextPropagation.CurrentSpan;
			current.Should().Be(span.Span);

			await CompleteOnThreadPool2();

			span.Dispose();

			// TraceContextPropagation.IsWithinTrace.Should().BeFalse();
			current = TraceContextPropagation.CurrentSpan;
			current.Should().BeNull();
		}

		[Test]
		public async Task PushPop_pop_on_task()
		{
			var span = new StartClientTrace("span4", isDebug: true);

			// TraceContextPropagation.IsWithinTrace.Should().BeTrue();
			var current = TraceContextPropagation.CurrentSpan;
			current.Should().Be(span.Span);

			await CompleteWithinSpawnedTask();

			span.Dispose();

			// TraceContextPropagation.IsWithinTrace.Should().BeFalse();
			current = TraceContextPropagation.CurrentSpan;
			current.Should().BeNull();
		}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
		private async Task<bool> ImmediateRet()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
		{
			return true;
		}

		private Task<bool> CompleteOnThreadPool()
		{
			var tcs = new TaskCompletionSource<bool>();
			var local = new LocalTrace("childspan1");

			ThreadPool.QueueUserWorkItem((_) =>
			{
				Thread.Sleep(50);
				local.Dispose();
				tcs.SetResult(true);
			});

			return tcs.Task;
		}

		private Task<bool> CompleteOnThreadPool2()
		{
			var tcs = new TaskCompletionSource<bool>();

			ThreadPool.UnsafeQueueUserWorkItem((_) =>
			{
				Thread.Sleep(50);
				tcs.SetResult(true);
			}, null);

			return tcs.Task;
		}

		private Task<bool> CompleteWithinSpawnedTask()
		{
			return Task.Run(new Func<bool>(() =>
			{
				Thread.Sleep(50);

				return true;
			}));
		}
	}
}