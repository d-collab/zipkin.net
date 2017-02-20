namespace Zipkin.Codecs.Thrift.TypeSystem
{
	using System;
	public class TException : Exception
	{
		public TException()
		{
		}

		public TException(string message) : base(message)
		{
		}
	}
}