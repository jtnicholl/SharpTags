// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.Contracts;
using SharpTags.Nodes.Html.Attributes.Values;

namespace SharpTags.Nodes.Html.Attributes;

partial class FormMethodAttribute
{
	[Pure]
	public static AttributeResult Evaluate(FormMethod value)
	{
		return value switch
		{
			FormMethod.Get => new(InputAttributes.Get),
			FormMethod.Post => new(InputAttributes.Post),
			FormMethod.Dialog => new(InputAttributes.Dialog),
			_ => AttributeResult.NotPresent,
		};
	}
}
