// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.Contracts;
using SharpTags.Nodes.Html.Attributes.Values;

namespace SharpTags.Nodes.Html.Attributes;

partial class WrapAttribute
{
	[Pure]
	public static AttributeResult Evaluate(Wrap value)
	{
		return value == Wrap.Hard ? new(MiscAttributeValues.Hard) : AttributeResult.NotPresent;
	}
}
