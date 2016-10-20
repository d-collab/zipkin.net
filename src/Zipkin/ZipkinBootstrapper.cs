namespace Zipkin
{
	using System;
	using System.Configuration;
	using System.Globalization;
	using System.Net;
	using System.Runtime.CompilerServices;
	using System.Threading;
	using Utils;

	internal static class ZipkinConfig
	{
		internal static double _sampleRate;
		internal static Endpoint ThisService;
		internal static Recorder Recorder;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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

		/// <summary>
		/// Initialize the config with the current service name, ipaddress (if any) and port.
		/// </summary>
		public ZipkinBootstrapper(string serviceName, IPAddress thisServiceAddress = null, short thisServicePort = 0)
		{
			_serviceName = serviceName;
			_thisServiceAddress = thisServiceAddress;
			_thisServicePort = thisServicePort;
			_httpPort = 9411;
			_scribePort = 9410;
			_zipkinHostname = "localhost";
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static ZipkinBootstrapper BuildFromAppDomainConfig()
		{
			//< add key="ZipkinServerName"  value="localhost" />
			//< add key="ZipkinSampleRate"  value="0.5" />
			//< add key="ZipkinServiceName" value="pit" />

			var serviceName = ConfigurationManager.AppSettings["ZipkinServiceName"];
			var sampleRate = double.Parse(ConfigurationManager.AppSettings["ZipkinSampleRate"] ?? "0.0", CultureInfo.InvariantCulture);
			var zipkinServer = ConfigurationManager.AppSettings["ZipkinServerName"];

			if (serviceName == null)  throw new Exception("Missing configuration entry: 'ZipkinServiceName' under appsettings, which should define this service's name");
			if (zipkinServer == null) throw new Exception("Missing configuration entry: 'ZipkinServerName' under appsettings, which should point to zipkin server's hostname");

			return new ZipkinBootstrapper(serviceName)
				.WithSampleRate(sampleRate)
				.ZipkinAt(zipkinServer);
		}

		/// <summary>
		/// Allows one to change the sampling rate post set up
		/// </summary>
		public static void ChangeSamplingRate(double newRate)
		{
			if (newRate < 0.0 || newRate > 1.0) throw new ArgumentOutOfRangeException(nameof(newRate), "sample rate should be between 0.0 and 1.0");
			ZipkinConfig._sampleRate = newRate;
		}

		/// <summary>
		/// Establish the sample rate: between 0.0 and 1.0
		/// </summary>
		public ZipkinBootstrapper WithSampleRate(double sampleRate)
		{
			if (sampleRate < 0.0 || sampleRate > 1.0) throw new ArgumentOutOfRangeException(nameof(sampleRate), "sample rate should be between 0.0 and 1.0");

			ZipkinConfig._sampleRate = sampleRate;

			return this;
		}

		/// <summary>
		/// Use to define where the zipkin server is located.
		/// </summary>
		public ZipkinBootstrapper ZipkinAt(string zipkinHostname, int httpPort = 9411, int scribePort = 9410)
		{
			_zipkinHostname = zipkinHostname;
			_httpPort = httpPort;
			_scribePort = scribePort;
			return this;
		}

		/// <summary>
		/// Configures which span dispatcher to use. By default we'll use <see cref="RestSpanDispatcher"/>
		/// </summary>
		public ZipkinBootstrapper DispatchTo(SpanDispatcher dispatcher)
		{
			_dispatcher = dispatcher;
			return this;
		}

		/// <summary>
		/// Configures which codec to use. By default we'll use <see cref="ThriftCodec"/>
		/// </summary>
		/// <param name="codec"></param>
		/// <returns></returns>
		public ZipkinBootstrapper WithCodec(Codec codec)
		{
			_codec = codec;
			return this;
		}

		/// <summary>
		/// Configures which metrics collector to use. By default, nothing will be collected or logged. 
		/// </summary>
		/// <param name="metrics"></param>
		/// <returns></returns>
		public ZipkinBootstrapper WithMetrics(RecorderMetrics metrics)
		{
			_metrics = metrics;
			return this;
		}

		/// <summary>
		/// Sets up the system for the AppDomain.
		/// </summary>
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