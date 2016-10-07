namespace Zipkin
{
	using System.Net;

	/// <summary>
	/// Indicates the network context of a service recording an annotation with two exceptions.
	/// </summary>
	/// <remarks>
	/// When a BinaryAnnotation, and key is {@link Constants#CLIENT_ADDR} or {@link Constants#SERVER_ADDR}, 
	/// the endpoint indicates the source or destination of an RPC. This exception allows zipkin 
	/// to display network context of uninstrumented services, or clients such as web browsers.
	/// </remarks>
	public class Endpoint
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
	}
}