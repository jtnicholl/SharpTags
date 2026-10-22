// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.Contracts;
using SharpTags.Nodes.Html.Attributes.Values;

namespace SharpTags.Nodes.Html.Attributes;

partial class ButtonTypeAttribute
{
	[Pure]
	public static AttributeResult Evaluate(ButtonType value)
	{
		return value switch
		{
			ButtonType.Button => new(InputAttributes.Button),
			ButtonType.Reset => new(InputAttributes.Reset),
			ButtonType.Submit => new(InputAttributes.Submit),
			_ => AttributeResult.NotPresent,
		};
	}
}
