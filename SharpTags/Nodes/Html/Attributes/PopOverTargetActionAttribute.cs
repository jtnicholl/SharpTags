// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.Contracts;
using SharpTags.Nodes.Html.Attributes.Values;

namespace SharpTags.Nodes.Html.Attributes;

partial class PopOverTargetActionAttribute
{
	[Pure]
	public static AttributeResult Evaluate(PopOverTargetAction value)
	{
		return value switch
		{
			PopOverTargetAction.Hide => new(MiscAttributeValues.Hide),
			PopOverTargetAction.Show => new(MiscAttributeValues.Show),
			PopOverTargetAction.Toggle => new(MiscAttributeValues.Toggle),
			_ => AttributeResult.NotPresent,
		};
	}
}
