namespace Zipkin
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Net.Sockets;
	using Codecs;
	using Codecs.Thrift;
	using Codecs.Thrift.Scribe;

	public class ThriftSpanDispatcher : SpanDispatcher
	{
		private readonly ScribeService _scribeService;

		private readonly ThriftCodec _thriftCodec = new ThriftCodec();

		public ThriftSpanDispatcher(string endpoint, int port)
		{
			_scribeService = new ScribeService(new ThriftProtocol(new TcpClient().GetStream()));
		}

		public override void DispatchSpans(IList<Span> spans)
		{
			try
			{
				if (spans.Count == 1)
				{
					// _thriftCodec.Encode( spans[i] ); 

					_scribeService.Log(new LogEntry() { message = "" });
				}
				else
				{
//					var entries = new List<LogEntry>(spans.Count);
//
//					for (int i = 0; i < spans.Count; i++)
//					{
//						entries[i] = _thriftCodec.Encode( spans[i] ); 
//					}
//
//					_scribeService.Log(entries);
				}
			}
			catch (Exception ex)
			{
				
			}
		}

		public override void Dispose()
		{
		}
	}
}