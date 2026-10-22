// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.Contracts;
using SharpTags.Nodes.Html.Attributes.Values;

namespace SharpTags.Nodes.Html.Attributes;

partial class AutoCapitalizeAttribute
{
	[Pure]
	public static AttributeResult Evaluate(AutoCapitalize value)
	{
		return value switch
		{
			AutoCapitalize.None => new(GlobalAttributeValues.Off),
			AutoCapitalize.Sentences => new(GlobalAttributeValues.On),
			AutoCapitalize.Words => new(GlobalAttributeValues.Words),
			AutoCapitalize.Characters => new(GlobalAttributeValues.Characters),
			_ => AttributeResult.NotPresent,
		};
	}
}
