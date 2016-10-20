namespace Zipkin
{
	using System.Collections.Generic;
	using System.Runtime.CompilerServices;


	public static class TraceExtensions
	{
		/// <summary>
		/// Adds an annotation specified with the tag. 
		/// </summary>
		public static ITrace TimeAnnotateWith(this ITrace source, PredefinedTag tag)
		{
			source.TimeAnnotateWith(ToStringTag(tag));

			return source;
		}

		/// <summary>
		/// Adds an annotation specified with the tag. 
		/// </summary>
		public static ITrace TimeAnnotateWith(this ITrace source, string tag)
		{
			if (source.Span == null) return source;

			source.Span.Annotations = source.Span.Annotations ?? new List<Annotation>();

			source.Span.Annotations.Add(new Annotation()
			{
				Value = tag,
				Host = ZipkinConfig.ThisService
			});

			return source;
		}

		/// <summary>
		/// Sets a local component name to a trace (usually a local trace)
		/// </summary>
		public static ITrace SetLocalComponentName(this ITrace source, string name)
		{
			source.AnnotateWith(new BinaryAnnotation(StandardAnnotationKeys.LocalComponent, name));

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
					return StandardAnnotationKeys.ClientSend;

				case PredefinedTag.ClientRecv:
					return StandardAnnotationKeys.ClientRecv;

				case PredefinedTag.ServerSend:
					return StandardAnnotationKeys.ServerSend;

				case PredefinedTag.ServerRecv:
					return StandardAnnotationKeys.ServerRecv;

				case PredefinedTag.WireSend:
					return StandardAnnotationKeys.WireSend;

				case PredefinedTag.WireRecv:
					return StandardAnnotationKeys.WireRecv;

				case PredefinedTag.ClientSendFragment:
					return StandardAnnotationKeys.ClientSendFragment;

				case PredefinedTag.ClientRecvFragment:
					return StandardAnnotationKeys.ClientRecvFragment;

				case PredefinedTag.ServerSendFragment:
					return StandardAnnotationKeys.ServerSendFragment;

				case PredefinedTag.ServerRecvFragment:
					return StandardAnnotationKeys.ServerRecvFragment;

				case PredefinedTag.ClientAddr:
					return StandardAnnotationKeys.ClientAddr;

				case PredefinedTag.ServerAddr:
					return StandardAnnotationKeys.ServerAddr;

				case PredefinedTag.LocalComponent:
					return StandardAnnotationKeys.ServerAddr;

				case PredefinedTag.Error:
					return StandardAnnotationKeys.Error;


				case PredefinedTag.HttpHost:
					return CustomAnnotationKeys.HttpHost;

				case PredefinedTag.HttpMethod:
					return CustomAnnotationKeys.HttpMethod;

				case PredefinedTag.HttpPath:
					return CustomAnnotationKeys.HttpPath;

				case PredefinedTag.SqlQuery:
					return CustomAnnotationKeys.SqlQuery;

				default:
					return string.Empty;
			}
		}
	}
}