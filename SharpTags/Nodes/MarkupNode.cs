// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.IO.Pipelines;

namespace SharpTags.Nodes;

/// <summary>
/// A node in an XML or HTML document, which can be an element or a text node.
/// </summary>
public abstract class MarkupNode
{
	// internal bool RequiresModel => false;

	/// <summary>
	/// Write UTF-8 text to a <see cref="Stream" />.
	/// </summary>
	/// <param name="utf8Stream">The stream to write text to.</param>
	/// <param name="cancel">A token used to cancel the operation.</param>
	public ValueTask Render(Stream utf8Stream, CancellationToken cancel = default)
	{
		ArgumentNullException.ThrowIfNull(utf8Stream);
		return RenderAsync(utf8Stream, cancel);
	}

	internal abstract ValueTask RenderAsync(Stream utf8Stream, CancellationToken cancel);

	/// <summary>
	/// Write UTF-8 text to a <see cref="PipeWriter" />.
	/// </summary>
	/// <param name="utf8Pipe">The pipe to write text to.</param>
	/// <param name="cancel">A token used to cancel the operation.</param>
	public ValueTask Render(PipeWriter utf8Pipe, CancellationToken cancel = default)
	{
		ArgumentNullException.ThrowIfNull(utf8Pipe);
		return RenderAsync(utf8Pipe, cancel);
	}

	internal abstract ValueTask RenderAsync(PipeWriter utf8Pipe, CancellationToken cancel);

	/// <summary>
	/// Write UTF-8 text to an <see cref="IBufferWriter{T}" />.
	/// </summary>
	/// <param name="utf8Buffer">The buffer to write text to.</param>
	public abstract void Render(IBufferWriter<byte> utf8Buffer);

	private protected MarkupNode() { }

	/// <summary>
	/// Convert this <see cref="MarkupNode" /> to a typed node.
	/// </summary>
	/// <typeparam name="TModel">The model type to convert to.</typeparam>
	/// <returns>A converted copy of this node.</returns>
	public abstract MarkupNode<TModel> AsTyped<TModel>();

	/// <summary>
	/// Implicitly convert a <see cref="ReadOnlySpan{T}" /> of UTF-8 text to a
	/// <see cref="TextNode" />.
	/// </summary>
	public static implicit operator MarkupNode(ReadOnlySpan<byte> text)
	{
		return new TextNode(new(text));
	}

	/// <summary>
	/// Implicitly convert a <see cref="PlainText" /> to a <see cref="TextNode" />.
	/// </summary>
	public static implicit operator MarkupNode(PlainText text)
	{
		return new TextNode(text);
	}

	/// <summary>
	/// Implicitly convert from a <see cref="string" /> to a <see cref="TextNode" />.
	/// </summary>
	[return: NotNullIfNotNull(nameof(text))]
	public static implicit operator MarkupNode?(string? text)
	{
		return text is null ? null : new TextNode(text);
	}

	internal virtual void RecursionCheck(HashSet<object> set) { }
}
