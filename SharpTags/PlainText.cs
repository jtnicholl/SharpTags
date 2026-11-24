// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Buffers;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Text;

namespace SharpTags;

/// <summary>
/// Wraps text which has already been escaped.
/// </summary>
public readonly struct PlainText : IEquatable<PlainText>
{
	/// <summary>
	/// An empty <see cref="PlainText" />.
	/// </summary>
	public static readonly PlainText Empty = default;

	/// <summary>
	/// If this text is empty.
	/// </summary>
	public bool IsEmpty => this.Utf8Bytes.IsEmpty;

	/// <summary>
	/// The length of this text in UTF-8 bytes.
	/// </summary>
	public int ByteLength => this.Utf8Bytes.Length;

	/// <summary>
	/// This text as UTF-8 bytes.
	/// </summary>
	public ReadOnlyMemory<byte> Utf8Bytes { get; }

	/// <summary>
	/// Construct an empty <see cref="PlainText" />.
	/// </summary>
	public PlainText() { }

	private PlainText(ReadOnlyMemory<byte> text)
	{
		this.Utf8Bytes = text;
	}

	/// <summary>
	/// Construct a <see cref="PlainText" /> by converting and escaping the provided UTF-16 text.
	/// </summary>
	/// <param name="unescapedUtf16">The text to escape.</param>
	public PlainText(ReadOnlySpan<char> unescapedUtf16)
	{
		this.Utf8Bytes = MarkupEscape.Escape(unescapedUtf16);
	}

	/// <summary>
	/// Construct a <see cref="PlainText" /> by escaping the provided UTF-8 text.
	/// </summary>
	/// <param name="unescapedUtf8">The text to escape.</param>
	/// <remarks></remarks>
	public PlainText(ReadOnlySpan<byte> unescapedUtf8)
	{
		this.Utf8Bytes = MarkupEscape.Escape(unescapedUtf8);
	}

	/// <summary>
	/// Convert a date without time or time zone information into an HTML date
	/// string <c>"yyyy-MM-dd"</c> (ISO 8601).
	/// </summary>
	/// <param name="plainDate">The date to convert.</param>
	/// <seealso href="https://developer.mozilla.org/en-US/docs/Web/HTML/Guides/Date_and_time_formats#date_strings" />
	public PlainText(DateOnly plainDate)
	{
		Span<byte> buffer = stackalloc byte[10];
		var success = plainDate.TryFormat(buffer, out var written, "O");
		Contract.Assert(success);
		Contract.Assert(written == 10);
		Utf8Bytes = buffer.ToArray();
	}

	/// <summary>
	/// Convert a date and time without time zone information into a normalized
	/// HTML local datetime string.
	/// </summary>
	/// <remarks>
	/// A normalized HTML local datetime starts with <c>yyyy-MM-ddThh:mm</c>.
	/// Seconds are added only if non-zero, with no trailing zeros in the
	/// decimal part.
	/// </remarks>
	/// <param name="plainDateTime">
	/// The date and time to convert. Time zone information is ignored.
	/// </param>
	/// <seealso href="https://developer.mozilla.org/en-US/docs/Web/HTML/Guides/Date_and_time_formats#local_date_and_time_strings" />
	public PlainText(DateTime plainDateTime)
	{
		Span<byte> buffer = stackalloc byte[33];
		var success = plainDateTime.TryFormat(buffer, out var written, "O");
		Contract.Assert(success);
		Contract.Assert(written >= 27);
		if (plainDateTime.Ticks % TimeSpan.TicksPerMinute == 0)
		{
			Utf8Bytes = buffer[..16].ToArray();
		}
		else if (plainDateTime.Ticks % TimeSpan.TicksPerSecond == 0)
		{
			Utf8Bytes = buffer[..19].ToArray();
		}
		else
		{
			Utf8Bytes = buffer[..27].TrimEnd((byte)'0').ToArray();
		}
	}

	/// <summary>
	/// Construct a <see cref="PlainText" /> from any arbitrary
	/// <see cref="IUtf8SpanFormattable" />, using
	/// <see cref="CultureInfo.InvariantCulture" /> and escaping the result.
	/// </summary>
	/// <param name="formattable">The object to format.</param>
	/// <param name="format">An optional span representing a format string.</param>
	/// <seealso cref="IUtf8SpanFormattable.TryFormat" />
	public PlainText(IUtf8SpanFormattable formattable, ReadOnlySpan<char> format = default)
		: this(formattable, format, CultureInfo.InvariantCulture)
	{ }

	/// <summary>
	/// Construct a <see cref="PlainText" /> from any arbitrary
	/// <see cref="IUtf8SpanFormattable" />, escaping the result.
	/// </summary>
	/// <param name="formattable">The object to format.</param>
	/// <param name="format">An optional span representing a format string.</param>
	/// <param name="provider">
	/// An optional provider for culture-specific formatting information.
	/// </param>
	/// <seealso cref="IUtf8SpanFormattable.TryFormat" />
	public PlainText(
		IUtf8SpanFormattable formattable,
		ReadOnlySpan<char> format,
		IFormatProvider? provider
	)
	{
		ArgumentNullException.ThrowIfNull(formattable);

		var buffer = ArrayPool<byte>.Shared.Rent(20);
		try
		{
			int length;

			while (!formattable.TryFormat(buffer, out length, format, provider))
			{
				var newLength = buffer.Length * 2;
				ArrayPool<byte>.Shared.Return(buffer);
				buffer = ArrayPool<byte>.Shared.Rent(newLength);
			}

			this.Utf8Bytes = MarkupEscape.Escape(buffer.AsSpan(0, length));
		}
		finally
		{
			ArrayPool<byte>.Shared.Return(buffer);
		}
	}

	/// <summary>
	/// Convert the given <paramref name="rawUtf16" /> to
	/// <see cref="PlainText" /> directly, without escaping any unsafe
	/// characters.
	/// </summary>
	/// <param name="rawUtf16">The text to convert.</param>
	/// <returns>
	/// A <see cref="PlainText" />, converted directly from the provided
	/// <paramref name="rawUtf16" /> without escaping.
	/// </returns>
	/// <remarks>
	/// This can be used for advanced scenarios where the text must be left
	/// unescaped.<br />
	/// Note that this overload does not provide the same performance benefit as
	/// the others, because the text still needs to be converted to UTF-8.
	/// </remarks>
	[Pure]
	public static PlainText UnsafeRaw(ReadOnlySpan<char> rawUtf16)
	{
		// Encoding does not have a direct ReadOnlySpan<char> -> byte[] conversion.
		// Have to do some extra work.
		var bytes = GC.AllocateUninitializedArray<byte>(Encoding.UTF8.GetByteCount(rawUtf16));
		Encoding.UTF8.GetBytes(rawUtf16, bytes);
		return new(bytes.AsMemory());
	}

	/// <summary>
	/// Convert the given <paramref name="rawUtf8" /> to
	/// <see cref="PlainText" /> directly, without escaping any unsafe
	/// characters.
	/// </summary>
	/// <param name="rawUtf8">The text to convert, as UTF-8 bytes.</param>
	/// <returns>
	/// A <see cref="PlainText" />, converted directly from the provided
	/// <paramref name="rawUtf8" /> without escaping.
	/// </returns>
	/// <remarks>
	/// This can be used for advanced scenarios where the text must be left
	/// unescaped, or to improve performance in situations where XSS is
	/// guaranteed not to be a risk.<br />
	/// The data is copied to guarantee immutability. If you need multiple
	/// <see cref="PlainText" /> instances from the same underlying bytes,
	/// consider using the <see cref="ImmutableArray{T}" /> overloads.
	/// </remarks>
	[Pure]
	public static PlainText UnsafeRaw(ReadOnlySpan<byte> rawUtf8)
	{
		return new(rawUtf8.ToArrayFast().AsMemory());
	}

	/// <summary>
	/// Convert the given <paramref name="rawUtf8" /> to
	/// <see cref="PlainText" /> directly, without escaping any unsafe
	/// characters.
	/// </summary>
	/// <param name="rawUtf8">The text to convert, as UTF-8 bytes.</param>
	/// <returns>
	/// A <see cref="PlainText" />, converted directly from the provided
	/// <paramref name="rawUtf8" /> without escaping.
	/// </returns>
	/// <remarks>
	/// This can be used for advanced scenarios where the text must be left
	/// unescaped, or to improve performance in situations where XSS is
	/// guaranteed not to be a risk.<br />
	/// Unlike <see cref="UnsafeRaw(ReadOnlySpan{byte})" />, this overload
	/// avoids creating additional copies of the underlying data.
	/// </remarks>
	[Pure]
	public static PlainText UnsafeRaw(ImmutableArray<byte> rawUtf8)
	{
		return new(rawUtf8.AsMemory());
	}

	/// <summary>
	/// Convert a slice of the given <paramref name="rawUtf8" /> to
	/// <see cref="PlainText" /> directly, without escaping any unsafe
	/// characters.
	/// </summary>
	/// <param name="rawUtf8">
	/// The <see cref="ImmutableArray{T}" /> containing the text to convert, as UTF-8 bytes.
	/// </param>
	/// <param name="start">
	/// The byte index in <paramref name="rawUtf8" /> at which to begin the slice.
	/// </param>
	/// <returns>
	/// A <see cref="PlainText" />, converted directly from the provided
	/// <paramref name="rawUtf8" /> without escaping.
	/// </returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// If <paramref name="start" /> is less than zero or greater than the
	/// length of <paramref name="rawUtf8" />.
	/// </exception>
	/// <remarks>
	/// This can be used for advanced scenarios where the text must be left
	/// unescaped, or to improve performance in situations where XSS is
	/// guaranteed not to be a risk.<br />
	/// Unlike <see cref="UnsafeRaw(ReadOnlySpan{byte})" />, this overload
	/// avoids creating additional copies of the underlying data.
	/// </remarks>
	[Pure]
	public static PlainText UnsafeRaw(ImmutableArray<byte> rawUtf8, int start)
	{
		return new(rawUtf8.AsMemory().Slice(start));
	}

	/// <summary>
	/// Convert a slice of the given <paramref name="rawUtf8" /> to
	/// <see cref="PlainText" /> directly, without escaping any unsafe
	/// characters.
	/// </summary>
	/// <param name="rawUtf8">
	/// The <see cref="ImmutableArray{T}" /> containing the text to convert, as UTF-8 bytes.
	/// </param>
	/// <param name="start">
	/// The byte index in <paramref name="rawUtf8" /> at which to begin the slice.
	/// </param>
	/// <param name="length">
	/// The length of the slice to take from <paramref name="rawUtf8" />.
	/// </param>
	/// <returns>
	/// A <see cref="PlainText" />, converted directly from the provided
	/// <paramref name="rawUtf8" /> without escaping.
	/// </returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// If either <paramref name="start" /> or <paramref name="length" /> are
	/// less than zero, or if <paramref name="start" /> +
	/// <paramref name="length" /> is greater than the length of
	/// <paramref name="rawUtf8" />.
	/// </exception>
	/// <remarks>
	/// This can be used for advanced scenarios where the text must be left
	/// unescaped, or to improve performance in situations where XSS is
	/// guaranteed not to be a risk.<br />
	/// Unlike <see cref="UnsafeRaw(ReadOnlySpan{byte})" />, this overload
	/// avoids creating additional copies of the underlying data.
	/// </remarks>
	[Pure]
	public static PlainText UnsafeRaw(ImmutableArray<byte> rawUtf8, int start, int length)
	{
		return new(rawUtf8.AsMemory().Slice(start, length));
	}

	/// <summary>
	/// Construct a <see cref="PlainText" /> from the given
	/// <paramref name="rawUtf8" /> <b>without making a copy or escaping</b>.
	/// </summary>
	/// <param name="rawUtf8">
	/// The text to use.
	/// </param>
	/// <remarks>
	/// <see cref="PlainText" /> is meant to be entirely immutable. Care must be
	/// taken to ensure the provided <paramref name="rawUtf8" /> is not only
	/// escaped appropriately, but also that its contents will not change over
	/// the lifetime of the returned <see cref="PlainText" />.
	/// </remarks>
	[Pure]
	internal static PlainText UnsafeReference(ReadOnlyMemory<byte> rawUtf8)
	{
		return new(rawUtf8);
	}

	[Pure]
	internal static PlainText Join(PlainText separator, params ReadOnlySpan<PlainText> text)
	{
		if (text.IsEmpty)
		{
			return default;
		}
		else if (text.Length == 1)
		{
			return text[0];
		}

		int outputLength = 0;
		foreach (var item in text)
		{
			outputLength += item.ByteLength;
		}
		outputLength += separator.ByteLength * (text.Length - 1);

		var output = GC.AllocateUninitializedArray<byte>(outputLength);

		var index = 0;
		for (int i = 0; i < text.Length - 1; i++)
		{
			text[i].Utf8Bytes.CopyTo(output.AsMemory(index));
			index += text[i].ByteLength;
			separator.Utf8Bytes.Span.CopyTo(output.AsMemory(index).Span);
			index += separator.ByteLength;
		}
		text[^1].Utf8Bytes.CopyTo(output.AsMemory(index));

		return new(output.AsMemory());
	}

	[Pure]
	internal static PlainText Join(PlainText separator, IEnumerable<PlainText> text)
	{
		using var enumerator = text.GetEnumerator();
		if (!enumerator.MoveNext())
		{
			return Empty;
		}

		var firstValue = enumerator.Current;
		if (!enumerator.MoveNext())
		{
			return firstValue;
		}

		using var buffer = new PooledList<byte>([
			..firstValue.Utf8Bytes.Span,
			..separator.Utf8Bytes.Span,
			..enumerator.Current.Utf8Bytes.Span,
		]);
		while (enumerator.MoveNext())
		{
			buffer.AddRange([..separator.Utf8Bytes.Span, ..enumerator.Current.Utf8Bytes.Span]);
		}

		return new(buffer.ToArray().AsMemory());
	}

	/// <summary>
	/// Check if this <see cref="PlainText" /> is exactly equal to the
	/// <paramref name="other" />, comparing by value.
	/// </summary>
	/// <param name="other">The other <see cref="PlainText" /> to compare to.</param>
	/// <returns>
	/// <see langword="true" /> it they are equal, <see langword="false" /> otherwise.
	/// </returns>
	public bool Equals(PlainText other)
	{
		return Utf8Bytes.Span.SequenceEqual(other.Utf8Bytes.Span);
	}

	/// <inheritdoc />
	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		return obj is PlainText other && Equals(other);
	}

	/// <inheritdoc />
	public override int GetHashCode()
	{
		HashCode output = new();

		output.AddBytes(Utf8Bytes.Span);

		return output.ToHashCode();
	}

	/// <summary>
	/// Convert the text back to a <see cref="string" />. Any text escaping will not be undone.
	/// </summary>
	/// <returns>The text as a <see cref="string" />.</returns>
	public override string ToString()
	{
		return Encoding.UTF8.GetString(Utf8Bytes.Span);
	}

	/// <summary>
	/// Check if the <paramref name="left" /> <see cref="PlainText" /> is
	/// exactly equal to the <paramref name="right" />, comparing by value.
	/// </summary>
	/// <returns>
	/// <see langword="true" /> it they are equal, <see langword="false" /> otherwise.
	/// </returns>
	public static bool operator ==(PlainText left, PlainText right)
	{
		return left.Equals(right);
	}

	/// <summary>
	/// Check if the <paramref name="left" /> <see cref="PlainText" /> is not
	/// exactly equal to the <paramref name="right" />, comparing by value.
	/// </summary>
	/// <returns>
	/// <see langword="false" /> it they are equal, <see langword="true" /> otherwise.
	/// </returns>
	public static bool operator !=(PlainText left, PlainText right)
	{
		return !left.Equals(right);
	}

	/// <summary>
	/// Implicit conversion from UTF-16 to <see cref="PlainText" />, escaping unsafe XML characters.
	/// </summary>
	/// <param name="unescapedUtf16">The text to escape and convert, as UTF-16 bytes.</param>
	/// <returns>An escaped <see cref="PlainText" />.</returns>
	public static implicit operator PlainText(ReadOnlySpan<char> unescapedUtf16)
	{
		return new(unescapedUtf16);
	}

	/// <summary>
	/// Implicit conversion from UTF-8 to <see cref="PlainText" />, escaping unsafe XML characters.
	/// </summary>
	/// <param name="unescapedUtf8">The text to escape and convert, as UTF-8 bytes.</param>
	/// <returns>An escaped <see cref="PlainText" />.</returns>
	public static implicit operator PlainText(ReadOnlySpan<byte> unescapedUtf8)
	{
		return new(unescapedUtf8);
	}

	/// <summary>
	/// Implicit conversion from <see cref="DateOnly" /> to a <see cref="PlainText" />.
	/// </summary>
	/// <param name="date">The date to convert.</param>
	/// <returns>An escaped <see cref="PlainText" />.</returns>
	public static implicit operator PlainText(DateOnly date) => new(date);
}
