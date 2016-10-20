namespace Zipkin
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.CompilerServices;
	using System.Threading;
	using Utils;

	/// <summary>
	/// Used to propagate and recover trace information 
	/// (trace id and parent span id) across processes.
	/// </summary>
	public static class TraceContextPropagation
	{
		// Todo: make it compatible with
//		X-B3-TraceId : 64 lower-hex encoded bits(required)
//		X-B3-SpanId : 64 lower-hex encoded bits(required)
//		X-B3-ParentSpanId : 64 lower-hex encoded bits(absent on root span)
//		X-B3-Sampled : Boolean(either “1” or “0”, can be absent)
//		X-B3-Flags : “1” means debug(can be absent)

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
		public static void PopSpan()
		{
			EnsureStack();

			LocalSpanStack.Value.Pop();
		}

		public static Span CurrentSpan
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				if (LocalSpanStack.Value != null && LocalSpanStack.Value.Count != 0)
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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryObtainTraceIdFrom(IDictionary<string, string> dictionary, out TraceInfo? traceInfo)
		{
			traceInfo = null;

			string value;
			if (dictionary != null && dictionary.TryGetValue(TraceIdKey, out value))
			{
				var traceId = Convert.ToInt64(value);
				var parentSpanId = 0L;

				if (dictionary.TryGetValue(SpanIdKey, out value))
					parentSpanId = Convert.ToInt64(value);

				traceInfo = new TraceInfo
				{
					span = new Span(traceId, null, 0L) { ParentId = parentSpanId }
				};
			}

			return traceInfo.HasValue;
		}

		/// <summary>
		/// Checks the specified dictionary for required trace information. 
		/// The dictionary should carry data from a cross process boundary (for example http headers)
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryObtainTraceIdFrom(IDictionary<string, object> dictionary, out TraceInfo? traceInfo)
		{
			traceInfo = null;

			object value;
			if (dictionary != null && dictionary.TryGetValue(TraceIdKey, out value))
			{
				var traceId = Convert.ToInt64(value);
				long parentSpanId = 0;

				if (dictionary.TryGetValue(SpanIdKey, out value))
					parentSpanId = Convert.ToInt64(value);

				traceInfo = new TraceInfo
				{
					span = new Span(traceId, null, 0L) { ParentId = parentSpanId }
				};
			}

			return traceInfo.HasValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TraceInfo? GetTraceInfoFrom(IDictionary<string, string> dictionary)
		{
			TraceInfo? traceInfo;
			TryObtainTraceIdFrom(dictionary, out traceInfo);
			return traceInfo;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TraceInfo? GetTraceInfoFrom(IDictionary<string, object> dictionary)
		{
			TraceInfo? traceInfo;
			TryObtainTraceIdFrom(dictionary, out traceInfo);
			return traceInfo;
		}

		public static TraceInfo CaptureCurrentTrace()
		{
			var span = CurrentSpan;

			if (span != null)
			{
				return new TraceInfo
				{
					span = span.Clone()
				};
			}

			return new TraceInfo();
		}

		public static void ApplyCaptured(TraceInfo capturedContext)
		{
			if (capturedContext.span != null)
			{
				capturedContext.span.DurationInMicroseconds = TickClock.Start();

				PushSpan(capturedContext.span);
			}
		}

		public static void UndoApply(TraceInfo capturedContext)
		{
			if (capturedContext.span != null)
			{
				PopSpan();

				capturedContext.span.DurationInMicroseconds = TickClock.GetDuration(capturedContext.span.DurationInMicroseconds.Value);

				ZipkinConfig.Record(capturedContext.span);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void EnsureStack()
		{
			if (LocalSpanStack.Value == null)
				LocalSpanStack.Value = new Stack<Span>();
		}

		/// <summary>
		/// For unit tests
		/// </summary>
		internal static void Reset()
		{
			LocalSpanStack.Value = null;
		}
	}

	public struct TraceInfo
	{
		internal Span span;
	}
}