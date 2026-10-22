// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.Contracts;
using SharpTags.Nodes.Html.Attributes.Values;

namespace SharpTags.Nodes.Html.Attributes;

partial class InputTypeAttribute
{
	[Pure]
	public static AttributeResult Evaluate(InputType value)
	{
		return value switch
		{
			InputType.Button => new(InputAttributes.Button),
			InputType.Checkbox => new(InputAttributes.Checkbox),
			InputType.Color => new(InputAttributes.Color),
			InputType.PlainDate => new(InputAttributes.PlainDate),
			InputType.PlainDateTime => new(InputAttributes.PlainDateTime),
			InputType.Email => new(InputAttributes.Email),
			InputType.File => new(InputAttributes.File),
			InputType.Hidden => new(InputAttributes.Hidden),
			InputType.Image => new(InputAttributes.Image),
			InputType.PlainMonth => new(InputAttributes.PlainMonth),
			InputType.Number => new(InputAttributes.Number),
			InputType.Password => new(InputAttributes.Password),
			InputType.Radio => new(InputAttributes.Radio),
			InputType.Range => new(InputAttributes.Range),
			InputType.Reset => new(InputAttributes.Reset),
			InputType.Search => new(InputAttributes.Search),
			InputType.Submit => new(InputAttributes.Submit),
			InputType.Telephone => new(InputAttributes.Telephone),
			InputType.PlainTime => new(InputAttributes.PlainTime),
			InputType.Url => new(InputAttributes.Url),
			InputType.PlainWeek => new(InputAttributes.PlainWeek),
			_ => AttributeResult.NotPresent,
		};
	}
}
