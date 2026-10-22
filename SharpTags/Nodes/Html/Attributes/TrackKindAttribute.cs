// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.Contracts;
using SharpTags.Nodes.Html.Attributes.Values;

namespace SharpTags.Nodes.Html.Attributes;

partial class TrackKindAttribute
{
	[Pure]
	public static AttributeResult Evaluate(TrackKind value)
	{
		return value switch
		{
			TrackKind.Captions => new(MiscAttributeValues.Captions),
			TrackKind.Descriptions => new(MiscAttributeValues.Descriptions),
			TrackKind.Chapters => new(MiscAttributeValues.Chapters),
			TrackKind.Metadata => new(MiscAttributeValues.Metadata),
			_ => AttributeResult.NotPresent,
		};
	}
}
