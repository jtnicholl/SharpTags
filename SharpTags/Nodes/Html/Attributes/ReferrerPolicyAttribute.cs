// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.Contracts;
using SharpTags.Nodes.Html.Attributes.Values;

namespace SharpTags.Nodes.Html.Attributes;

partial class ReferrerPolicyAttribute
{
	[Pure]
	public static AttributeResult Evaluate(ReferrerPolicy value)
	{
		return value switch
		{
			ReferrerPolicy.NoReferrer => new(GlobalAttributeValues.NoReferrer),
			ReferrerPolicy.NoReferrerWhenDowngrade => new(GlobalAttributeValues.NoReferrerWhenDowngrade),
			ReferrerPolicy.Origin => new(GlobalAttributeValues.Origin),
			ReferrerPolicy.OriginWhenCrossOrigin => new(GlobalAttributeValues.OriginWhenCrossOrigin),
			ReferrerPolicy.SameOrigin => new(GlobalAttributeValues.SameOrigin),
			ReferrerPolicy.StrictOrigin => new(GlobalAttributeValues.StrictOrigin),
			ReferrerPolicy.StrictOriginWhenCrossOrigin => new(GlobalAttributeValues.StrictOriginWhenCrossOrigin),
			ReferrerPolicy.UnsafeUrl => new(GlobalAttributeValues.UnsafeUrl),
			_ => AttributeResult.NotPresent,
		};
	}
}
