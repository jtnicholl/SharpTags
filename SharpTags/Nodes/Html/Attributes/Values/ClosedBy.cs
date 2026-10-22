// SPDX-License-Identifier: LGPL-3.0-or-later

using SharpTags.Nodes.Html.Elements;

namespace SharpTags.Nodes.Html.Attributes.Values;

/// <summary>
/// The actions by which a <see cref="Dialog{TModel}" /> may be dismissed.
/// </summary>
/// <seealso href="https://developer.mozilla.org/en-US/docs/Web/HTML/Reference/Elements/dialog#closedby" />
public enum ClosedBy
{
	/// <summary>
	/// Do not specify the closing actions. The default varies by how the dialog was opened.
	/// </summary>
	NotSpecified,

	/// <summary>
	/// The dialog may be closed by any possible action:
	/// <ul>
	/// <li>A light dismissal (such as clicking or tapping outside),</li>
	/// <li>Platform-specific dismissal (such as the back button or escape key),</li>
	/// <li>By the page (via JavaScript or a <see cref="Form{TModel}" />).</li>
	/// </ul>
	/// </summary>
	Any,

	/// <summary>
	/// The dialog may not be closed by light dismissal (such as clicking or
	/// tapping outside). It can still be closed by:
	/// <ul>
	/// <li>Platform-specific dismissal (such as the back button or escape key),</li>
	/// <li>By the page (via JavaScript or a <see cref="Form{TModel}" />).</li>
	/// </ul>
	/// </summary>
	CloseRequest,

	/// <summary>
	/// The dialog may only be closed by the page (via JavaScript or a <see cref="Form{TModel}" />).
	/// </summary>
	None,
}
