// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;

namespace SharpTags.Nodes.Html.Attributes;

internal readonly struct AttributeResult : IEquatable<AttributeResult>
{
	public enum ResultType
	{
		/// <summary>
		/// Indicates that an attribute should not be present at all.
		/// </summary>
		NotPresent,

		/// <summary>
		/// Indicates that an attribute should be present with no value, not even an empty string.
		/// </summary>
		NoValue,

		/// <summary>
		/// Indicates that an attribute should be present with a value (which could be empty).
		/// </summary>
		HasValue,
	}

	public static readonly AttributeResult NotPresent = new(ResultType.NotPresent);
	public static readonly AttributeResult NoValue = new(ResultType.NoValue);

	public ResultType Result { get; }

	public ReadOnlyMemory<byte> Value { get; }

	public bool HasValue => Result == ResultType.HasValue;

	public bool IsPresent => Result != ResultType.NotPresent;

	public AttributeResult(int? value)
	{
		if (value is not int i)
		{
			this = default;
			return;
		}

		Result = ResultType.HasValue;
		Span<byte> buffer = stackalloc byte[11];
		var success = i.TryFormat(
			buffer,
			out var written,
			default,
			CultureInfo.InvariantCulture
		);
		Contract.Assert(success);
		Contract.Assert(written > 0);
		Value = buffer[..written].ToArray();
	}

	public AttributeResult(double? value)
	{
		if (value is not double d || !double.IsFinite(d))
		{
			this = default;
			return;
		}

		Result = ResultType.HasValue;
		Span<byte> buffer = stackalloc byte[24] /* TODO confirm max length */;
		var success = d.TryFormat(
			buffer,
			out var written,
			default,
			CultureInfo.InvariantCulture
		);
		Contract.Assert(success);
		Contract.Assert(written > 0);
		Value = buffer[..written].ToArray();
	}

	public AttributeResult(DateOnly? value)
	{
		if (value is not DateOnly d)
		{
			this = default;
			return;
		}

		Result = ResultType.HasValue;
		Value = new PlainText(d).Utf8Bytes;
	}

	public AttributeResult(DateTime? value)
	{
		if (value is not DateTime dt)
		{
			this = default;
			return;
		}

		Result = ResultType.HasValue;
		Value = new PlainText(dt).Utf8Bytes;
	}

	public AttributeResult(ReadOnlyMemory<byte> value)
	{
		Result = ResultType.HasValue;
		Value = value;
	}

	private AttributeResult(ResultType result)
	{
		Result = result;
	}

	public bool Equals(AttributeResult other)
	{
		return Result == other.Result && Value.Span.SequenceEqual(other.Value.Span);
	}

	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		return obj is AttributeResult other && Equals(other);
	}

	public override int GetHashCode()
	{
		HashCode output = new();

		output.Add(Result);
		output.AddBytes(Value.Span);

		return output.ToHashCode();
	}

	public static implicit operator AttributeResult(PlainText text)
	{
		return new(text.Utf8Bytes);
	}

	public static bool operator ==(AttributeResult left, AttributeResult right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(AttributeResult left, AttributeResult right)
	{
		return !left.Equals(right);
	}
}
