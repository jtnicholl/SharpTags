// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.Contracts;
using SharpTags.Nodes.Html.Attributes.Values;

namespace SharpTags.Nodes.Html.Attributes;

partial class ContentEditableAttribute
{
	[Pure]
	public static AttributeResult Evaluate(ContentEditable value)
	{
		return value switch
		{
			ContentEditable.True => AttributeResult.NoValue,
			ContentEditable.False => new(GlobalAttributeValues.False),
			ContentEditable.PlainTextOnly => new(GlobalAttributeValues.PlainTextOnly),
			_ => AttributeResult.NotPresent,
		};
	}
}
