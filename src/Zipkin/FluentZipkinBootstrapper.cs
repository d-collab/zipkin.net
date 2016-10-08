namespace Zipkin
{
	using System;

	internal static class ZipkinConfig
	{
		internal static double _sampleRate;
		internal static Endpoint ThisService;
		internal static AsyncRecorder Recorder;

		public static bool ShouldSample()
		{
			return RandomHelper.Sample(_sampleRate);
		}

		public static void Record(Span span)
		{
			Recorder?.Record(span);
		}
	}

	public class FluentZipkinBootstrapper
	{
		internal static FluentZipkinBootstrapper SingleSink;

		private SpanDispatcher _dispatcher;
		private RecorderMetrics _metrics;
		private int _httpPort, _scribePort;
		private string _zipkinHostname;
		private Codec _codec;
		private readonly string _serviceName;

		public FluentZipkinBootstrapper(string serviceName)
		{
			_serviceName = serviceName;
			_httpPort = 9411;
			_scribePort = 9410;
			_zipkinHostname = "localhost";
		}

		public FluentZipkinBootstrapper WithSampleRate(double sampleRate)
		{
			if (sampleRate < 0.0 || sampleRate > 1.0)
				throw new ArgumentOutOfRangeException(nameof(sampleRate), "sample rate should be between 0.0 and 1.0");

			ZipkinConfig._sampleRate = sampleRate;

			return this;
		}

		public FluentZipkinBootstrapper ZipkinAt(string zipkinHostname, int httpPort = 9411, int scribePort = 9410)
		{
			_zipkinHostname = zipkinHostname;
			_httpPort = httpPort;
			_scribePort = scribePort;
			return this;
		}

		public FluentZipkinBootstrapper DispatchTo(SpanDispatcher dispatcher)
		{
			_dispatcher = dispatcher;
			return this;
		}

		public FluentZipkinBootstrapper WithCodec(Codec codec)
		{
			_codec = codec;
			return this;
		}

		public FluentZipkinBootstrapper WithMetrics(RecorderMetrics metrics)
		{
			_metrics = metrics;
			return this;
		}

		public void Start()
		{
			var codec = _codec ?? new ThriftCodec();
			var dispatcher = _dispatcher ?? new RestSpanDispatcher(codec, _zipkinHostname, _httpPort);
			var metrics = _metrics ?? new RecorderMetrics();

			ZipkinConfig.ThisService = new Endpoint()
			{
				ServiceName = _serviceName
			};

			ZipkinConfig.Recorder = new AsyncRecorder(dispatcher, metrics);
		}
	}
}