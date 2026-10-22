// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Immutable;
using System.Text;

namespace SharpTags.UnitTests;

[TestFixture]
internal sealed class PlainTextTests
{
	[Test]
	public void UnsafeRaw_FromString_DoesNotEscape()
	{
		var text = PlainText.UnsafeRaw("UnsafeString&<>\"");

		var expected = "UnsafeString&<>\""u8;
		using var _ = Assert.EnterMultipleScope();
		Assert.That(text.ToString(), Is.EqualTo(Encoding.UTF8.GetString(expected)));
		Assert.That(text.ByteLength, Is.EqualTo(expected.Length));
		Assert.That(text.Utf8Bytes.Span.SequenceEqual(expected), Is.True);
	}

	[Test]
	public void UnsafeRaw_FromByteSpan_DoesNotEscape()
	{
		var text = PlainText.UnsafeRaw("UnsafeString&<>\""u8);

		var expected = "UnsafeString&<>\""u8;
		using var _ = Assert.EnterMultipleScope();
		Assert.That(text.ToString(), Is.EqualTo(Encoding.UTF8.GetString(expected)));
		Assert.That(text.ByteLength, Is.EqualTo(expected.Length));
		Assert.That(text.Utf8Bytes.Span.SequenceEqual(expected), Is.True);
	}

	[Test]
	public void UnsafeRaw_FromByteArray_DoesNotEscape()
	{
		var text = PlainText.UnsafeRaw("UnsafeString&<>\""u8.ToImmutableArray());

		var expected = "UnsafeString&<>\""u8;
		using var _ = Assert.EnterMultipleScope();
		Assert.That(text.ToString(), Is.EqualTo(Encoding.UTF8.GetString(expected)));
		Assert.That(text.ByteLength, Is.EqualTo(expected.Length));
		Assert.That(text.Utf8Bytes.Span.SequenceEqual(expected), Is.True);
	}

	[Test]
	public void UnsafeRaw_FromByteArrayRange_SlicesAndDoesNotEscape()
	{
		var text = PlainText.UnsafeRaw("UnsafeString&<>\""u8.ToImmutableArray(), 4);

		var expected = "feString&<>\""u8;
		using var _ = Assert.EnterMultipleScope();
		Assert.That(text.ToString(), Is.EqualTo(Encoding.UTF8.GetString(expected)));
		Assert.That(text.ByteLength, Is.EqualTo(expected.Length));
		Assert.That(text.Utf8Bytes.Span.SequenceEqual(expected), Is.True);
	}

	[Test]
	public void UnsafeRaw_FromByteArraySlice_SlicesAndDoesNotEscape()
	{
		var text = PlainText.UnsafeRaw("Unsafe&<>\"String"u8.ToImmutableArray(), 4, 8);

		var expected = "fe&<>\"St"u8;
		using var _ = Assert.EnterMultipleScope();
		Assert.That(text.ToString(), Is.EqualTo(Encoding.UTF8.GetString(expected)));
		Assert.That(text.ByteLength, Is.EqualTo(expected.Length));
		Assert.That(text.Utf8Bytes.Span.SequenceEqual(expected), Is.True);
	}

	[Test]
	public void DefaultConstructor_IsEmpty()
	{
		PlainText text = new();

		using var _ = Assert.EnterMultipleScope();
		Assert.That(text.ToString(), Is.Empty);
		Assert.That(text.IsEmpty, Is.True);
	}

	[Test]
	public void StringConstructor_FromString_Escapes()
	{
		PlainText text = new("UnsafeString&<>\"");

		var expected = "UnsafeString&amp;&lt;&gt;&quot;"u8;
		using var _ = Assert.EnterMultipleScope();
		Assert.That(text.ToString(), Is.EqualTo(Encoding.UTF8.GetString(expected)));
		Assert.That(text.ByteLength, Is.EqualTo(expected.Length));
		Assert.That(text.Utf8Bytes.Span.SequenceEqual(expected), Is.True);
	}

	[Test]
	public void StringConstructor_FromNull_IsEmpty()
	{
		PlainText text = new((string?)null);

		using var _ = Assert.EnterMultipleScope();
		Assert.That(text.ToString(), Is.Empty);
		Assert.That(text.IsEmpty, Is.True);
	}

	[Test]
	public void CharSpanConstructor_FromString_Escapes()
	{
		PlainText text = new("UnsafeString&<>\"".AsSpan());

		var expected = "UnsafeString&amp;&lt;&gt;&quot;"u8;
		using var _ = Assert.EnterMultipleScope();
		Assert.That(text.ToString(), Is.EqualTo(Encoding.UTF8.GetString(expected)));
		Assert.That(text.ByteLength, Is.EqualTo(expected.Length));
		Assert.That(text.Utf8Bytes.Span.SequenceEqual(expected), Is.True);
	}

	[Test]
	public void ByteSpanConstructor_FromBytes_Escapes()
	{
		PlainText text = new("UnsafeString&<>\""u8);

		var expected = "UnsafeString&amp;&lt;&gt;&quot;"u8;
		using var _ = Assert.EnterMultipleScope();
		Assert.That(text.ToString(), Is.EqualTo(Encoding.UTF8.GetString(expected)));
		Assert.That(text.ByteLength, Is.EqualTo(expected.Length));
		Assert.That(text.Utf8Bytes.Span.SequenceEqual(expected), Is.True);
	}

	[Test]
	public void DateOnlyConstructor_WithSingleDigitMonthAndDay_IsZeroPadded()
	{
		PlainText text = new(new DateOnly(2026, 8, 1));

		var expected = "2026-08-01"u8;
		using var _ = Assert.EnterMultipleScope();
		Assert.That(text.ToString(), Is.EqualTo(Encoding.UTF8.GetString(expected)));
		Assert.That(text.ByteLength, Is.EqualTo(expected.Length));
		Assert.That(text.Utf8Bytes.Span.SequenceEqual(expected), Is.True);
	}

	[Test]
	public void DateOnlyConstructor_WithSingleDigitYear_IsZeroPadded()
	{
		PlainText text = new(new DateOnly(8, 8, 1));

		var expected = "0008-08-01"u8;
		using var _ = Assert.EnterMultipleScope();
		Assert.That(text.ToString(), Is.EqualTo(Encoding.UTF8.GetString(expected)));
		Assert.That(text.ByteLength, Is.EqualTo(expected.Length));
		Assert.That(text.Utf8Bytes.Span.SequenceEqual(expected), Is.True);
	}

	[Test]
	public void DateTimeConstructor_WithZeroMinutesAndSeconds_HasMinutesWithoutSeconds()
	{
		PlainText text = new(new DateTime(2026, 8, 1, 13, 0, 0));

		var expected = "2026-08-01T13:00"u8;
		using var _ = Assert.EnterMultipleScope();
		Assert.That(text.ToString(), Is.EqualTo(Encoding.UTF8.GetString(expected)));
		Assert.That(text.ByteLength, Is.EqualTo(expected.Length));
		Assert.That(text.Utf8Bytes.Span.SequenceEqual(expected), Is.True);
	}

	[Test]
	public void DateTimeConstructor_WithOneSecond_HasSecondsWithoutDecimal()
	{
		PlainText text = new(new DateTime(2026, 8, 1, 13, 0, 1));

		var expected = "2026-08-01T13:00:01"u8;
		using var _ = Assert.EnterMultipleScope();
		Assert.That(text.ToString(), Is.EqualTo(Encoding.UTF8.GetString(expected)));
		Assert.That(text.ByteLength, Is.EqualTo(expected.Length));
		Assert.That(text.Utf8Bytes.Span.SequenceEqual(expected), Is.True);
	}

	[Test]
	public void DateTimeConstructor_WithHundredMilliseconds_HasOneDecimalPlace()
	{
		PlainText text = new(new DateTime(2026, 8, 1, 13, 0, 0, 100));

		var expected = "2026-08-01T13:00:00.1"u8;
		using var _ = Assert.EnterMultipleScope();
		Assert.That(text.ToString(), Is.EqualTo(Encoding.UTF8.GetString(expected)));
		Assert.That(text.ByteLength, Is.EqualTo(expected.Length));
		Assert.That(text.Utf8Bytes.Span.SequenceEqual(expected), Is.True);
	}

	[Test]
	public void DateTimeConstructor_WithOneMicrosecond_HasSixDecimalPlaces()
	{
		PlainText text = new(new DateTime(2026, 8, 1, 13, 0, 0, 0, 1));

		var expected = "2026-08-01T13:00:00.000001"u8;
		using var _ = Assert.EnterMultipleScope();
		Assert.That(text.ToString(), Is.EqualTo(Encoding.UTF8.GetString(expected)));
		Assert.That(text.ByteLength, Is.EqualTo(expected.Length));
		Assert.That(text.Utf8Bytes.Span.SequenceEqual(expected), Is.True);
	}

	[Test]
	public void DateTimeConstructor_WithHundredNanoseconds_HasMaxDecimalPlaces()
	{
		DateTime datetime = new(new DateTime(2026, 8, 1, 13, 0, 0).Ticks + 1);

		PlainText text = new(datetime);

		var expected = "2026-08-01T13:00:00.0000001"u8;
		using var _ = Assert.EnterMultipleScope();
		Assert.That(text.ToString(), Is.EqualTo(Encoding.UTF8.GetString(expected)));
		Assert.That(text.ByteLength, Is.EqualTo(expected.Length));
		Assert.That(text.Utf8Bytes.Span.SequenceEqual(expected), Is.True);
	}

	[Test]
	public void FormattableConstructor_FromDateTime_Formats()
	{
		IUtf8SpanFormattable formattable = new DateTime(
			2026,
			8,
			11,
			13,
			30,
			0,
			105,
			1,
			DateTimeKind.Utc
		);
		PlainText text = new(formattable, "O");

		var expected = "2026-08-11T13:30:00.1050010Z"u8;
		using var _ = Assert.EnterMultipleScope();
		Assert.That(text.ToString(), Is.EqualTo(Encoding.UTF8.GetString(expected)));
		Assert.That(text.ByteLength, Is.EqualTo(expected.Length));
		Assert.That(text.Utf8Bytes.Span.SequenceEqual(expected));
	}

	[Test]
	public void ImplicitConversion_FromString_Escapes()
	{
		PlainText text = "UnsafeString&<>\"";

		var expected = "UnsafeString&amp;&lt;&gt;&quot;"u8;
		using var _ = Assert.EnterMultipleScope();
		Assert.That(text.ToString(), Is.EqualTo(Encoding.UTF8.GetString(expected)));
		Assert.That(text.ByteLength, Is.EqualTo(expected.Length));
		Assert.That(text.Utf8Bytes.Span.SequenceEqual(expected));
	}

	[Test]
	public void ImplicitConversion_FromByteSpan_Escapes()
	{
		PlainText text = "UnsafeString&<>\""u8;

		var expected = "UnsafeString&amp;&lt;&gt;&quot;"u8;
		using var _ = Assert.EnterMultipleScope();
		Assert.That(text.ToString(), Is.EqualTo(Encoding.UTF8.GetString(expected)));
		Assert.That(text.ByteLength, Is.EqualTo(expected.Length));
		Assert.That(text.Utf8Bytes.Span.SequenceEqual(expected));
	}

	[Test]
	public void Equals_FromImmutableArraySlices_ComparesByValue()
	{
		var array = "ababaText   Text888"u8.ToImmutableArray();
		PlainText textA = PlainText.UnsafeRaw(array, 5, 4);
		PlainText textB = PlainText.UnsafeRaw(array, 12, 4);

		var result = textA.Equals(textB);

		Assert.That(result, Is.True);
	}

	[Test]
	public void Equals_FromSpans_ComparesByValue()
	{
		PlainText textA = "Text".AsSpan();
		PlainText textB = "Text"u8;

		var result = textA.Equals(textB);

		Assert.That(result, Is.True);
	}

	[Test]
	public void HashCode_FromImmutableArraySlices_ComparesByValue()
	{
		var array = "ababaText   Text888"u8.ToImmutableArray();
		PlainText textA = PlainText.UnsafeRaw(array, 5, 4);
		PlainText textB = PlainText.UnsafeRaw(array, 12, 4);
		PlainText defaultText = new();

		var resultA = textA.GetHashCode();
		var resultB = textB.GetHashCode();
		var resultDefault = defaultText.GetHashCode();

		using var _ = Assert.EnterMultipleScope();
		Assert.That(resultA, Is.EqualTo(resultB));
		Assert.That(resultA, Is.Not.EqualTo(resultDefault));
	}

	[Test]
	public void HashCode_FromSpans_ComparesByValue()
	{
		PlainText textA = "Text".AsSpan();
		PlainText textB = "Text"u8;
		PlainText defaultText = new();

		var resultA = textA.GetHashCode();
		var resultB = textB.GetHashCode();
		var resultDefault = defaultText.GetHashCode();

		using var _ = Assert.EnterMultipleScope();
		Assert.That(resultA, Is.EqualTo(resultB));
		Assert.That(resultA, Is.Not.EqualTo(resultDefault));
	}
}
