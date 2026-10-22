// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.Contracts;
using SharpTags.Nodes.Html.Attributes.Values;

namespace SharpTags.Nodes.Html.Attributes;

partial class TextDirectionAttribute
{
	[Pure]
	public static AttributeResult Evaluate(TextDirection value)
	{
		return value switch
		{
			TextDirection.LeftToRight => new(GlobalAttributeValues.LeftToRight),
			TextDirection.RightToLeft => new(GlobalAttributeValues.RightToLeft),
			TextDirection.Auto => new(GlobalAttributeValues.Auto),
			_ => AttributeResult.NotPresent,
		};
	}
}
