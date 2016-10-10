namespace Zipkin.Utils
{
	using System.Diagnostics;
	using System.Runtime.CompilerServices;

	public static class TickClock
	{
		/// <summary>
		/// CPU frequency converted to microseconds ratio
		/// </summary>
		private static readonly double TickFreq = 1000000.0 / (double)Stopwatch.Frequency;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long Start()
		{
			return Stopwatch.GetTimestamp();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long GetDuration(long start)
		{
			var now = Stopwatch.GetTimestamp();
			return (long) ((now - start) * TickFreq);
		}
	}
}