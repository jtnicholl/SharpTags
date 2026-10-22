// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.Contracts;
using SharpTags.Nodes.Html.Attributes.Values;

namespace SharpTags.Nodes.Html.Attributes;

partial class DecodingAttribute
{
	[Pure]
	public static AttributeResult Evaluate(Decoding value)
	{
		return value switch
		{
			Decoding.Sync => new(GlobalAttributeValues.Sync),
			Decoding.Async => new(GlobalAttributeValues.Async),
			_ => AttributeResult.NotPresent,
		};
	}
}
