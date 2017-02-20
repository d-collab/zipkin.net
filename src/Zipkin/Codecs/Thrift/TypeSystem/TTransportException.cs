namespace Zipkin.Codecs.Thrift.TypeSystem
{
	using System;

	public class TTransportException : TException
	{
		protected ExceptionType type;

		public TTransportException()
		{
		}

		public TTransportException(ExceptionType type)
		{
			this.type = type;
		}

		public TTransportException(ExceptionType type, string message) : base(message)
		{
			this.type = type;
		}

		public TTransportException(string message) : base(message)
		{
		}

		public ExceptionType Type
		{
			get { return type; }
		}

		public enum ExceptionType
		{
			Unknown,
			NotOpen,
			AlreadyOpen,
			TimedOut,
			EndOfFile,
			Interrupted
		}
	}
}