// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.Contracts;
using SharpTags.Nodes.Html.Attributes.Values;

namespace SharpTags.Nodes.Html.Attributes;

partial class ShapeAttribute
{
	[Pure]
	public static AttributeResult Evaluate(Shape value)
	{
		return value switch
		{
			Shape.Rectangle => new(MiscAttributeValues.Rectangle),
			Shape.Circle => new(MiscAttributeValues.Circle),
			Shape.Polygon => new(MiscAttributeValues.Polygon),
			Shape.Default => new(MiscAttributeValues.Default),
			_ => AttributeResult.NotPresent,
		};
	}
}
