// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.Contracts;
using SharpTags.Nodes.Html.Attributes.Values;

namespace SharpTags.Nodes.Html.Attributes;

partial class FetchPriorityAttribute
{
	[Pure]
	public static AttributeResult Evaluate(FetchPriority value)
	{
		return value switch
		{
			FetchPriority.Low => new(GlobalAttributeValues.Low),
			FetchPriority.High => new(GlobalAttributeValues.High),
			_ => AttributeResult.NotPresent,
		};
	}
}
