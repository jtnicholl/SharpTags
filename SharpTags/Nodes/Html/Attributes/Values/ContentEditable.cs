// SPDX-License-Identifier: LGPL-3.0-or-later

namespace SharpTags.Nodes.Html.Attributes.Values;

/// <summary>
/// Specifies the ability for the user to edit text.
/// </summary>
public enum ContentEditable
{
	/// <summary>
	/// Do not specify the editability. The value will be inherited from the parent element.
	/// </summary>
	NotSpecified,

	/// <summary>
	/// The content may be edited as rich text.
	/// </summary>
	True,

	/// <summary>
	/// The content may not be edited.
	/// </summary>
	False,

	/// <summary>
	/// The content may be edited as plain text.
	/// </summary>
	PlainTextOnly,
}
