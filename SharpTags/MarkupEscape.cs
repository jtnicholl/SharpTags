// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace SharpTags;

internal static class MarkupEscape
{
	internal static readonly SearchValues<byte> EscapeChars = SearchValues.Create("<>&\""u8);
	internal static readonly SearchValues<char> EscapeChars16 = SearchValues.Create("<>&\"");
	internal static readonly ReadOnlyMemory<byte> Substitutions = "&lt;&gt;&amp;&quot;"u8.ToArray();

	public static async ValueTask Escape(
		ReadOnlyMemory<byte> utf8Text,
		Stream utf8Stream,
		CancellationToken cancel
	)
	{
		var next = utf8Text.Span.IndexOfAny(EscapeChars);
		while (next != -1)
		{
			await utf8Stream.WriteAsync(utf8Text[..next], cancel).ConfigureAwait(false);
			await utf8Stream
				.WriteAsync(
					Substitutions[utf8Text.Span[next] switch
					{
						(byte)'<' => ..4,
						(byte)'>' => 4..8,
						(byte)'&' => 8..13,
						_ => 13..,
					}],
					cancel
				)
				.ConfigureAwait(false);
			utf8Text = utf8Text[(next + 1)..];
			next = utf8Text.Span.IndexOfAny(EscapeChars);
		}

		await utf8Stream.WriteAsync(utf8Text, cancel).ConfigureAwait(false);
	}

	public static void Escape(ReadOnlySpan<byte> utf8Text, Span<byte> output)
	{
		var next = utf8Text.IndexOfAny(EscapeChars);
		while (next != -1)
		{
			utf8Text[..next].CopyTo(output);
			var sub = Substitutions[utf8Text[next] switch
			{
				(byte)'<' => ..4,
				(byte)'>' => 4..8,
				(byte)'&' => 8..13,
				_ => 13..,
			}];
			sub.Span.CopyTo(output[next..]);
			utf8Text = utf8Text[(next + 1)..];
			output = output[(next + sub.Length)..];
			next = utf8Text.IndexOfAny(EscapeChars);
		}

		utf8Text.CopyTo(output);
	}

	public static void Escape(ReadOnlySpan<char> utf16Text, Span<byte> output)
	{
		var next = utf16Text.IndexOfAny(EscapeChars16);
		while (next != -1)
		{
			var written = Encoding.UTF8.GetBytes(utf16Text[..next], output);
			var sub = Substitutions[utf16Text[next] switch
			{
				'<' => ..4,
				'>' => 4..8,
				'&' => 8..13,
				_ => 13..,
			}];
			sub.Span.CopyTo(output[next..]);
			utf16Text = utf16Text[(next + 1)..];
			output = output[(written + sub.Length)..];
			next = utf16Text.IndexOfAny(EscapeChars16);
		}

		Encoding.UTF8.GetBytes(utf16Text, output);
	}

	/// <summary>
	/// Returns the same parameter if it does not need escaping
	/// </summary>
	public static ReadOnlyMemory<byte> Escape(ReadOnlyMemory<byte> unescapedUtf8)
	{
		var escapedLength = GetEscapedByteCount(unescapedUtf8.Span);
		if (escapedLength == unescapedUtf8.Length)
		{
			return unescapedUtf8;
		}

		var output = GC.AllocateUninitializedArray<byte>(escapedLength);
		Escape(unescapedUtf8.Span, output);
		return output;
	}

	public static byte[] Escape(ReadOnlySpan<byte> unescapedUtf8)
	{
		if (unescapedUtf8.IsEmpty)
		{
			return [];
		}

		var output = GC.AllocateUninitializedArray<byte>(GetEscapedByteCount(unescapedUtf8));
		Escape(unescapedUtf8, output);
		return output;
	}

	[return: NotNullIfNotNull(nameof(unescapedUtf16))]
	public static byte[]? Escape(string? unescapedUtf16)
	{
		return unescapedUtf16 is not null ? Escape(unescapedUtf16.AsSpan()) : null;
	}

	public static byte[] Escape(ReadOnlySpan<char> unescapedUtf16)
	{
		if (unescapedUtf16.IsEmpty)
		{
			return [];
		}

		var output = GC.AllocateUninitializedArray<byte>(GetEscapedByteCount(unescapedUtf16));
		Escape(unescapedUtf16, output);
		return output;
	}

	public static int GetEscapedByteCount(ReadOnlySpan<byte> utf8Text)
	{
		int output = 0;

		var next = utf8Text.IndexOfAny(EscapeChars);
		while (next != -1)
		{
			output += next;
			output += utf8Text[next] switch
			{
				(byte)'<' => 4,
				(byte)'>' => 4,
				(byte)'&' => 5,
				_ => 6,
			};
			utf8Text = utf8Text[(next + 1)..];
			next = utf8Text.IndexOfAny(EscapeChars);
		}

		return output + utf8Text.Length;
	}

	public static int GetEscapedByteCount(ReadOnlySpan<char> utf16Text)
	{
		int output = 0;

		var next = utf16Text.IndexOfAny(EscapeChars16);
		while (next != -1)
		{
			output += Encoding.UTF8.GetByteCount(utf16Text[..next]);
			output += utf16Text[next] switch
			{
				'<' => 4,
				'>' => 4,
				'&' => 5,
				_ => 6,
			};
			utf16Text = utf16Text[(next + 1)..];
			next = utf16Text.IndexOfAny(EscapeChars16);
		}

		return output + Encoding.UTF8.GetByteCount(utf16Text);
	}
}
