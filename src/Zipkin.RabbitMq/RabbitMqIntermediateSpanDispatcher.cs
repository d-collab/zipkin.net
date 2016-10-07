namespace Zipkin.RabbitMq
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;

	/// <summary>
	/// Publishes serialized Spans to a RabbitMQ exchange, 
	/// which is a faster operation when compared to a REST call or Scribe rpc, 
	/// thus wont consume too many precious CPU cycles.
	/// 
	/// <para>
	/// However, since it's an intermediate step, it still necessary to hookup a 
	/// different service to shove that queue content into Zipkin service. 
	/// </para>
	/// </summary>
	public class RabbitMqIntermediateSpanDispatcher : SpanDispatcher
	{
		public RabbitMqIntermediateSpanDispatcher(RabbitMqEndpointConfig config)
		{
			throw new NotImplementedException();
		}

		public override Task DispatchSpans(IList<Span> spans)
		{
			throw new NotImplementedException();
		}

		public override void Dispose()
		{
		}
	}
}
