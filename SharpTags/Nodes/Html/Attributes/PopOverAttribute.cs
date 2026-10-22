// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.Contracts;
using SharpTags.Nodes.Html.Attributes.Values;

namespace SharpTags.Nodes.Html.Attributes;

partial class PopOverAttribute
{
	[Pure]
	public static AttributeResult Evaluate(PopOver value)
	{
		return value switch
		{
			PopOver.Auto => new(GlobalAttributeValues.Auto),
			PopOver.Hint => new(GlobalAttributeValues.Hint),
			PopOver.Manual => new(GlobalAttributeValues.Manual),
			_ => AttributeResult.NotPresent,
		};
	}
}
