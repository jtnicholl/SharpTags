// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.Contracts;
using SharpTags.Nodes.Html.Attributes.Values;

namespace SharpTags.Nodes.Html.Attributes;

internal sealed class SpellCheckAttribute : ITernaryAttribute
{
	public static ReadOnlyMemory<byte> Name => AttributeNames.SpellCheck;

	public static ReadOnlyMemory<byte> TrueValue { get; } = (byte[])
	[
		..AttributeNames.SpellCheck.Span,
		(byte)'"',
	];

	public static ReadOnlyMemory<byte> FalseValue { get; } = (byte[])
	[
		..AttributeNames.SpellCheck.Span,
		..GlobalAttributeValues.False.Span,
		(byte)'"',
	];

	[Pure]
	public static AttributeResult Evaluate(bool? value)
	{
		return value switch
		{
			true => new(ReadOnlyMemory<byte>.Empty),
			false => new(GlobalAttributeValues.False),
			null => AttributeResult.NotPresent,
		};
	}
}
