// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.IO.Pipelines;

namespace SharpTags.Nodes;

/// <summary>
/// Renders plain text within an HTML or XML document.
/// </summary>
public sealed class TextNode : MarkupNode, IEquatable<TextNode>
{
	/// <summary>
	/// The value of this <see cref="TextNode{TModel}" />.
	/// </summary>
	public PlainText Text { get; }

	/// <summary>
	/// Construct a new <see cref="TextNode" />.
	/// </summary>
	/// <param name="text">The text to render.</param>
	public TextNode(PlainText text)
	{
		this.Text = text;
	}

	/// <inheritdoc />
	internal override ValueTask RenderAsync(Stream utf8Stream, CancellationToken cancel)
	{
		return utf8Stream.WriteAsync(Text.Utf8Bytes, cancel);
	}

	internal override async ValueTask RenderAsync(PipeWriter utf8Pipe, CancellationToken cancel)
	{
		await utf8Pipe.WriteAsync(Text.Utf8Bytes, cancel).ConfigureAwait(false);
	}

	/// <inheritdoc />
	public override void Render(IBufferWriter<byte> utf8Buffer)
	{
		utf8Buffer.Write(Text.Utf8Bytes.Span);
	}

	/// <inheritdoc />
	public override TextNode<TModel> AsTyped<TModel>()
	{
		return new(Text);
	}

	/// <inheritdoc />
	[Pure]
	public bool Equals([NotNullWhen(true)] TextNode? other)
	{
		if (other is null)
		{
			return false;
		}
		if (ReferenceEquals(this, other))
		{
			return true;
		}

		return Text.Equals(other.Text);
	}

	/// <inheritdoc />
	[Pure]
	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		return obj is TextNode other && Equals(other);
	}

	/// <inheritdoc />
	[Pure]
	public override int GetHashCode() => Text.GetHashCode();

	/// <summary>
	/// Check for equality.
	/// </summary>
	public static bool operator ==(TextNode? left, TextNode? right)
	{
		return left?.Equals(right) ?? right is null;
	}

	/// <summary>
	/// Check for inequality.
	/// </summary>
	public static bool operator !=(TextNode? left, TextNode? right)
	{
		return !(left == right);
	}

	/// <summary>
	/// Implicitly convert a <see cref="TextValue{TModel}" /> to a <see cref="TextNode{TModel}" />.
	/// </summary>
	public static implicit operator TextNode(PlainText text)
	{
		return new(text);
	}

	/// <summary>
	/// Implicitly convert a <see cref="ReadOnlySpan{T}" /> to a <see cref="TextNode{TModel}" />.
	/// </summary>
	public static implicit operator TextNode(ReadOnlySpan<byte> text)
	{
		return new(new(text));
	}

	/// <summary>
	/// Implicitly convert a <see cref="string" /> to a <see cref="TextNode{TModel}" />.
	/// </summary>
	[return: NotNullIfNotNull(nameof(text))]
	public static implicit operator TextNode?(string? text)
	{
		return text is null ? null : new(text);
	}
}
