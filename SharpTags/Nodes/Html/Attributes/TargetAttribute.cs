// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.Contracts;
using SharpTags.Nodes.Html.Attributes.Values;

namespace SharpTags.Nodes.Html.Attributes;

partial class TargetAttribute
{
	[Pure]
	public static AttributeResult Evaluate(Target value)
	{
		return value switch
		{
			Target.Self => new(MiscAttributeValues.Self),
			Target.Blank => new(MiscAttributeValues.Blank),
			Target.Parent => new(MiscAttributeValues.Parent),
			Target.Top => new(MiscAttributeValues.Top),
			Target.UnreferencedTop => new(MiscAttributeValues.UnreferencedTop),
			_ => AttributeResult.NotPresent,
		};
	}
}
