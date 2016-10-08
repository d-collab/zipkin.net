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
		private const string SpanIdKey = "_zipkin_spanid";

		private static readonly AsyncLocal<Stack<Span>> LocalSpanStack = new AsyncLocal<Stack<Span>>();

		public static void PushSpan(Span span)
		{
			EnsureStack();

			LocalSpanStack.Value.Push(span);

		}
		public static void PopSpan(Span span)
		{
			EnsureStack();

			var existing = LocalSpanStack.Value.Pop();
		}

		public static Span CurrentSpan
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				EnsureStack();
				if (LocalSpanStack.Value.Count != 0)
				{
					return LocalSpanStack.Value.Peek();
				}
				return null;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void EnsureStack()
		{
			if (LocalSpanStack.Value == null)
				LocalSpanStack.Value = new Stack<Span>();
		}

		public static bool IsWithinTrace
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get { return CurrentSpan != null; }
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
				var span = CurrentSpan;
				dictionary[TraceIdKey] = span.TraceId;
				dictionary[SpanIdKey] = span.Id;
			}
		}

		public static bool TryObtainTraceIdFrom(IDictionary<string, string> dictionary)
		{
			return false;
		}

		public static bool TryObtainTraceIdFrom(IDictionary<string, object> dictionary, out long traceId, out long parentSpanId)
		{
			traceId = parentSpanId = 0;

			object value;
			if (dictionary.TryGetValue(TraceIdKey, out value))
			{
				traceId = Convert.ToInt64(value);

				if (dictionary.TryGetValue(SpanIdKey, out value))
				{
					parentSpanId = Convert.ToInt64(value);
				}

				return true;
			}
			return false;
		}
	}
}