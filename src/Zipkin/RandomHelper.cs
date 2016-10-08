namespace Zipkin
{
	using System;
	using System.Runtime.CompilerServices;

	public static class RandomHelper
	{
		private static readonly Random Rnd = new Random();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long NewId()
		{
			return Rnd.Next();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Sample(double rate)
		{
			return Rnd.NextDouble() < rate;
		}
	}
}