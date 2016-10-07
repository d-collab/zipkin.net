namespace Zipkin.Codecs.Thrift
{
	using System.Net.Sockets;

	public class TTcpClientTransport : TStreamTransport
	{
		public TTcpClientTransport(TcpClient client) : base(client.GetStream())
		{
		}
	}
}