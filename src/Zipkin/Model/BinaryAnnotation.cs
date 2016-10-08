namespace Zipkin
{
	using System;
	using System.Text;

	/// <summary>
	/// Binary annotations are tags applied to a Span to give it context. For
	/// example, a binary annotation of "http.url" could be the path to a resource in a
	/// RPC call.
	/// 
	/// <para>
	/// Binary annotations of type STRING are always queryable, though more a
	/// historical implementation detail than a structural concern.
	/// </para>
	/// </summary>
	/// <remarks>
	/// Binary annotations can repeat, and vary on the host. Similar to Annotation,
	/// the host indicates who logged the event. This allows you to tell the
	/// difference between the client and server side of the same key. For example,
	/// the key "http.url" might be different on the client and server side due to
	/// rewriting, like "/api/v1/myresource" vs "/myresource. Via the host field,
	/// you can see the different points of view, which often help in debugging.
	/// </remarks>
	public class BinaryAnnotation
	{
		public BinaryAnnotation(string key, string value) : this(key, AnnotationType.STRING)
		{
			this.ValAsString = value;
		}
		public BinaryAnnotation(string key, byte[] value) : this(key, AnnotationType.BYTES)
		{
			this.ValAsBArray = value;
		}
		public BinaryAnnotation(string key, bool value) : this(key, AnnotationType.BOOL)
		{
			this.ValAsBool = value;
		}
		public BinaryAnnotation(string key, Int16 value) : this(key, AnnotationType.I16)
		{
			this.ValAsI16 = value;
		}
		public BinaryAnnotation(string key, Int32 value) : this(key, AnnotationType.I32)
		{
			this.ValAsI32 = value;
		}
		public BinaryAnnotation(string key, Int64 value) : this(key, AnnotationType.I64)
		{
			this.ValAsI64 = value;
		}
		public BinaryAnnotation(string key, double value) : this(key, AnnotationType.DOUBLE)
		{
			this.ValAsDouble = value;
		}

		protected BinaryAnnotation(string key, AnnotationType type)
		{
			if (key == null) throw new ArgumentNullException(nameof(key));
			this.Key = key;
			this.AnnotationType = type;
		}

		protected internal BinaryAnnotation()
		{
		}

		/// <summary>
		/// The host that recorded tag, which allows you to differentiate between
		/// multiple tags with the same key. There are two exceptions to this.
		/// 
		/// When the key is CLIENT_ADDR or SERVER_ADDR, host indicates the source or
		/// destination of an RPC. This exception allows zipkin to display network
		/// context of uninstrumented services, or clients such as web browsers.
		/// </summary>
		public Endpoint Host;

		/// <summary>
		/// Name used to lookup spans, such as TraceKeys#HTTP_PATH "http.path" or "error"
		/// </summary>
		public string Key;

		// <summary>
		// Serialized thrift bytes, in TBinaryProtocol format. 
		// For legacy reasons, byte order is big-endian. See THRIFT-3217.
		// </summary>
//		public byte[] Value;

		/// <summary>
		/// The thrift type of value, most often STRING.
		/// </summary>
		public AnnotationType AnnotationType;

		public string ValAsString;
		public byte[] ValAsBArray;
		public short ValAsI16;
		public int ValAsI32;
		public long ValAsI64;
		public double ValAsDouble;
		public bool ValAsBool;

		
	}
}