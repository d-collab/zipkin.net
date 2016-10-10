namespace Zipkin.Tests
{
	using System.Threading;
	using FluentAssertions;
	using NUnit.Framework;
	using Utils;

	[TestFixture]
	public class NanoClockTest
	{
		[Test]
		public void GetDuration_returns_time_in_microseconds()
		{
			var start = TickClock.Start();

			Thread.Sleep(100);

			var duration = TickClock.GetDuration(start);

			duration.Should().BeInRange(100000, 105000);
		}
	}
}