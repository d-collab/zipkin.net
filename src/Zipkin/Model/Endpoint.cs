namespace Zipkin
{
	using System;
	using System.Net;

	/// <summary>
	/// Indicates the network context of a service recording an annotation with two exceptions.
	/// When a BinaryAnnotation, and key is {@link Constants#CLIENT_ADDR} or {@link Constants#SERVER_ADDR}, 
	/// the endpoint indicates the source or destination of an RPC. This exception allows zipkin 
	/// to display network context of uninstrumented services, or clients such as web browsers.
	/// </summary>
	public class Endpoint : IEquatable<Endpoint>
	{
		public IPAddress IPAddress;

		public ushort? Port;

		/// <summary>
		/// Classifier of a source or destination in lowercase, such as "zipkin-server".
		/// </summary>
		/// <remarks>
		/// This is the primary parameter for trace lookup, so should be intuitive 
		/// as possible, for example, matching names in service discovery.
		/// </remarks>
		public string ServiceName;


		#region Equality - for unit test

		bool IEquatable<Endpoint>.Equals(Endpoint other)
		{
			return other != null &&
					other.ServiceName == this.ServiceName &&
					other.Port == this.Port &&
					(this.IPAddress != null && other.IPAddress != null && other.IPAddress.Equals(this.IPAddress));
		}

		public static bool operator ==(Endpoint lhs, Endpoint rhs)
		{
			return (ReferenceEquals(lhs, null) && ReferenceEquals(rhs, null)) || 
					!ReferenceEquals(lhs, null) &&
					!ReferenceEquals(rhs, null) &&
					lhs.Equals(rhs);
		}

		public static bool operator !=(Endpoint lhs, Endpoint rhs)
		{
			return !(lhs == rhs);
		}

		public override bool Equals(object obj)
		{
			return (this as IEquatable<Endpoint>).Equals(obj as Endpoint);
		}

		#endregion
	}
}