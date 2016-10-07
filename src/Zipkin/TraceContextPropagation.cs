namespace Zipkin
{
	using System.Collections.Generic;


	public static class TraceContextPropagation
	{
		public static bool HasContext
		{
			get { return false; }
		}

		public static void PropagateOnto(IDictionary<string, string> dictionary)
		{
		}
		public static void PropagateOnto(IDictionary<string, object> dictionary)
		{
		}

		public static bool TryObtainFrom(IDictionary<string, string> dictionary)
		{
			return false;
		}

		public static bool TryObtainFrom(IDictionary<string, object> dictionary)
		{
			return false;
		}
	}
}