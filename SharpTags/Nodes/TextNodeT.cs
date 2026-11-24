// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Buffers;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.IO.Pipelines;
using SharpTags.Built;

namespace SharpTags.Nodes;

/// <summary>
/// Renders plain text within an HTML or XML document.
/// </summary>
public sealed class TextNode<TModel> : MarkupNode<TModel>, IEquatable<TextNode<TModel>>
{
	/// <summary>
	/// The value of this <see cref="TextNode{TModel}" />.
	/// </summary>
	public TextValue<TModel> Text { get; }

	internal override bool RequiresModel => Text.Value is Delegate;

	/// <summary>
	/// Construct a new <see cref="TextNode{TModel}" />.
	/// </summary>
	/// <param name="text">The text to render.</param>
	public TextNode(TextValue<TModel> text)
	{
		this.Text = text;
	}

	/// <summary>
	/// Construct a new <see cref="TextNode{TModel}" />.
	/// </summary>
	/// <param name="textFunction">The text to render.</param>
	public TextNode(Func<TModel, string?> textFunction)
	{
		this.Text = textFunction;
	}

	/// <summary>
	/// Construct a new <see cref="TextNode{TModel}" />.
	/// </summary>
	/// <param name="textFunction">The text to render.</param>
	public TextNode(Func<TModel, PlainText> textFunction)
	{
		this.Text = textFunction;
	}

	internal override ValueTask RenderAsync(
		Stream utf8Stream,
		TModel model,
		CancellationToken cancel
	)
	{
		return utf8Stream.WriteAsync(Text.Evaluate(model).Utf8Bytes, cancel);
	}

	internal override async ValueTask RenderAsync(
		PipeWriter utf8Pipe,
		TModel model,
		CancellationToken cancel
	)
	{
		await utf8Pipe.WriteAsync(Text.Evaluate(model).Utf8Bytes, cancel).ConfigureAwait(false);
	}

	/// <inheritdoc />
	public override void Render(IBufferWriter<byte> utf8Buffer, TModel model)
	{
		ArgumentNullException.ThrowIfNull(utf8Buffer);
		var text = Text.Evaluate(model);
		utf8Buffer.Write(text.Utf8Bytes.Span);
	}

	private protected override IEnumerable<BuiltMarkupSegment<TModel>> BuildImplementation(
		Dictionary<object, IEnumerable> cache,
		HashSet<object> recursionCheck,
		bool capContext
	)
	{
		return Text switch
		{
			PlainText pt => [pt.Utf8Bytes],
			Func<TModel, PlainText> f => [new BuiltText<TModel>(f)],
			Func<TModel, string?> f => [new BuiltText<TModel>(f)],
			null => [],
		};
	}

	/// <inheritdoc />
	[Pure]
	public bool Equals([NotNullWhen(true)] TextNode<TModel>? other)
	{
		if (other is null)
		{
			return false;
		}

		if (ReferenceEquals(this, other))
		{
			return true;
		}

		return Text.Value?.Equals(other.Text.Value) ?? other.Text.Value is null;
	}

	/// <inheritdoc />
	[Pure]
	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		return obj is TextNode<TModel> other && Equals(other);
	}

	/// <inheritdoc />
	[Pure]
	public override int GetHashCode() => Text.Value?.GetHashCode() ?? 0;

	/// <summary>
	/// Check for equality.
	/// </summary>
	public static bool operator ==(TextNode<TModel>? left, TextNode<TModel>? right)
	{
		return left?.Equals(right) ?? right is null;
	}

	/// <summary>
	/// Check for inequality.
	/// </summary>
	public static bool operator !=(TextNode<TModel>? left, TextNode<TModel>? right)
	{
		return !(left == right);
	}

	/// <summary>
	/// Implicitly convert a <see cref="TextValue{TModel}" /> to a <see cref="TextNode{TModel}" />.
	/// </summary>
	public static implicit operator TextNode<TModel>?(TextValue<TModel> text)
	{
		return text.Value is null ? null : new(text);
	}

	/// <summary>
	/// Implicitly convert a <see cref="Func{T, TResult}" /> to a <see cref="TextNode{TModel}" />.
	/// </summary>
	[return: NotNullIfNotNull(nameof(text))]
	public static implicit operator TextNode<TModel>?(Func<TModel, string?>? text)
	{
		return text is null ? null : new(text);
	}

	/// <summary>
	/// Implicitly convert a <see cref="Func{T, TResult}" /> to a <see cref="TextNode{TModel}" />.
	/// </summary>
	[return: NotNullIfNotNull(nameof(text))]
	public static implicit operator TextNode<TModel>?(Func<TModel, PlainText>? text)
	{
		return text is null ? null : new(text);
	}

	/// <summary>
	/// Implicitly convert a <see cref="ReadOnlySpan{T}" /> to a <see cref="TextNode{TModel}" />.
	/// </summary>
	public static implicit operator TextNode<TModel>(ReadOnlySpan<byte> text)
	{
		return new(new PlainText(text));
	}

	/// <summary>
	/// Implicitly convert a <see cref="PlainText" /> to a <see cref="TextNode{TModel}" />.
	/// </summary>
	public static implicit operator TextNode<TModel>(PlainText text)
	{
		return new(text);
	}

	/// <summary>
	/// Implicitly convert a <see cref="string" /> to a <see cref="TextNode{TModel}" />.
	/// </summary>
	[return: NotNullIfNotNull(nameof(text))]
	public static implicit operator TextNode<TModel>?(string? text)
	{
		return text is null ? null : new(text);
	}
}
