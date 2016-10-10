namespace Zipkin
{
	using System;
	using Utils;

	/// <summary>
	/// An Annotation is used to record an occurance in time. 
	/// There’s a set of core annotations used to define the beginning and end of a request:
	/// 
	/// <para>
	/// * cs - Client Start. The client has made the request. This sets the beginning of the span.
	/// * sr - Server Receive: The server has received the request and will start processing it.The difference between this and cs will be combination of network latency and clock jitter.
	/// * ss - Server Send: The server has completed processing and has sent the request back to the client. The difference between this and sr will be the amount of time it took the server to process the request.
	/// * cr - Client Receive: The client has received the response from the server.This sets the end of the span.The RPC is considered complete when this annotation is recorded.
	/// </para>
	/// </summary>
	/// <remarks>
	/// Other annotations can be recorded during the request’s lifetime in order to provide further 
	/// insight. For instance adding an annotation when a server begins and ends an expensive computation 
	/// may provide insight into how much time is being spent pre and post processing the request 
	/// versus how much time is spent running the calculation.
	/// </remarks>
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
		/// Usually a short tag indicating an event, like 
		/// {@link Constants#SERVER_RECV "sr"}. or {@link Constants#ERROR "error"}
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