// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.Contracts;
using SharpTags.Nodes.Html.Attributes.Values;

namespace SharpTags.Nodes.Html.Attributes;

partial class HttpEquivalentAttribute
{
	[Pure]
	public static AttributeResult Evaluate(HttpEquivalent value)
	{
		return value switch
		{
			HttpEquivalent.ContentType => new(MiscAttributeValues.ContentType),
			HttpEquivalent.ContentSecurityPolicy => new(MiscAttributeValues.ContentSecurityPolicy),
			HttpEquivalent.DefaultStyle => new(MiscAttributeValues.DefaultStyle),
			HttpEquivalent.Refresh => new(MiscAttributeValues.Refresh),
			_ => AttributeResult.NotPresent,
		};
	}
}
