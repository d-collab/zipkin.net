namespace Zipkin.Codecs.Thrift.TypeSystem
{
	using System;
	using System.Runtime.Serialization;

	[Serializable]
	public class TException : Exception
	{
		public TException()
		{
		}

		public TException(string message) : base(message)
		{
		}

		protected TException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}