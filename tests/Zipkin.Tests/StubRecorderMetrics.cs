namespace Zipkin.Tests
{
	using System;

	class StubRecorderMetrics : RecorderMetrics
	{
		public int EnqueuedCount, ErrorCount, DroppedCount;

		public override void ErrorDispatching(Exception error)
		{
			ErrorCount++;
		}

		public override void Enqueued(int howMany)
		{
			EnqueuedCount++;
		}

		public override void Dropping(int howMany)
		{
			DroppedCount++;
		}
	}
}