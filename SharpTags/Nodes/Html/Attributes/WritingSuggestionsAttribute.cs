// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.Contracts;
using SharpTags.Nodes.Html.Attributes.Values;

namespace SharpTags.Nodes.Html.Attributes;

internal sealed class WritingSuggestionsAttribute : ITernaryAttribute
{
	public static ReadOnlyMemory<byte> Name => AttributeNames.WritingSuggestions;

	public static ReadOnlyMemory<byte> TrueValue { get; } = AttributeNames.WritingSuggestions[..^2];

	public static ReadOnlyMemory<byte> FalseValue { get; } = (byte[])
	[
		..AttributeNames.WritingSuggestions.Span,
		..GlobalAttributeValues.False.Span,
		(byte)'"',
	];

	[Pure]
	public static AttributeResult Evaluate(bool? value)
	{
		return value switch
		{
			true => AttributeResult.NoValue,
			false => new(GlobalAttributeValues.False),
			null => AttributeResult.NotPresent,
		};
	}
}
