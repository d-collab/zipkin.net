namespace Zipkin
{
	using System.Collections.Generic;
	using System.Runtime.CompilerServices;
	using Model;

	public static class TraceExtensions
	{
		/// <summary>
		/// A trace marked as Debug will always be traced, in other words, it
		/// skips the sampling rate configuration. 
		/// </summary>
		public static ITrace MarkAsDebug(this ITrace source, bool isDebug)
		{
			if (source.Span == null) return source;

			source.Span.IsDebug = isDebug;

			return source;
		}

		/// <summary>
		/// Adds an annotation specified with the tag. 
		/// </summary>
		public static ITrace AnnotateWithTag(this ITrace source, PredefinedTag tag)
		{
			if (source.Span == null) return source;

			source.Span.Annotations = source.Span.Annotations ?? new List<Annotation>();

			source.Span.Annotations.Add(new Annotation()
			{
				Value = ToStringTag(tag),
				Host = ZipkinConfig.ThisService
			});

			return source;
		}

		/// <summary>
		/// Sets a local component name to a trace (usually a local trace)
		/// </summary>
		public static ITrace SetLocalComponentName(this ITrace source, string name)
		{
			source.AnnotateWith(new BinaryAnnotation(AnnotationConstants.LOCAL_COMPONENT, name));

			return source;
		}

		public static ITrace AnnotateWith(this ITrace source, PredefinedTag tag, string value)
		{
			source.AnnotateWith(new BinaryAnnotation(ToStringTag(tag), value));

			return source;
		}

		public static ITrace AnnotateWith(this ITrace source, string key, string value)
		{
			source.AnnotateWith(new BinaryAnnotation(key, value));

			return source;
		}

		public static ITrace AnnotateWith(this ITrace source, PredefinedTag tag, int value)
		{
			source.AnnotateWith(new BinaryAnnotation(ToStringTag(tag), value));

			return source;
		}

		public static ITrace AnnotateWith(this ITrace source, string key, int value)
		{
			source.AnnotateWith(new BinaryAnnotation(key, value));

			return source;
		}

		public static ITrace AnnotateWith(this ITrace source, PredefinedTag tag, bool value)
		{
			source.AnnotateWith(new BinaryAnnotation(ToStringTag(tag), value));

			return source;
		}

		public static ITrace AnnotateWith(this ITrace source, string key, bool value)
		{
			source.AnnotateWith(new BinaryAnnotation(key, value));

			return source;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AnnotateWith(this ITrace source, BinaryAnnotation binaryAnnotation)
		{
			if (source.Span == null) return;

			source.Span.BinaryAnnotations = source.Span.BinaryAnnotations ?? new List<BinaryAnnotation>();

			if (binaryAnnotation.Host == null)
				binaryAnnotation.Host = ZipkinConfig.ThisService;

			source.Span.BinaryAnnotations.Add(binaryAnnotation);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static string ToStringTag(PredefinedTag tag)
		{
			switch (tag)
			{
				case PredefinedTag.ClientSend:
					return AnnotationConstants.CLIENT_SEND;

				case PredefinedTag.ClientRecv:
					return AnnotationConstants.CLIENT_RECV;

				case PredefinedTag.ServerSend:
					return AnnotationConstants.SERVER_SEND;

				case PredefinedTag.ServerRecv:
					return AnnotationConstants.SERVER_RECV;

				case PredefinedTag.WireSend:
					return AnnotationConstants.WIRE_SEND;

				case PredefinedTag.WireRecv:
					return AnnotationConstants.WIRE_RECV;

				case PredefinedTag.ClientSendFragment:
					return AnnotationConstants.CLIENT_SEND_FRAGMENT;

				case PredefinedTag.ClientRecvFragment:
					return AnnotationConstants.CLIENT_RECV_FRAGMENT;

				case PredefinedTag.ServerSendFragment:
					return AnnotationConstants.SERVER_SEND_FRAGMENT;

				case PredefinedTag.ServerRecvFragment:
					return AnnotationConstants.SERVER_RECV_FRAGMENT;

				case PredefinedTag.ClientAddr:
					return AnnotationConstants.CLIENT_ADDR;

				case PredefinedTag.ServerAddr:
					return AnnotationConstants.SERVER_ADDR;

				case PredefinedTag.LocalComponent:
					return AnnotationConstants.SERVER_ADDR;

				case PredefinedTag.Error:
					return AnnotationConstants.ERROR;


				case PredefinedTag.HttpHost:
					return CommonKeys.HttpHost;

				case PredefinedTag.HttpMethod:
					return CommonKeys.HttpMethod;

				case PredefinedTag.HttpPath:
					return CommonKeys.HttpPath;

				case PredefinedTag.SqlQuery:
					return CommonKeys.SqlQuery;

				default:
					return string.Empty;
			}
		}
	}
}