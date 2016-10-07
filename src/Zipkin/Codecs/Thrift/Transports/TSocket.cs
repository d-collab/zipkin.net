namespace Zipkin.Codecs.Thrift
{
	using System.Net.Sockets;

	public class TSocket : TStreamTransport
	{
		public TSocket(TcpClient client) : base(client.GetStream())
		{
		}
	}
}