namespace Zipkin
{

	public class FluentZipkinBootstrapper
	{
		private SpanDispatcher _dispatcher;

		public FluentZipkinBootstrapper()
		{
		}

		public FluentZipkinBootstrapper WithSampleRate(float sampleRate)
		{
			return this;
		}

		public FluentZipkinBootstrapper DispatchTo(SpanDispatcher dispatcher)
		{
			_dispatcher = dispatcher;
			return this;
		}

		public void Start()
		{
			// Build stuff
		}
	}
}