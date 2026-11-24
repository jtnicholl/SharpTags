// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Buffers;
using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.IO.Pipelines;
using SharpTags.Built;

namespace SharpTags.Nodes;

/// <summary>
/// A node which renders conditionally.
/// </summary>
public sealed class ConditionalNode<TModel> : MarkupNode<TModel>
{
	internal override bool RequiresModel => true;

	internal readonly ImmutableArray<Func<TModel, bool>> conditions;
	internal readonly ImmutableArray<ImmutableArray<MarkupNode<TModel>>> children;

	internal ConditionalNode(
		ImmutableArray<Func<TModel, bool>> conditions,
		ImmutableArray<ImmutableArray<MarkupNode<TModel>>> children
	)
	{
		Contract.Assert(conditions.Length == children.Length);
		this.conditions = conditions;
		this.children = children;
	}

	/// <inheritdoc />
	internal override async ValueTask RenderAsync(
		Stream utf8Stream,
		TModel model,
		CancellationToken cancel
	)
	{
		for (int i = 0; i < this.conditions.Length; i++)
		{
			var evaluation = this.conditions[i].Invoke(model);

			if (evaluation)
			{
				foreach (var child in this.children[i])
				{
					await child.RenderAsync(utf8Stream, model, cancel).ConfigureAwait(false);
				}
				break;
			}
		}
	}

	internal override async ValueTask RenderAsync(
		PipeWriter utf8Pipe,
		TModel model,
		CancellationToken cancel
	)
	{
		for (int i = 0; i < this.conditions.Length; i++)
		{
			var evaluation = this.conditions[i].Invoke(model);

			if (evaluation)
			{
				foreach (var child in this.children[i])
				{
					await child.RenderAsync(utf8Pipe, model, cancel).ConfigureAwait(false);
				}
				break;
			}
		}
	}

	/// <inheritdoc />
	public override void Render(IBufferWriter<byte> utf8Buffer, TModel model)
	{
		ArgumentNullException.ThrowIfNull(utf8Buffer);
		for (int i = 0; i < this.conditions.Length; i++)
		{
			var evaluation = this.conditions[i].Invoke(model);

			if (evaluation)
			{
				foreach (var child in this.children[i])
				{
					child.Render(utf8Buffer, model);
				}
				break;
			}
		}
	}

	private protected override IEnumerable<BuiltMarkupSegment<TModel>> BuildImplementation(
		Dictionary<object, IEnumerable> cache,
		HashSet<object> recursionCheck,
		bool capContext
	)
	{
		using var buffer = new PooledList<BuiltMarkupSegment<TModel>>();

		for (int i = this.conditions.Length - 1; i >= 0; i--)
		{
			var startCount = buffer.Count;
			if (startCount != 0)
			{
				buffer.Add(new SkipCondition<TModel>(null, 0, startCount));
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

	internal override void RecursionCheck(HashSet<object> set)
	{
		if (!set.Add(this))
		{
			throw new RecursiveRelationshipException(nameof(ConditionalNode<>));
		}
		foreach (var node in this.children.SelectMany(c => c))
		{
			node.RecursionCheck(set);
		}
		set.Remove(this);
	}
}
