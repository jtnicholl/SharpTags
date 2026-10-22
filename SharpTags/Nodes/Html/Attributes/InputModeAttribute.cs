// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.Contracts;
using SharpTags.Nodes.Html.Attributes.Values;

namespace SharpTags.Nodes.Html.Attributes;

partial class InputModeAttribute
{
	[Pure]
	public static AttributeResult Evaluate(InputMode value)
	{
		return value switch
		{
			InputMode.None => new(GlobalAttributeValues.None),
			InputMode.Decimal => new(GlobalAttributeValues.Decimal),
			InputMode.Numeric => new(GlobalAttributeValues.Numeric),
			InputMode.Telephone => new(GlobalAttributeValues.Tel),
			InputMode.Search => new(GlobalAttributeValues.Search),
			InputMode.Email => new(GlobalAttributeValues.Email),
			InputMode.Url => new(GlobalAttributeValues.Url),
			_ => AttributeResult.NotPresent,
		};
	}
}
