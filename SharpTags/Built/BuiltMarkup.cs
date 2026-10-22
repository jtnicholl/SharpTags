// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections;
using System.Diagnostics.Contracts;
using SharpTags.Nodes;

namespace SharpTags.Built;

/// <summary>
/// Provides static methods for building immutable, optimized markup generators.
/// </summary>
public static class BuiltMarkup
{
	/// <summary>
	/// Build an immutable and optimized markup generator from one or more
	/// provided <paramref name="nodes" />.
	/// </summary>
	/// <param name="nodes">The <see cref="MarkupNode{TModel}" />s to build from.</param>
	/// <typeparam name="TModel">
	/// The type of model, which can be used to vary rendered output.
	/// </typeparam>
	/// <returns>An immutable and optimized <see cref="BuiltMarkup{TModel}" />.</returns>
	public static BuiltMarkup<TModel> Build<TModel>(params ReadOnlySpan<MarkupNode<TModel>> nodes)
	{
		return Build(nodes, new(), new());
	}

	/// <summary>
	/// Build an immutable and optimized markup generator from one or more
	/// provided <paramref name="nodes" />.
	/// </summary>
	/// <param name="nodes">One or more <see cref="MarkupNode{TModel}" />s to build from.</param>
	/// <param name="captureContext">
	/// If <see langword="true" />, the render functions of the resulting
	/// <see cref="BuiltMarkup{TModel}" /> will attempt to capture the
	/// <see cref="SynchronizationContext" /> they are called from and invoke
	/// all model delegates on it.
	/// </param>
	/// <typeparam name="TModel">
	/// The type of model, which can be used to vary rendered output.
	/// </typeparam>
	/// <returns>An immutable and optimized <see cref="BuiltMarkup{TModel}" />.</returns>
	public static BuiltMarkup<TModel> Build<TModel>(
		ReadOnlySpan<MarkupNode<TModel>> nodes,
		bool captureContext
	)
	{
		return Build(nodes, new(), new(), captureContext);
	}

	/// <summary>
	/// Build an immutable and optimized markup generator from one or more
	/// provided <paramref name="nodes" />.
	/// </summary>
	/// <param name="nodes">One or more <see cref="MarkupNode{TModel}" />s to build from.</param>
	/// <param name="captureContext">
	/// If <see langword="true" /> (the default), the render functions of the
	/// resulting <see cref="BuiltMarkup{TModel}" /> will attempt to
	/// capture the <see cref="SynchronizationContext" /> or
	/// <see cref="TaskScheduler" /> they are called from and invoke all model
	/// delegates on it.
	/// </param>
	/// <typeparam name="TModel">
	/// The type of model, which can be used to vary rendered output.
	/// </typeparam>
	/// <returns>An immutable and optimized <see cref="BuiltMarkup{TModel}" />.</returns>
	public static BuiltMarkup<TModel> Build<TModel>(
		IEnumerable<MarkupNode<TModel>> nodes,
		bool captureContext = true
	)
	{
		ArgumentNullException.ThrowIfNull(nodes);
		return Build(nodes, new(), new(), captureContext);
	}

	internal static BuiltMarkup<TModel> Build<TModel>(
		ReadOnlySpan<MarkupNode<TModel>> nodes,
		Dictionary<object, IEnumerable> cache,
		HashSet<object> recursionCheck,
		bool capContext = true
	)
	{
		using var enumerator = nodes.GetEnumerator();
		return Build<TModel, ReadOnlySpan<MarkupNode<TModel>>.Enumerator>(
			enumerator,
			cache,
			recursionCheck,
			capContext
		);
	}

	internal static BuiltMarkup<TModel> Build<TModel>(
		IEnumerable<MarkupNode<TModel>> nodes,
		Dictionary<object, IEnumerable> cache,
		HashSet<object> recursionCheck,
		bool capContext = true
	)
	{
		using var enumerator = nodes.GetEnumerator();
		return Build<TModel, IEnumerator<MarkupNode<TModel>>>(
			enumerator,
			cache,
			recursionCheck,
			capContext
		);
	}

	private static BuiltMarkup<TModel> Build<TModel, TEnumerator>(
		TEnumerator nodes,
		Dictionary<object, IEnumerable> cache,
		HashSet<object> recursionCheck,
		bool capContext
	) where TEnumerator : IEnumerator<MarkupNode<TModel>>, allows ref struct
	{
		using PooledList<byte> textBuffer = new();
		using PooledList<IntermediateBuildResult<TModel>> outputParameters = new();
		int textStart = 0;

		while (nodes.MoveNext())
		{
			var node = nodes.Current ?? throw new ArgumentNullException(nameof(nodes));
			foreach (var result in node.Build(cache, recursionCheck, capContext))
			{
				if (result.Value is ReadOnlyMemory<byte> text)
				{
					textBuffer.AddRange(text.Span);
				}
				else
				{
					if (textStart != textBuffer.Count)
					{
						outputParameters.Add(new Range(textStart, textBuffer.Count));
						textStart = textBuffer.Count;
					}

					if (result.Value is IBuiltSegment<TModel> built)
					{
						outputParameters.Add(built);
					}
					else if (result.Value is SkipCondition<TModel> skipCondition)
					{
						outputParameters.Add(skipCondition);
					}
					else
					{
						Contract.Assert(result.Value is BuiltEnumerator<TModel>);
						outputParameters.Add((BuiltEnumerator<TModel>)result.Value);
					}
				}
			}
		}

		if (textStart != textBuffer.Count)
		{
			outputParameters.Add(new Range(textStart, textBuffer.Count));
		}

		var finalBuffer = textBuffer.ToArray();
		return new(outputParameters.Select(res => res.Build(finalBuffer)).ToArray());
	}
}
