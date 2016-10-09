namespace Zipkin.Tests
{
	using System.IO;
	using System.Text;
	using Codecs;
	using Codecs.Json;
	using NUnit.Framework;

	[TestFixture]
	public class StreamWriterExtensionsTest
	{
		[Test, Explicit("Slow")]
		public void WriteLowerHex()
		{
			var sb = new StringBuilder();
			for (long i = 0; i < (int.MaxValue + 1L); i++)
			{
				sb.Length = 0;
				using (var writer = new StringWriter(sb))
				{
					writer.WriteLowerHex(i);
					writer.Flush();
				}

				Assert.AreEqual(string.Format("{0:x16}", i), sb.ToString());
			}
		}
	}
}