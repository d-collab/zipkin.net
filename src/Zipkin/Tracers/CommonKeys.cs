namespace Zipkin.Model
{
	/// <summary>
	/// Well-known {BinaryAnnotation#key} binary annotation keys.
	/// </summary>
	public static class CommonKeys
	{
		/// <summary>
		/// The domain portion of the URL or host header. Ex. "mybucket.s3.amazonaws.com"
		/// </summary>
		public const string HttpHost = "http.host";

		/// <summary>
		/// The HTTP method, or verb, such as "GET" or "POST".
		/// </summary>
		public const string HttpMethod = "http.method";

		/// <summary>
		/// The absolute http path, without any query parameters. Ex. "/objects/abcd-ff"
		/// </summary>
		public const string HttpPath = "http.path";

		/// <summary>
		/// The entire URL, including the scheme, host and query parameters if available.
		/// </summary>
		public const string HttpUrl = "http.url";

		/// <summary>
		/// The HTTP status code, when not in 2xx range. Ex. "503"
		/// </summary>
		public const string HttpStatusCode = "http.status_code";

		/// <summary>
		/// The size of the non-empty HTTP request body, in bytes. 
		/// </summary>
		public const string HttpRequestSize = "http.request.size";

		/// <summary>
		/// The size of the non-empty HTTP response body, in bytes.
		/// </summary>
		public const string HttpResponseSize = "http.response.size";

		/// <summary>
		/// The query executed for SQL call.  Ex. "select * from customers where id = ?"
		/// </summary>
		public const string SqlQuery = "sql.query";
	}
}