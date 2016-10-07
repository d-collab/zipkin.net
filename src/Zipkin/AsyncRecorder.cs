namespace Zipkin
{
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Threading;


	public class AsyncRecorder : Recorder
	{
		private readonly SpanDispatcher _dispatcher;
		private readonly ConcurrentQueue<Span> _enqueuedSpans = new ConcurrentQueue<Span>();
		private readonly AutoResetEvent _spansEnqueuedEvent = new AutoResetEvent(false);
		private int _disposed;

		public AsyncRecorder(SpanDispatcher dispatcher)
		{
			_dispatcher = dispatcher;

			new Thread(ThreadWorkProc)
			{
				IsBackground = true,
				Name = "ZipkinRecorder"
			}.Start();
		}

		public override void Record(params Span[] spans)
		{
			foreach (var span in spans)
			{
				_enqueuedSpans.Enqueue(span);
			}

			_spansEnqueuedEvent.Set();
		}

		public override void Dispose()
		{
			if (Interlocked.CompareExchange(ref _disposed, 1, 0) == 0)
			{
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
					spanList.Clear();

					_spansEnqueuedEvent.WaitOne();

					if (_disposed == 1) break;

					Span span;
					while (_enqueuedSpans.TryDequeue(out span))
					{
						spanList.Add(span);
					}

					_dispatcher.DispatchSpans(spanList);
				}
			}
			catch (Exception)
			{
			}
		}
	}
}