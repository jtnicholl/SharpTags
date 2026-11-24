// SPDX-License-Identifier: LGPL-3.0-or-later

namespace SharpTags.Nodes.Html.Elements;

/// <summary>
/// Base class for HTML elements that are not model-dependent.
/// </summary>
public closed partial class HtmlElement : MarkupElement
{
	/// <summary>
	/// If this element does not allow children.
	/// </summary>
	/// <seealso href="https://developer.mozilla.org/en-US/docs/Glossary/Void_element" />
	public bool IsVoid => SelfClosing;

	private protected HtmlElement(
		ReadOnlyMemory<byte> name,
		bool isVoid,
		IList<MarkupNode> children
	) : base(name, isVoid, children) { }
}
