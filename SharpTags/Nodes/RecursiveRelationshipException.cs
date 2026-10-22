// SPDX-License-Identifier: LGPL-3.0-or-later

namespace SharpTags.Nodes;

/// <summary>
/// Thrown when a <see cref="MarkupNode{TModel}" /> is parented to itself,
/// either directly or indirectly.
/// </summary>
public sealed class RecursiveRelationshipException : Exception
{
	/// <summary>
	/// Construct a <see cref="RecursiveRelationshipException" />.
	/// </summary>
	public RecursiveRelationshipException()
		: base("A node was added as a child to one of its own descendants.")
	{ }

	/// <summary>
	/// Construct a <see cref="RecursiveRelationshipException" />.
	/// </summary>
	/// <param name="name">The name of the node type with the reference loop.</param>
	public RecursiveRelationshipException(string name)
		: base($"A(n) {name} was added as a child to one of its own descendants.")
	{ }
}
