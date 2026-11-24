// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Buffers;
using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.IO.Pipelines;
using SharpTags.Built;

namespace SharpTags.Nodes;

/// <summary>
/// A chain of "if" + "else if" conditions, with a final "else" fallback.
/// </summary>
/// <typeparam name="TModel">
/// The model type used for the predicates and by the rendered nodes.
/// </typeparam>
public sealed class TernaryNode<TModel> : MarkupNode<TModel>
{
	internal override bool RequiresModel => true;

	private readonly ImmutableArray<Func<TModel, bool>> conditions;
	private readonly ImmutableArray<ImmutableArray<MarkupNode<TModel>>> children;
	private readonly ImmutableArray<MarkupNode<TModel>> fallback;

	internal TernaryNode(
		ImmutableArray<Func<TModel, bool>> conditions,
		ImmutableArray<ImmutableArray<MarkupNode<TModel>>> children,
		ImmutableArray<MarkupNode<TModel>> fallback
	)
	{
		Contract.Assert(conditions.Length == children.Length);
		Contract.Assert(conditions.Length > 0);
		this.conditions = conditions;
		this.children = children;
		this.fallback = fallback;
	}

	internal override async ValueTask RenderAsync(
		Stream utf8Stream,
		TModel model,
		CancellationToken cancel
	)
	{
		foreach (var node in Evaluate(model))
		{
			await node.Render(utf8Stream, model, cancel).ConfigureAwait(false);
		}
	}

	internal override async ValueTask RenderAsync(
		PipeWriter utf8Pipe,
		TModel model,
		CancellationToken cancel
	)
	{
		foreach (var node in Evaluate(model))
		{
			await node.Render(utf8Pipe, model, cancel).ConfigureAwait(false);
		}
	}

	/// <inheritdoc />
	public override void Render(IBufferWriter<byte> utf8Buffer, TModel model)
	{
		ArgumentNullException.ThrowIfNull(utf8Buffer);
		foreach (var child in Evaluate(model))
		{
			child.Render(utf8Buffer, model);
		}
	}

	private protected override IEnumerable<BuiltMarkupSegment<TModel>> BuildImplementation(
		Dictionary<object, IEnumerable> cache,
		HashSet<object> recursionCheck,
		bool capContext
	)
	{
		using var buffer = new PooledList<BuiltMarkupSegment<TModel>>();

		for (int i = this.fallback.Length - 1; i >= 0; i--)
		{
			buffer.AddRangeReversed(this.fallback[i].Build(cache, recursionCheck, capContext));
		}

		for (int i = this.conditions.Length - 1; i >= 0; i--)
		{
			var startCount = buffer.Count;
			if (startCount != 0)
			{
				buffer.Add(new SkipCondition<TModel>(default, 0, startCount));
			}

			for (int j = this.children[i].Length - 1; j >= 0; j--)
			{
				buffer.AddRangeReversed(
					this.children[i][j].Build(cache, recursionCheck, capContext)
				);
			}

			var skip = buffer.Count - startCount;
			if (skip != 0)
			{
				buffer.Add(new SkipCondition<TModel>(this.conditions[i], 0, skip));
			}
		}

		for (int i = buffer.Count - 1; i >= 0; i--)
		{
			yield return buffer[i];
		}
	}

	private ImmutableArray<MarkupNode<TModel>> Evaluate(TModel model)
	{
		for (int i = 0; i < this.conditions.Length; i++)
		{
			if (this.conditions[i].Invoke(model))
			{
				return this.children[i];
			}
		}

		return this.fallback;
	}

	internal override void RecursionCheck(HashSet<object> set)
	{
		if (!set.Add(this))
		{
			throw new RecursiveRelationshipException(nameof(TernaryNode<>));
		}
		foreach (var node in this.children.SelectMany(c => c).Concat(this.fallback))
		{
			node.RecursionCheck(set);
		}
		set.Remove(this);
	}
}
