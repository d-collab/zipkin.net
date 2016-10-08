namespace Zipkin
{
	using System;
	using System.Net;
	using System.Threading;

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

	public class ZipkinBootstrapper
	{
		internal static ZipkinBootstrapper SingleSink;

		private SpanDispatcher _dispatcher;
		private RecorderMetrics _metrics;
		private int _httpPort, _scribePort;
		private string _zipkinHostname;
		private Codec _codec;
		private readonly string _serviceName;
		private readonly IPAddress _thisServiceAddress;
		private readonly short _thisServicePort;
		private volatile int _started;

		public ZipkinBootstrapper(string serviceName, IPAddress thisServiceAddress = null, short thisServicePort = 0)
		{
			_serviceName = serviceName;
			_thisServiceAddress = thisServiceAddress;
			_thisServicePort = thisServicePort;
			_httpPort = 9411;
			_scribePort = 9410;
			_zipkinHostname = "localhost";
		}

		public ZipkinBootstrapper WithSampleRate(double sampleRate)
		{
			if (sampleRate < 0.0 || sampleRate > 1.0)
				throw new ArgumentOutOfRangeException(nameof(sampleRate), "sample rate should be between 0.0 and 1.0");

			ZipkinConfig._sampleRate = sampleRate;

			return this;
		}

		public ZipkinBootstrapper ZipkinAt(string zipkinHostname, int httpPort = 9411, int scribePort = 9410)
		{
			_zipkinHostname = zipkinHostname;
			_httpPort = httpPort;
			_scribePort = scribePort;
			return this;
		}

		public ZipkinBootstrapper DispatchTo(SpanDispatcher dispatcher)
		{
			_dispatcher = dispatcher;
			return this;
		}

		public ZipkinBootstrapper WithCodec(Codec codec)
		{
			_codec = codec;
			return this;
		}

		public ZipkinBootstrapper WithMetrics(RecorderMetrics metrics)
		{
			_metrics = metrics;
			return this;
		}

		public void Start()
		{
			if (_started == 1)
				throw new Exception("Already set up for this process. Only one set up is allowed per process (or appdomain)");

			Interlocked.Increment(ref _started);

			// Apply defaults
			var codec = _codec ?? new ThriftCodec();
			var dispatcher = _dispatcher ?? new RestSpanDispatcher(codec, _zipkinHostname, _httpPort);
			var metrics = _metrics ?? new RecorderMetrics();

			ZipkinConfig.ThisService = new Endpoint()
			{
				ServiceName = _serviceName,
				IPAddress = _thisServiceAddress,
				Port = (ushort) _thisServicePort
			};

			ZipkinConfig.Recorder = new AsyncRecorder(dispatcher, metrics);
		}
	}
}