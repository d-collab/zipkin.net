namespace Zipkin.Tests
{
	using System;
	using Codecs;
	using FluentAssertions;
	using NUnit.Framework;

	[TestFixture]
	public class DateTimeOffsetExtensionsTest
	{
		[Test]
		public void ConversionToUnixEpochInMicroseconds_back_and_forward()
		{
			// Arrange
			var dt = new DateTimeOffset(2016, 7, 16, 16, 42, 11, 999, TimeSpan.FromHours(1));

			// Act
			var nixVersion = DateTimeOffsetExtensions.ToNixTimeMicro(dt);
			var restored = DateTimeOffsetExtensions.FromLong(nixVersion);

			// Assert
			restored.Should().Be(dt);
		}
	}
}