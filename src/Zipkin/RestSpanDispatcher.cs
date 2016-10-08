namespace Zipkin
{
	using System;
	using System.Buffers;
	using System.Collections.Generic;
	using System.IO;
	using System.Net.Http;
	using System.Net.Http.Headers;
	using System.Threading.Tasks;

	/// <summary>
	/// Connects to the zipkin http endpoint, using the specified endpoint information 
	/// and <see cref="Codec"/> to dispatch <see cref="Span"/> instances. 
	/// </summary>
	public class RestSpanDispatcher : SpanDispatcher
	{
		private readonly HttpClient _client;
		private readonly Codec _codec;
		private readonly string _path;

		public RestSpanDispatcher(Codec codec, string endpoint, int port, string path = "/api/v1/spans")
		{
			_codec = codec;
			_path = path;

			_client = new HttpClient()
			{
				BaseAddress = new Uri($"http://{endpoint}:{port}")
			};

			_client.DefaultRequestHeaders.ConnectionClose = false;
		}

		public override async Task DispatchSpans(IList<Span> spans)
		{
			var buffer = ArrayPool<byte>.Shared.Rent(0x800000);

			try
			{
				using (var stream = new MemoryStream(buffer, writable: true))
				{
					_codec.WriteSpans(spans, stream);

					stream.Position = 0;

					var content = new StreamContent(stream)
					{
						Headers = { ContentType = new MediaTypeHeaderValue(_codec.ContentType) }
					};

					await _client.PostAsync(_path, content);
				}
			}
			finally
			{
				ArrayPool<byte>.Shared.Return(buffer);
			}
		}

		public override void Dispose()
		{
			_client.Dispose();
		}
	}
}