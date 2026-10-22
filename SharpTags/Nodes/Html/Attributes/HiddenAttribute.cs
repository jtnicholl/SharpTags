// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.Contracts;
using SharpTags.Nodes.Html.Attributes.Values;

namespace SharpTags.Nodes.Html.Attributes;

partial class HiddenAttribute
{
	[Pure]
	public static AttributeResult Evaluate(Hidden value)
	{
		return value switch
		{
			Hidden.Hidden => AttributeResult.NoValue,
			Hidden.UntilFound => new(GlobalAttributeValues.UntilFound),
			_ => AttributeResult.NotPresent,
		};
	}
}
