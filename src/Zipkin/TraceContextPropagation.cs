namespace Zipkin
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.CompilerServices;
	using System.Threading;

	/// <summary>
	/// Used to propagate and recover trace information 
	/// (trace id and parent span id) across processes.
	/// </summary>
	public static class TraceContextPropagation
	{
		private const string TraceIdKey = "_zipkin_traceid";
		private const string SpanIdKey = "_zipkin_spanid";

		private static readonly AsyncLocal<Stack<Span>> LocalSpanStack = new AsyncLocal<Stack<Span>>();

		/// <summary>
		/// For internal use only. Pushes Span onto context stack.
		/// </summary>
		/// <param name="span"></param>
		public static void PushSpan(Span span)
		{
			EnsureStack();

			LocalSpanStack.Value.Push(span);
		}

		/// <summary>
		/// For internal use only. Pops Span from context stack.
		/// </summary>
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
		
		/// <summary>
		/// Indicates whether there's an active trace in the current context.
		/// </summary>
		public static bool IsWithinTrace
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get { return CurrentSpan != null; }
		}

		/// <summary>
		/// If within a trace, writes the required information onto the specified dictionary.
		/// The dictionary should carry data from a cross process boundary (for example http headers)
		/// </summary>
		public static void PropagateTraceIdOnto(IDictionary<string, string> dictionary)
		{
			if (IsWithinTrace)
			{
				var span = CurrentSpan;
				dictionary[TraceIdKey] = span.TraceId.ToString();
				dictionary[SpanIdKey] = span.Id.ToString();
			}
		}

		/// <summary>
		/// If within a trace, writes the required information onto the specified dictionary.
		/// The dictionary should carry data from a cross process boundary (for example http headers)
		/// </summary>
		public static void PropagateTraceIdOnto(IDictionary<string, object> dictionary)
		{
			if (IsWithinTrace)
			{
				var span = CurrentSpan;
				dictionary[TraceIdKey] = span.TraceId;
				dictionary[SpanIdKey] = span.Id;
			}
		}

		/// <summary>
		/// Checks the specified dictionary for required trace information. 
		/// The dictionary should carry data from a cross process boundary (for example http headers)
		/// </summary>
		public static bool TryObtainTraceIdFrom(IDictionary<string, string> dictionary, out long traceId, out long parentSpanId)
		{
			traceId = parentSpanId = 0;

			string value;
			if (dictionary.TryGetValue(TraceIdKey, out value))
			{
				traceId = Convert.ToInt64(value);

				if (dictionary.TryGetValue(SpanIdKey, out value))
					parentSpanId = Convert.ToInt64(value);
			}

			return traceId != 0 && parentSpanId != 0;
		}

		/// <summary>
		/// Checks the specified dictionary for required trace information. 
		/// The dictionary should carry data from a cross process boundary (for example http headers)
		/// </summary>
		public static bool TryObtainTraceIdFrom(IDictionary<string, object> dictionary, out long traceId, out long parentSpanId)
		{
			traceId = parentSpanId = 0;

			object value;
			if (dictionary.TryGetValue(TraceIdKey, out value))
			{
				traceId = Convert.ToInt64(value);

				if (dictionary.TryGetValue(SpanIdKey, out value))
					parentSpanId = Convert.ToInt64(value);

			}

			return traceId != 0 && parentSpanId != 0;
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void EnsureStack()
		{
			if (LocalSpanStack.Value == null)
				LocalSpanStack.Value = new Stack<Span>();
		}
	}
}