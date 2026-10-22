// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.Contracts;
using SharpTags.Nodes.Html.Attributes.Values;

namespace SharpTags.Nodes.Html.Attributes;

partial class CaptureAttribute
{
	[Pure]
	public static AttributeResult Evaluate(Capture value)
	{
		return value switch
		{
			Capture.User => new(InputAttributes.User),
			Capture.Environment => new(InputAttributes.Environment),
			_ => AttributeResult.NotPresent,
		};
	}
}
