// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Buffers;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.IO.Pipelines;
using SharpTags.Built;

namespace SharpTags.Nodes;

/// <summary>
/// A node in an XML or HTML document, which can be an element or a text node.
/// </summary>
/// <typeparam name="TModel">
/// The type of model, which can be used to vary rendered output.
/// </typeparam>
public abstract class MarkupNode<TModel> :
	IStreamTextRender<TModel>,
	IPipeWriterTextRender<TModel>,
	IBufferWriterTextRender<TModel>
{
	internal abstract bool RequiresModel { get; }

	/// <inheritdoc />
	public ValueTask Render(Stream utf8Stream, TModel model, CancellationToken cancel = default)
	{
		ArgumentNullException.ThrowIfNull(utf8Stream);
		return RenderAsync(utf8Stream, model, cancel);
	}

	internal abstract ValueTask RenderAsync(
		Stream utf8Stream,
		TModel model,
		CancellationToken cancel
	);

	/// <inheritdoc />
	public ValueTask Render(PipeWriter utf8Pipe, TModel model, CancellationToken cancel = default)
	{
		ArgumentNullException.ThrowIfNull(utf8Pipe);
		return RenderAsync(utf8Pipe, model, cancel);
	}

	internal abstract ValueTask RenderAsync(
		PipeWriter utf8Pipe,
		TModel model,
		CancellationToken cancel
	);

	/// <inheritdoc />
	public abstract void Render(IBufferWriter<byte> utf8Buffer, TModel model);

	private protected MarkupNode() { }

	/// <summary>
	/// Implicitly convert a <see cref="TextValue{TModel}" /> to a <see cref="TextNode{TModel}" />.
	/// </summary>
	public static implicit operator MarkupNode<TModel>?(TextValue<TModel> text)
	{
		return text is null ? null : new TextNode<TModel>(text);
	}

	/// <summary>
	/// Implicitly convert a <see cref="Func{T, TResult}" /> to a <see cref="TextNode{TModel}" />.
	/// </summary>
	[return: NotNullIfNotNull(nameof(text))]
	public static implicit operator MarkupNode<TModel>?(Func<TModel, string?>? text)
	{
		return text is null ? null : new TextNode<TModel>(text);
	}

	/// <summary>
	/// Implicitly convert a <see cref="Func{T, TResult}" /> to a <see cref="TextNode{TModel}" />.
	/// </summary>
	[return: NotNullIfNotNull(nameof(text))]
	public static implicit operator MarkupNode<TModel>?(Func<TModel, PlainText>? text)
	{
		return text is null ? null : new TextNode<TModel>(text);
	}

	/// <summary>
	/// Implicitly convert a <see cref="ReadOnlySpan{T}" /> of UTF-8 text to a
	/// <see cref="TextNode{TModel}" />.
	/// </summary>
	public static implicit operator MarkupNode<TModel>(ReadOnlySpan<byte> text)
	{
		return new TextNode<TModel>(new PlainText(text));
	}

	/// <summary>
	/// Implicitly convert a <see cref="PlainText" /> to a <see cref="TextNode{TModel}" />.
	/// </summary>
	public static implicit operator MarkupNode<TModel>(PlainText text)
	{
		return new TextNode<TModel>(text);
	}

	/// <summary>
	/// Implicitly convert from a <see cref="string" /> to a <see cref="TextNode{TModel}" />.
	/// </summary>
	[return: NotNullIfNotNull(nameof(text))]
	public static implicit operator MarkupNode<TModel>?(string? text)
	{
		return text is null ? null : new TextNode<TModel>(text);
	}

	[SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
	internal IEnumerable<BuiltMarkupSegment<TModel>> Build(
		Dictionary<object, IEnumerable> cache,
		HashSet<object> recursionCheck,
		bool capContext
	)
	{
		RecursionCheck(recursionCheck);
		if (!cache.TryGetValue(this, out var result))
		{
			result = BuildImplementation(cache, recursionCheck, capContext);
			cache[this] = result;
		}

		return result.Cast<BuiltMarkupSegment<TModel>>();
	}

	private protected abstract IEnumerable<BuiltMarkupSegment<TModel>> BuildImplementation(
		Dictionary<object, IEnumerable> cache,
		HashSet<object> recursionCheck,
		bool capContext
	);

	internal virtual void RecursionCheck(HashSet<object> set) { }
}
