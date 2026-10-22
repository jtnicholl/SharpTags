// SPDX-License-Identifier: LGPL-3.0-or-later

using SharpTags.Nodes.Html.Elements;

namespace SharpTags.Nodes.Html.Attributes.Values;

/// <summary>
/// The types of <see cref="Button{TModel}" />.
/// </summary>
/// <seealso href="https://developer.mozilla.org/en-US/docs/Web/HTML/Reference/Elements/button#type" />
public enum ButtonType
{
	/// <summary>
	/// Do not specify the button type. The default varies by where the button is placed.
	/// </summary>
	NotSpecified,

	/// <summary>
	/// A button for submitting a <see cref="Form{TModel}" />.
	/// </summary>
	Submit,

	/// <summary>
	/// A button which resets a <see cref="Form{TModel}" />.
	/// </summary>
	Reset,

	/// <summary>
	/// A generic button with no default behavior.
	/// </summary>
	Button,
}
