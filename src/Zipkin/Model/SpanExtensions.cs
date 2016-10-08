namespace Zipkin
{
	using System.Collections.Generic;
	using System.Runtime.CompilerServices;
	using Model;

	public static class SpanExtensions
	{
		public static Span AnnotateWithTag(this Span source, PredefinedTag tag)
		{
			source.Annotations = source.Annotations ?? new List<Annotation>();

			source.Annotations.Add(new Annotation()
			{
				Value = ToStringTag(tag),
				Host = ZipkinConfig.ThisService
			});

			return source;
		}

		public static Span AnnotateWith(this Span source, PredefinedTag tag, string value)
		{
			source.AnnotateWith(new BinaryAnnotation(ToStringTag(tag), value));

			return source;
		}

		public static Span AnnotateWith(this Span source, string key, string value)
		{
			source.AnnotateWith(new BinaryAnnotation(key, value));

			return source;
		}

		public static Span AnnotateWith(this Span source, PredefinedTag tag, int value)
		{
			source.AnnotateWith(new BinaryAnnotation(ToStringTag(tag), value));

			return source;
		}

		public static Span AnnotateWith(this Span source, string key, int value)
		{
			source.AnnotateWith(new BinaryAnnotation(key, value));

			return source;
		}

		public static Span AnnotateWith(this Span source, PredefinedTag tag, bool value)
		{
			source.AnnotateWith(new BinaryAnnotation(ToStringTag(tag), value));

			return source;
		}

		public static Span AnnotateWith(this Span source, string key, bool value)
		{
			source.AnnotateWith(new BinaryAnnotation(key, value));

			return source;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AnnotateWith(this Span source, BinaryAnnotation binaryAnnotation)
		{
			source.BinaryAnnotations = source.BinaryAnnotations ?? new List<BinaryAnnotation>();

			if (binaryAnnotation.Host == null)
				binaryAnnotation.Host = ZipkinConfig.ThisService;

			source.BinaryAnnotations.Add(binaryAnnotation);
		}

		// [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

	public enum PredefinedTag
	{
		HttpHost,
		HttpMethod,
		HttpPath,
		SqlQuery,


		ClientSend,
		ClientRecv,
		ServerSend,
		ServerRecv,
		WireSend,
		WireRecv,
		ClientSendFragment,
		ClientRecvFragment,
		ServerSendFragment,
		ServerRecvFragment, 
		ClientAddr,
		ServerAddr,
		LocalComponent
	}
}