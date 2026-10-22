// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.Contracts;
using SharpTags.Nodes.Html.Attributes.Values;

namespace SharpTags.Nodes.Html.Attributes;

partial class CrossOriginAttribute
{
	[Pure]
	public static AttributeResult Evaluate(CrossOrigin value)
	{
		return value switch
		{
			CrossOrigin.Anonymous => new(MiscAttributeValues.Anonymous),
			CrossOrigin.UseCredentials => new(MiscAttributeValues.UseCredentials),
			_ => AttributeResult.NotPresent,
		};
	}
}
