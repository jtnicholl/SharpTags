// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.Contracts;
using SharpTags.Nodes.Html.Attributes.Values;

namespace SharpTags.Nodes.Html.Attributes;

partial class ControlsListAttribute
{
	[Pure]
	public static AttributeResult Evaluate(ControlsList value)
	{
		return value switch
		{
			ControlsList.NoDownload => new(MiscAttributeValues.NoDownload),
			ControlsList.NoFullscreen => new(MiscAttributeValues.NoFullscreen),
			ControlsList.NoRemotePlayback => new(MiscAttributeValues.NoRemotePlayback),
			_ => AttributeResult.NotPresent,
		};
	}
}
