// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.Contracts;
using SharpTags.Nodes.Html.Attributes.Values;

namespace SharpTags.Nodes.Html.Attributes;

internal sealed class DraggableAttribute : ITernaryAttribute
{
	public static ReadOnlyMemory<byte> Name => AttributeNames.Draggable;

	public static ReadOnlyMemory<byte> TrueValue { get; } = (byte[])
	[
		..AttributeNames.Draggable.Span,
		..GlobalAttributeValues.True.Span,
		(byte)'"',
	];

	public static ReadOnlyMemory<byte> FalseValue { get; } = (byte[])
	[
		..AttributeNames.Draggable.Span,
		..GlobalAttributeValues.False.Span,
		(byte)'"',
	];

	[Pure]
	public static AttributeResult Evaluate(bool? value)
	{
		return value switch
		{
			true => new(GlobalAttributeValues.True),
			false => new(GlobalAttributeValues.False),
			null => AttributeResult.NotPresent,
		};
	}
}
