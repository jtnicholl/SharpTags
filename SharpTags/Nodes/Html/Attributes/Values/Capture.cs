// SPDX-License-Identifier: LGPL-3.0-or-later

namespace SharpTags.Nodes.Html.Attributes.Values;

/// <summary>
/// Values for the capture attribute, which provides a suggestion to capture new
/// media (photo, video, or audio) for a file input.
/// </summary>
/// <seealso href="https://developer.mozilla.org/en-US/docs/Web/HTML/Reference/Attributes/capture" />
public enum Capture
{
	/// <summary>
	/// No capture suggestion is provided, the client should upload an existing file.
	/// </summary>
	NotSpecified,

	/// <summary>
	/// Suggest using the user-facing camera and/or microphone.
	/// </summary>
	User,

	/// <summary>
	/// Suggest using the outward-facing camera and/or microphone.
	/// </summary>
	Environment,
}
