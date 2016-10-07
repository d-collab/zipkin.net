namespace Zipkin
{
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Threading;

	public class AsyncRecorder : Recorder
	{
		private readonly SpanDispatcher _dispatcher;
		private readonly RecorderMetrics _metrics;
		private ConcurrentQueue<Span> _enqueuedSpans = new ConcurrentQueue<Span>();
		private AutoResetEvent _spansEnqueuedEvent = new AutoResetEvent(false);
		private volatile int _disposed;

		private const int DisposedSet = 1;
		private const int MaxQueueSize = 1000;

		public AsyncRecorder(SpanDispatcher dispatcher, RecorderMetrics metrics)
		{
			_dispatcher = dispatcher;
			_metrics = metrics;

			new Thread(ThreadWorkProc)
			{
				IsBackground = true,
				Name = "ZipkinRecorder"
			}.Start();
		}

		public override void Record(params Span[] spans)
		{
			if (_disposed == DisposedSet || _enqueuedSpans.Count + spans.Length >= MaxQueueSize)
			{
				_metrics.Dropping(spans.Length);
				return;
			}

			foreach (var span in spans)
			{
				if (span == null) continue;

				_enqueuedSpans.Enqueue(span);
			}

			_spansEnqueuedEvent.Set();

			_metrics.Enqueued(spans.Length);
		}

		public override void Dispose()
		{
			if (Interlocked.CompareExchange(ref _disposed, DisposedSet, 0) == 0)
			{
				// releases thread so it can clean exit
				_spansEnqueuedEvent.Set();
			}
		}

		private void ThreadWorkProc()
		{
			try
			{
				var spanList = new List<Span>();

				while (true)
				{
					_spansEnqueuedEvent.WaitOne();

					if (_disposed == DisposedSet)
					{
						_spansEnqueuedEvent.Dispose();
						_enqueuedSpans = null;
						_spansEnqueuedEvent = null;
						break;
					}

					Span span;
					while (_enqueuedSpans.TryDequeue(out span))
					{
						spanList.Add(span);
					}

					try
					{
						_dispatcher.DispatchSpans(spanList).Wait();
					}
					catch (Exception ex)
					{
						_metrics.ErrorDispatching(ex);
					}

					spanList.Clear();
				}
			}
			catch (Exception ex)
			{
				_metrics.ErrorDispatching(ex);
			}
		}
	}
}
