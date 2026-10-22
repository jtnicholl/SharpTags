// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.Contracts;
using SharpTags.Nodes.Html.Attributes.Values;

namespace SharpTags.Nodes.Html.Attributes;

internal sealed class AutoCorrectAttribute : ITernaryAttribute
{
	public static ReadOnlyMemory<byte> TrueValue { get; } = (byte[])[
		..AttributeNames.AutoCorrect.Span,
		(byte)'"',
	];

	public static ReadOnlyMemory<byte> FalseValue { get; } = (byte[])[
		..AttributeNames.AutoCorrect.Span,
		..GlobalAttributeValues.Off.Span,
		(byte)'"',
	];

	[Pure]
	public static AttributeResult Evaluate(bool? value)
	{
		return value switch
		{
			true => new(ReadOnlyMemory<byte>.Empty),
			false => new(GlobalAttributeValues.Off),
			null => AttributeResult.NotPresent,
		};
	}
}
