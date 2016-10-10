namespace Zipkin
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Net.Sockets;
	using System.Runtime.CompilerServices;
	using System.Threading.Tasks;
	using Codecs.Thrift;
	using Codecs.Thrift.Scribe;

	/// <summary>
	/// Connects to the Scribe endpoint exposed by the zipkin server, and posts spans
	/// wrapped in a <see cref="LogEntry"/> instance. In this case, the message content 
	/// of the log entry is a base64 representation of a single <see cref="Span"/>
	/// serialized by <see cref="ThriftCodec"/>.
	/// </summary>
	/// 
	/// <remarks>
	/// Assumption: <see cref="DispatchSpans"/> is not used concurrently.
	/// </remarks>
	public class ScribeSpanDispatcher : SpanDispatcher
	{
		// TODO: needs to be more robust

		private readonly string _hostname;
		private readonly int _port;
		private readonly ScribeSpanConverter _converter = new ScribeSpanConverter();

		private readonly ScribeService _scribeService;
		private readonly TcpClient _tcpClient;
		private readonly TFramedTransport _frame;

		public ScribeSpanDispatcher(string hostname, int port)
		{
			_hostname = hostname;
			_port = port;

			_tcpClient = new TcpClient();
			_tcpClient.Connect(hostname, port);

			_frame = new TFramedTransport(new TTcpClientTransport(_tcpClient));
			_scribeService = new ScribeService(new ThriftProtocol(_frame));
		}

		public override async Task DispatchSpans(IList<Span> spans)
		{
			var logEntries = _converter.ToLogEntries(spans);

			await _scribeService.Log(logEntries).ConfigureAwait(false);
		}

		public override void Dispose()
		{
			_frame.Dispose();
			_tcpClient.Dispose();
		}

		public class ScribeSpanConverter
		{
			private readonly ThriftCodec _thriftCodec = new ThriftCodec();

			private readonly MemoryStream _stream = new MemoryStream();

			public IList<LogEntry> ToLogEntries(IList<Span> spans)
			{
				var logEntries = new List<LogEntry>(capacity: spans.Count);

				for (int i = 0; i < spans.Count; i++)
				{
					_stream.Position = 0;
					_stream.SetLength(0);

					var span = spans[i];
					_thriftCodec.WriteSpan(span, _stream);

					var messageContent = ToBase64(_stream);

					logEntries.Add(new LogEntry { message = messageContent });
				}

				return logEntries;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private static string ToBase64(MemoryStream stream)
			{
				return Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length);
			}
		}
	}
}