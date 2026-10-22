// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using SharpTags.Built;

namespace SharpTags.Nodes;

/// <summary>
/// Extension methods for rendering flow control.
/// </summary>
public static class NodeExtensions
{
	/// <summary>
	/// Create a <see cref="ForEachNode{TModel, TIterated}" /> from the given
	/// <paramref name="node" /> and <paramref name="iterator" />.
	/// </summary>
	/// <param name="node">The <see cref="MarkupNode{TModel}" /> to repeat.</param>
	/// <param name="iterator">The iterator function.</param>
	/// <typeparam name="TModel">The type of model providing the enumerable.</typeparam>
	/// <typeparam name="TIterated">The type being iterated.</typeparam>
	/// <returns>The <see cref="ForEachNode{TModel, TIterated}" />.</returns>
	[Pure]
	public static ForEachNode<TModel, TIterated> ForEach<TModel, TIterated>(
		this MarkupNode<TIterated> node,
		ModelEnumerator<TModel, TIterated> iterator
	)
	{
		ArgumentNullException.ThrowIfNull(node);
		ArgumentNullException.ThrowIfNull(iterator);
		return new(iterator, [node]);
	}

	/// <summary>
	/// Create a <see cref="ConditionalNode{TModel}" /> from the given
	/// <paramref name="node" /> and <paramref name="condition" />.
	/// </summary>
	/// <param name="node">The <see cref="MarkupNode{TModel}" /> to render conditionally.</param>
	/// <param name="condition">The visibility function.</param>
	/// <typeparam name="TModel">The type of model.</typeparam>
	/// <returns>The <see cref="ConditionalNode{TModel}" />.</returns>
	[Pure]
	public static ConditionalNode<TModel> If<TModel>(
		this MarkupNode<TModel> node,
		Func<TModel, bool> condition
	)
	{
		ArgumentNullException.ThrowIfNull(node);
		ArgumentNullException.ThrowIfNull(condition);
		return new([condition], [[node]]);
	}

	/// <summary>
	/// Create an extended <see cref="ConditionalNode{TModel}" /> from the given
	/// <paramref name="node" /> and <paramref name="condition" />.
	/// </summary>
	/// <param name="node">
	/// The existing <see cref="ConditionalNode{TModel}" /> to add a new condition to.
	/// </param>
	/// <param name="condition">The condition for the new nodes.</param>
	/// <param name="nodes">
	/// The nodes to render if all previous conditions fail, but the new condition succeeds.
	/// </param>
	/// <typeparam name="TModel">The type of model.</typeparam>
	/// <returns>The new <see cref="ConditionalNode{TModel}" />.</returns>
	[Pure]
	public static ConditionalNode<TModel> ElseIf<TModel>(
		this ConditionalNode<TModel> node,
		Func<TModel, bool> condition,
		params ReadOnlySpan<MarkupNode<TModel>> nodes
	)
	{
		ArgumentNullException.ThrowIfNull(node);
		ArgumentNullException.ThrowIfNull(condition);
		ArgumentNullException.ThrowIfContainsNull(nodes);
		return new(node.conditions.Add(condition), node.children.Add(nodes.ToImmutableArray()));
	}

	/// <summary>
	/// Create an extended <see cref="ConditionalNode{TModel}" /> from the given
	/// <paramref name="node" /> and <paramref name="condition" />.
	/// </summary>
	/// <param name="node">
	/// The existing <see cref="ConditionalNode{TModel}" /> to add a new condition to.
	/// </param>
	/// <param name="condition">The condition for the new nodes.</param>
	/// <param name="nodes">
	/// The nodes to render if all previous conditions fail, but the new condition succeeds.
	/// </param>
	/// <typeparam name="TModel">The type of model.</typeparam>
	/// <returns>The new <see cref="ConditionalNode{TModel}" />.</returns>
	[Pure]
	public static ConditionalNode<TModel> ElseIf<TModel>(
		this ConditionalNode<TModel> node,
		Func<TModel, bool> condition,
		IEnumerable<MarkupNode<TModel>> nodes
	)
	{
		ArgumentNullException.ThrowIfNull(node);
		ArgumentNullException.ThrowIfNull(condition);
		ArgumentNullException.ThrowIfNull(nodes);
		var array = nodes.ToImmutableArray();
		ArgumentNullException.ThrowIfContainsNull(array, nameof(nodes));
		return new(node.conditions.Add(condition), node.children.Add(array));
	}

	/// <summary>
	/// Create a <see cref="TernaryNode{TModel}" /> from the given
	/// <paramref name="node" /> and <paramref name="nodes" />.
	/// </summary>
	/// <param name="node">
	/// The existing <see cref="ConditionalNode{TModel}" /> to add a final fallback to.
	/// </param>
	/// <param name="nodes">
	/// The nodes to render if all conditions fail.
	/// </param>
	/// <typeparam name="TModel">The type of model.</typeparam>
	/// <returns>The new <see cref="TernaryNode{TModel}" />.</returns>
	[Pure]
	public static TernaryNode<TModel> Else<TModel>(
		this ConditionalNode<TModel> node,
		params ReadOnlySpan<MarkupNode<TModel>> nodes
	)
	{
		ArgumentNullException.ThrowIfNull(node);
		ArgumentNullException.ThrowIfContainsNull(nodes);
		return new(node.conditions, node.children, nodes.ToImmutableArray());
	}

	/// <summary>
	/// Create a <see cref="TernaryNode{TModel}" /> from the given
	/// <paramref name="node" /> and <paramref name="nodes" />.
	/// </summary>
	/// <param name="node">
	/// The existing <see cref="ConditionalNode{TModel}" /> to add a final fallback to.
	/// </param>
	/// <param name="nodes">
	/// The nodes to render if all conditions fail.
	/// </param>
	/// <typeparam name="TModel">The type of model.</typeparam>
	/// <returns>The new <see cref="TernaryNode{TModel}" />.</returns>
	[Pure]
	public static TernaryNode<TModel> Else<TModel>(
		this ConditionalNode<TModel> node,
		IEnumerable<MarkupNode<TModel>> nodes
	)
	{
		ArgumentNullException.ThrowIfNull(node);
		ArgumentNullException.ThrowIfNull(nodes);
		var array = nodes.ToImmutableArray();
		ArgumentNullException.ThrowIfContainsNull(array, nameof(nodes));
		return new(node.conditions, node.children, array);
	}

	/// <summary>
	/// Build an immutable and optimized markup generator from the provided
	/// <paramref name="node" />.
	/// </summary>
	/// <param name="node">The <see cref="MarkupNode{TModel}" /> to build from.</param>
	/// <typeparam name="TModel">
	/// The type of model, which can be used to vary rendered output.
	/// </typeparam>
	/// <returns>An immutable and optimized <see cref="BuiltMarkup{TModel}" />.</returns>
	/// <seealso cref="BuiltMarkup.Build{TModel}(ReadOnlySpan{MarkupNode{TModel}})"/>
	[Pure]
	public static BuiltMarkup<TModel> Build<TModel>(this MarkupNode<TModel> node)
	{
		return BuiltMarkup.Build(node);
	}
}
