// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.Contracts;
using SharpTags.Nodes.Html.Attributes.Values;

namespace SharpTags.Nodes.Html.Attributes;

partial class ShadowRootModeAttribute
{
	[Pure]
	public static AttributeResult Evaluate(ShadowRootMode value)
	{
		return value switch
		{
			ShadowRootMode.Open => new(MiscAttributeValues.Open),
			ShadowRootMode.Closed => new(MiscAttributeValues.Closed),
			_ => AttributeResult.NotPresent,
		};
	}
}
