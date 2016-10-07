namespace Zipkin.Codecs
{
	using System;

	internal static class DateTimeOffsetExtensions
	{
		public static readonly DateTimeOffset NixEpoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

		public static long ToNixTimeMicro(this DateTimeOffset source)
		{
			unchecked
			{
				return (long) (source - NixEpoch).TotalMilliseconds * 1000L;
			}
		}

		public static DateTimeOffset FromLong(long offsetInMicrosecsSinceEpoch)
		{
			var valInMs = offsetInMicrosecsSinceEpoch / 1000.0;

			return NixEpoch.Add(TimeSpan.FromMilliseconds(valInMs));
		}
	}
}