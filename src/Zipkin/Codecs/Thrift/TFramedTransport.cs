namespace Zipkin.Codecs.Thrift
{
	using System;

	public class TFramedTransport : /*TTransport,*/ IDisposable
	{

		public void Dispose()
		{
			// Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
		}
	}
}