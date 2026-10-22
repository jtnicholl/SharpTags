// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.Contracts;
using SharpTags.Nodes.Html.Attributes.Values;

namespace SharpTags.Nodes.Html.Attributes;

partial class EnterKeyHintAttribute
{
	[Pure]
	public static AttributeResult Evaluate(EnterKeyHint value)
	{
		return value switch
		{
			EnterKeyHint.Enter => new(GlobalAttributeValues.Enter),
			EnterKeyHint.Done => new(GlobalAttributeValues.Done),
			EnterKeyHint.Go => new(GlobalAttributeValues.Go),
			EnterKeyHint.Next => new(GlobalAttributeValues.Next),
			EnterKeyHint.Previous => new(GlobalAttributeValues.Previous),
			EnterKeyHint.Search => new(GlobalAttributeValues.Search),
			EnterKeyHint.Send => new(GlobalAttributeValues.Send),
			_ => AttributeResult.NotPresent,
		};
	}
}
