namespace Zipkin
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Net.Http;
	using System.Net.Http.Headers;
	using System.Threading.Tasks;

	/// <summary>
	/// Connects to the zipkin http endpoint, using the specified endpoint information 
	/// and <see cref="Codec"/> to dispatch <see cref="Span"/> instances. 
	/// </summary>
	/// 
	/// <remarks>
	/// Assumption: <see cref="DispatchSpans"/> is not used concurrently.
	/// </remarks>
	public class RestSpanDispatcher : SpanDispatcher
	{
		private readonly HttpClient _client;
		private readonly Codec _codec;
		private readonly string _path;
		private readonly MemoryStream _stream;

		public RestSpanDispatcher(Codec codec, string endpoint, int port, string path = "/api/v1/spans")
		{
			_codec = codec;
			_path = path;
			_stream = new MemoryStream();

			_client = new HttpClient()
			{
				BaseAddress = new Uri($"http://{endpoint}:{port}")
			};

			_client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("curl", "7.30.0"));
			_client.DefaultRequestHeaders.ConnectionClose = false;
		}

		public override async Task DispatchSpans(IList<Span> spans)
		{
			_stream.Position = 0;
			_stream.SetLength(0);

			_codec.WriteSpans(spans, _stream);

			_stream.Position = 0;

			var content = new StreamContent(_stream)
			{
				Headers = { ContentType = new MediaTypeHeaderValue(_codec.ContentType) }
			};

			await _client.PostAsync(_path, content);
		}

		public override void Dispose()
		{
			_client.Dispose();
		}
	}
}