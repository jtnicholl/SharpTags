// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.Contracts;
using SharpTags.Nodes.Html.Attributes.Values;

namespace SharpTags.Nodes.Html.Attributes;

partial class OrderedListTypeAttribute
{
	[Pure]
	public static AttributeResult Evaluate(OrderedListType value)
	{
		return value switch
		{
			OrderedListType.LowercaseLetters => new(MiscAttributeValues.LowercaseLetters),
			OrderedListType.UppercaseLetters => new(MiscAttributeValues.UppercaseLetters),
			OrderedListType.LowercaseRomanNumerals =>
				new(MiscAttributeValues.LowercaseRomanNumerals),
			OrderedListType.UppercaseRomanNumerals =>
				new(MiscAttributeValues.UppercaseRomanNumerals),
			_ => AttributeResult.NotPresent,
		};
	}
}
