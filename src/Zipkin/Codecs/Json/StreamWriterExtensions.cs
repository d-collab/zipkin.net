namespace Zipkin.Codecs.Json
{
	using System;
	using System.IO;
	using System.Runtime.CompilerServices;
	using System.Text;

	// 
	// See: https://github.com/openzipkin/zipkin/blob/master/zipkin/src/main/java/zipkin/internal/Buffer.java
	//
	public static class StreamWriterExtensions
	{
		private static readonly string[] ReplacementChars;

		private const string U2028 = "\\u2028";
		private const string U2029 = "\\u2029";

		static StreamWriterExtensions()
		{
			ReplacementChars = new string[128];
			for (int i = 0; i <= 0x1f; i++)
			{
				ReplacementChars[i] = string.Format("\\u%04x", (int)i);
			}
			ReplacementChars['"'] = "\\\"";
			ReplacementChars['\\'] = "\\\\";
			ReplacementChars['\t'] = "\\t";
			ReplacementChars['\b'] = "\\b";
			ReplacementChars['\n'] = "\\n";
			ReplacementChars['\r'] = "\\r";
			ReplacementChars['\f'] = "\\f";
		}

		private static readonly char[] HexDigits = new[]
		{
			'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f'
		};

		public static bool NeedsJsonEscaping(byte[] v)
		{
			for (int i = 0; i < v.Length; i++)
			{
				int current = v[i] & 0xFF;

				if (i >= 2 &&
					// Is this the end of a u2028 or u2028 UTF-8 codepoint?
					// 0xE2 0x80 0xA8 == u2028; 0xE2 0x80 0xA9 == u2028
					(current == 0xA8 || current == 0xA9)
					&& (v[i - 1] & 0xFF) == 0x80
					&& (v[i - 2] & 0xFF) == 0xE2)
				{
					return true;
				}
				else if (current < 0x80 && ReplacementChars[current] != null)
				{
					return true;
				}
			}
			return false; // must be a string we don't need to escape.
		}

		public static void WriteLowerHex(this TextWriter source, long val)
		{
			WriteHexByte((byte)((val >> 56) & 0xff), source);
			WriteHexByte((byte)((val >> 48) & 0xff), source);
			WriteHexByte((byte)((val >> 40) & 0xff), source);
			WriteHexByte((byte)((val >> 32) & 0xff), source);
			WriteHexByte((byte)((val >> 24) & 0xff), source);
			WriteHexByte((byte)((val >> 16) & 0xff), source);
			WriteHexByte((byte)((val >> 8) & 0xff), source);
			WriteHexByte((byte)(val & 0xff), source);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void WriteHexByte(byte v, TextWriter writer)
		{
			writer.Write(HexDigits[(v >> 4) & 0xf]);
			writer.Write(HexDigits[v & 0xf]);
		}

		public static void WriteJsonEscaped(this StreamWriter source, byte[] val)
		{
			if (NeedsJsonEscaping(val))
				WriteJsonEscaped(source, Encoding.UTF8.GetString(val));
			else
				// source.Write(val, 0, val.Length);
				WriteAsciiOrUtf8(source, Encoding.UTF8.GetString(val));
		}

		public static void WriteJsonEscaped(this StreamWriter source, string val)
		{
			int afterReplacement = 0;
			int length = val.Length;
			StringBuilder builder = null;
			for (int i = 0; i < length; i++)
			{
				char c = val[i];
				string replacement;
				if (c < 0x80)
				{
					replacement = ReplacementChars[c];
					if (replacement == null) continue;
				}
				else if (c == '\u2028')
				{
					replacement = U2028;
				}
				else if (c == '\u2029')
				{
					replacement = U2029;
				}
				else
				{
					continue;
				}

				if (afterReplacement < i)
				{ // write characters between the last replacement and now
					if (builder == null) builder = new StringBuilder();
					builder.Append(val, afterReplacement, i);
				}
				if (builder == null)
					builder = new StringBuilder();
				builder.Append(replacement);
				afterReplacement = i + 1;
			}

			if (builder == null)
			{
				// then we didn't escape anything
				WriteAsciiOrUtf8(source, val);
				return;
			}

			if (afterReplacement < length)
			{
				builder.Append(val, afterReplacement, length);
			}

			WriteAsciiOrUtf8(source, builder.ToString());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void WriteAsciiOrUtf8(this StreamWriter source, string val)
		{
			if (IsAscii(val))
				InternalWriteAscii(source, val);
			else
				InternalWriteUtf8(source, val);

		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void InternalWriteAscii(StreamWriter source, string s)
		{
			source.Write(s);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void InternalWriteUtf8(StreamWriter source, string s)
		{
			byte[] temp = Encoding.UTF8.GetBytes(s);
			source.Write(temp);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteBase64Url(this StreamWriter source, byte[] val)
		{
			source.Write( Convert.ToBase64String(val) );
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool IsAscii(string val)
		{
			foreach (var c in val)
			{
				if (c >= 0x80) return false;
			}
			return true;
		}
	}
}