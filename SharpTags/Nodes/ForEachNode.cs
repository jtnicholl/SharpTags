// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Buffers;
using System.Collections;
using System.IO.Pipelines;
using SharpTags.Built;

namespace SharpTags.Nodes;

/// <summary>
/// Node which renders a variable number of times.
/// </summary>
/// <typeparam name="TModel">
/// The type of model which provides the iterator.
/// </typeparam>
/// <typeparam name="TIterated">
/// The iterated type and the model type for the rendered nodes.
/// </typeparam>
public sealed class ForEachNode<TModel, TIterated> : MarkupNode<TModel>
{
	internal override bool RequiresModel => this.children.Any();

	private readonly ModelEnumerator<TModel, TIterated> iterator;
	private readonly IEnumerable<MarkupNode<TIterated>> children;

	internal ForEachNode(
		ModelEnumerator<TModel, TIterated> iterator,
		IEnumerable<MarkupNode<TIterated>> children
	)
	{
		this.iterator = iterator;
		this.children = children;
	}

	/// <inheritdoc />
	public override void Render(IBufferWriter<byte> utf8Buffer, TModel model)
	{
		ArgumentNullException.ThrowIfNull(utf8Buffer);
		foreach (var item in this.iterator.Invoke(model))
		{
			foreach (var child in this.children)
			{
				child.Render(utf8Buffer, item);
			}
		}
	}

	private protected override IEnumerable<BuiltMarkupSegment<TModel>> BuildImplementation(
		Dictionary<object, IEnumerable> cache,
		HashSet<object> recursionCheck,
		bool capContext
	)
	{
		return this.children.Any()
			? [new BuiltEnumerator<TModel, TIterated>(
				this.iterator,
				BuiltMarkup.Build(this.children, cache, recursionCheck)
			)]
			: [];
	}

	internal override async ValueTask RenderAsync(
		Stream utf8Stream,
		TModel model,
		CancellationToken cancel
	)
	{
		foreach (var item in this.iterator.Invoke(model))
		{
			foreach (var child in this.children)
			{
				await child.Render(utf8Stream, item, cancel).ConfigureAwait(false);
			}
		}
	}

	internal override async ValueTask RenderAsync(
		PipeWriter utf8Pipe,
		TModel model,
		CancellationToken cancel
	)
	{
		foreach (var item in this.iterator.Invoke(model))
		{
			foreach (var child in this.children)
			{
				await child.Render(utf8Pipe, item, cancel).ConfigureAwait(false);
			}
		}
	}

	internal override void RecursionCheck(HashSet<object> set)
	{
		if (!set.Add(this))
		{
			throw new RecursiveRelationshipException(nameof(ConditionalNode<>));
		}
		foreach (var node in this.children)
		{
			node.RecursionCheck(set);
		}
		set.Remove(this);
	}
}
