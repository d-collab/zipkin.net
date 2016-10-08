namespace Zipkin
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.CompilerServices;
	using System.Runtime.Remoting.Messaging;
	using System.Threading;


	public static class TraceContextPropagation
	{
		private const string TraceIdKey = "_zipkin_traceid";

		private static readonly AsyncLocal<Stack<Span>> LocalSpanStack = new AsyncLocal<Stack<Span>>();

		public static bool IsWithinTrace
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get { return CurrentTraceId.HasValue; }
		}

		internal static long? CurrentTraceId
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get { return (long?) CallContext.LogicalGetData(TraceIdKey); }
			set
			{
				CallContext.LogicalSetData(TraceIdKey, value);
			}
		}

		public static void PropagateTraceIdOnto(IDictionary<string, string> dictionary)
		{
			if (IsWithinTrace)
			{
				
			}
		}

		public static void PropagateTraceIdOnto(IDictionary<string, object> dictionary)
		{
			if (IsWithinTrace)
			{
				dictionary[TraceIdKey] = CurrentTraceId.Value;
			}
		}

		public static bool TryObtainTraceIdFrom(IDictionary<string, string> dictionary)
		{
			return false;
		}

		public static bool TryObtainTraceIdFrom(IDictionary<string, object> dictionary)
		{
			object value;
			if (dictionary.TryGetValue(TraceIdKey, out value))
			{
				CurrentTraceId = Convert.ToInt64(value);
				return true;
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetRootTrace(Span span)
		{
			CallContext.LogicalSetData(TraceIdKey, span.TraceId);

			if (LocalSpanStack.Value == null)
			{
				LocalSpanStack.Value = new Stack<Span>();
			}

			LocalSpanStack.Value.Push(span);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void RemoveRootTrace(Span span)
		{
			CallContext.FreeNamedDataSlot(TraceIdKey);

			if (LocalSpanStack.Value != null && LocalSpanStack.Value.Count != 0)
			{
				var existing = LocalSpanStack.Value.Pop();

				if (existing.Id != span.Id)
				{
					// Order is messed up
					System.Diagnostics.Trace.Write("RemoveRootTrace: unexpected span in the context stack");
				}
			}
		}
	}
}