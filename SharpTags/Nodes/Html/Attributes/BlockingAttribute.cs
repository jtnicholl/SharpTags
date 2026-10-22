// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.Contracts;
using SharpTags.Nodes.Html.Attributes.Values;

namespace SharpTags.Nodes.Html.Attributes;

partial class BlockingAttribute
{
	[Pure]
	public static AttributeResult Evaluate(Blocking value)
	{
		return value == Blocking.Render
			? new(MiscAttributeValues.Render)
			: AttributeResult.NotPresent;
	}
}
