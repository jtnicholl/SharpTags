// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.Contracts;
using SharpTags.Nodes.Html.Attributes.Values;

namespace SharpTags.Nodes.Html.Attributes;

partial class LoadingAttribute
{
	[Pure]
	public static AttributeResult Evaluate(Loading value)
	{
		return value switch
		{
			Loading.Lazy => new(GlobalAttributeValues.Lazy),
			_ => AttributeResult.NotPresent,
		};
	}
}
