namespace Zipkin
{
	using System;

	/// <summary>
	/// This is your bridge to plug your logging system or similar effect 
	/// into the Recorder internal ops
	/// </summary>
	public class RecorderMetrics
	{
		/// <summary>
		/// When an error happened invoking the dispatcher.
		/// </summary>
		/// <param name="error"></param>
		public virtual void ErrorDispatching(Exception error)
		{
		}

		/// <summary>
		/// Indicates that spans were enqueued for dispatching
		/// </summary>
		/// <param name="howMany">How many spans enqueued</param>
		public virtual void Enqueued(int howMany)
		{
		}

		/// <summary>
		/// Indicates that spans were discared (not recorded) 
		/// due to the queue size being past the max allowed.
		/// </summary>
		/// <param name="howMany">How many spans being dropped</param>
		public virtual void Dropping(int howMany)
		{
		}
	}
}