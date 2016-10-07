namespace Zipkin.Codecs.Thrift.Scribe
{
	public class LogEntry
	{
		public string category => "zipkin";
		public string message;
	}
}