// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.Contracts;
using SharpTags.Nodes.Html.Attributes.Values;

namespace SharpTags.Nodes.Html.Attributes;

partial class ClosedByAttribute
{
	[Pure]
	public static AttributeResult Evaluate(ClosedBy value)
	{
		return value switch
		{
			ClosedBy.Any => new(MiscAttributeValues.Any),
			ClosedBy.CloseRequest => new(MiscAttributeValues.CloseRequest),
			ClosedBy.None => new(GlobalAttributeValues.None),
			_ => AttributeResult.NotPresent,
		};
	}
}
