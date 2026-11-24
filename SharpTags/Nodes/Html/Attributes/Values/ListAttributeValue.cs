// SPDX-License-Identifier: LGPL-3.0-or-later

namespace SharpTags.Nodes.Html.Attributes.Values;

/// <summary>
/// Union type for lists of text strings.
/// </summary>
public readonly union ListAttributeValue(
	PlainText,
	IEnumerable<PlainText>,
	IEnumerable<string>
)
{
	/// <summary>
	/// This <see cref="ListAttributeValue" /> as an <see cref="IEnumerable{T}" />.
	/// </summary>
	public IEnumerable<PlainText>? Enumerable
	{
		get
		{
			return this switch
			{
				PlainText pt => [pt],
				IEnumerable<PlainText> ie => ie,
				IEnumerable<string> ie => ie.Select(s => new PlainText(s)),
				null => null,
			};
		}
	}

	/// <summary>
	/// Implicitly convert a <see cref="string" /> to a <see cref="ListAttributeValue" />.
	/// </summary>
	public static implicit operator ListAttributeValue(string? value)
	{
		return value is null ? default(ListAttributeValue) : new PlainText(value);
	}

	/// <summary>
	/// Implicitly convert a <see cref="ReadOnlySpan{T}" /> to a
	/// <see cref="ListAttributeValue{TModel}" />.
	/// </summary>
	public static implicit operator ListAttributeValue(ReadOnlySpan<byte> value)
	{
		return new PlainText(value);
	}
}
