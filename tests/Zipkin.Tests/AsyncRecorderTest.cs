namespace Zipkin.Tests
{
	using System.Threading;
	using FluentAssertions;
	using NUnit.Framework;

	[TestFixture]
	public class AsyncRecorderTest
	{
		private StubDispatcher _stubDispatcher;
		private StubRecorderMetrics _metrics;
		private AsyncRecorder _recorder;

		[SetUp]
		public void Init()
		{
			_stubDispatcher = new StubDispatcher();
			_metrics = new StubRecorderMetrics();

			_recorder = new AsyncRecorder(_stubDispatcher, _metrics);
		}

		[TearDown]
		public void Terminate()
		{
			_recorder.Dispose();
		}

		[Test]
		public void Record_Should_update_metrics()
		{
			// Act
			_recorder.Record(new Span(1, "name", 2));

			// Assert
			_metrics.DroppedCount.Should().Be(0);
			_metrics.ErrorCount.Should().Be(0);
			_metrics.EnqueuedCount.Should().Be(1);
		}

		[Test]
		public void Record_Should_dispatch()
		{
			// Act
			_recorder.Record(new Span(1, "name", 1));
			_recorder.Record(new Span(1, "name", 2));
			_recorder.Record(new Span(1, "name", 3));

			Thread.Sleep(100);

			// Assert
			_stubDispatcher.Dispatched.Should().HaveCount(3);
		}
	}
}