// SPDX-License-Identifier: LGPL-3.0-or-later

namespace SharpTags.Nodes.Html.Attributes.Values;

/// <summary>
/// The possible auto-capitalize values.
/// </summary>
/// <seealso href="https://developer.mozilla.org/en-US/docs/Web/HTML/Reference/Global_attributes/autocapitalize" />
public enum AutoCapitalize
{
	/// <summary>
	/// Do not specify the auto-capitalize behavior. The default varies by browser.
	/// </summary>
	NotSpecified,

	/// <summary>
	/// Disables auto-capitalization.
	/// </summary>
	None,

	/// <summary>
	/// Automatically capitalizes the first character of each sentence.
	/// </summary>
	Sentences,

	/// <summary>
	/// Automatically capitalizes the first character of each word.
	/// </summary>
	Words,

	/// <summary>
	/// Automatically capitalizes all characters.
	/// </summary>
	Characters,
}
