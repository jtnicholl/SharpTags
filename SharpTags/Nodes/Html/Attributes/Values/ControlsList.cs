// SPDX-License-Identifier: LGPL-3.0-or-later

using SharpTags.Nodes.Html.Elements;

namespace SharpTags.Nodes.Html.Attributes.Values;

/// <summary>
/// Specifies available controls for <see cref="Audio{TModel}" /> and <see cref="Video{TModel}" />.
/// </summary>
public enum ControlsList
{
	NotSpecified,
	NoDownload,
	NoFullscreen,
	NoRemotePlayback,
}
