// SPDX-License-Identifier: LGPL-3.0-or-later

namespace SharpTags.SourceGenerator;

internal sealed class HtmlElementDefinition
{
	/// <summary>
	/// Full name of the element in Pascal-case.
	/// </summary>
	public required string Name { get; init; }

	/// <summary>
	/// A Pascal-case name that, when converted to lowercase, is the actual name
	/// used for this element in HTML.<br />
	/// If absent, <see cref="Name" /> is used instead.
	/// </summary>
	public string? Element { get; init; }

	/// <summary>
	/// Additional attributes for this element.
	/// </summary>
	public IReadOnlyList<HtmlAttributeDefinition>? Attributes { get; init; }

	/// <summary>
	/// If this element is a
	/// <see href="https://developer.mozilla.org/en-US/docs/Glossary/Void_element">
	/// void element</see>.
	/// </summary>
	public bool Void { get; init; }

	/// <summary>
	/// Provides a summary tag for the class XML doc comment.
	/// </summary>
	public string? Summary { get; init; }

	/// <summary>
	/// A list of other element names to include as see also links in the class XML doc comment.
	/// </summary>
	public IReadOnlyList<string>? SeeAlso { get; init; }
}
