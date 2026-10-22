// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.Contracts;
using SharpTags.Nodes.Html.Attributes.Values;

namespace SharpTags.Nodes.Html.Attributes;

partial class PreloadAttribute
{
	[Pure]
	public static AttributeResult Evaluate(Preload value)
	{
		return value switch
		{
			Preload.None => new(GlobalAttributeValues.None),
			Preload.Metadata => new(MiscAttributeValues.Metadata),
			Preload.Auto => new(GlobalAttributeValues.Auto),
			_ => AttributeResult.NotPresent,
		};
	}
}
