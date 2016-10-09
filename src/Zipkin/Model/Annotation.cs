namespace Zipkin
{
	using System;

	public class Annotation : IEquatable<Annotation>
	{
		public Annotation()
		{
			Timestamp = DateTimeOffset.UtcNow;
		}

		/// <summary>
		/// The host that recorded {@link #value}, primarily for query by service name.
		/// </summary>
		public Endpoint Host;

		/// <summary>
		/// Microseconds from epoch.
		/// </summary>
		public DateTimeOffset Timestamp;

		/// <summary>
		/// Usually a short tag indicating an event, like {@link Constants#SERVER_RECV "sr"}. or {@link Constants#ERROR "error"}
		/// </summary>
		public string Value;


		#region Equality - for unit test

		bool IEquatable<Annotation>.Equals(Annotation other)
		{
			return !ReferenceEquals(other, null) && 
				   (other.Timestamp.UtcTicks / 10000) == (this.Timestamp.UtcTicks / 10000) &&  // lower precision
				   other.Value == this.Value &&
				   other.Host == this.Host;
		}

		public static bool operator == (Annotation lhs, Annotation rhs)
		{
			return (ReferenceEquals(lhs, null) && ReferenceEquals(rhs, null)) ||
					!ReferenceEquals(lhs, null) &&
					!ReferenceEquals(rhs, null) &&
					lhs.Equals(rhs);
		}

		public static bool operator !=(Annotation lhs, Annotation rhs)
		{
			return !(lhs == rhs);
		}

		public override bool Equals(object obj)
		{
			return (this as IEquatable<Annotation>).Equals(obj as Annotation);
		}

		#endregion
	}
}