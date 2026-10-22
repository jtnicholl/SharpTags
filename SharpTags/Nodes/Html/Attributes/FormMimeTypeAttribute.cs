// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.Contracts;
using SharpTags.Nodes.Html.Attributes.Values;

namespace SharpTags.Nodes.Html.Attributes;

partial class FormMimeTypeAttribute
{
	[Pure]
	public static AttributeResult Evaluate(FormMimeType value)
	{
		return value switch
		{
			FormMimeType.FormUrlEncoded => new(InputAttributes.FormUrlEncoded),
			FormMimeType.FormData => new(InputAttributes.FormData),
			FormMimeType.PlainText => new(InputAttributes.PlainText),
			_ => AttributeResult.NotPresent,
		};
	}
}
