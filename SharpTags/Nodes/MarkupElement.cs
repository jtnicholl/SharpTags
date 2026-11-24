// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Buffers;
using System.IO.Pipelines;
using System.Text;

namespace SharpTags.Nodes;

/// <summary>
/// An element in an HTML or XML document.
/// </summary>
public closed class MarkupElement : MarkupNode
{
	private static readonly ReadOnlyMemory<byte> chars = "< />=\""u8.ToArray();
	internal static ReadOnlyMemory<byte> LessThan => chars[..1];
	internal static ReadOnlyMemory<byte> GreaterThan => chars[3..4];
	internal static ReadOnlyMemory<byte> Slash => chars[2..3];
	internal static ReadOnlyMemory<byte> SelfClose => chars[2..4];

	/// <summary>
	/// How this node renders when it has no children.
	/// </summary>
	public bool SelfClosing { get; }

	/// <summary>
	/// Child nodes of the element.
	/// </summary>
	public IList<MarkupNode> Children { get; }

	private protected abstract IEnumerable<ReadOnlyMemory<byte>> Attributes { get; }

	private protected readonly ReadOnlyMemory<byte> name;

	/// <inheritdoc />
	public abstract override MarkupElement<TModel> AsTyped<TModel>();

	private protected MarkupElement(
		ReadOnlyMemory<byte> name,
		bool selfClosing,
		IList<MarkupNode> children
	)
	{
		this.name = name;
		this.SelfClosing = selfClosing;
		this.Children = children;
	}

	internal override async ValueTask RenderAsync(Stream utf8Stream, CancellationToken cancel)
	{
		await utf8Stream.WriteAsync(LessThan, cancel).ConfigureAwait(false);
		await utf8Stream.WriteAsync(this.name, cancel).ConfigureAwait(false);
		foreach (var attribute in Attributes)
		{
			await utf8Stream.WriteAsync(attribute, cancel).ConfigureAwait(false);
		}

		if (!SelfClosing || this.Children.Count != 0)
		{
			await utf8Stream.WriteAsync(GreaterThan, cancel).ConfigureAwait(false);

			foreach (var child in this.Children)
			{
				await child.Render(utf8Stream, cancel).ConfigureAwait(false);
			}

			await utf8Stream.WriteAsync(LessThan, cancel).ConfigureAwait(false);
			await utf8Stream.WriteAsync(Slash, cancel).ConfigureAwait(false);
			await utf8Stream.WriteAsync(this.name, cancel).ConfigureAwait(false);
			await utf8Stream.WriteAsync(GreaterThan, cancel).ConfigureAwait(false);
		}
		else
		{
			await utf8Stream.WriteAsync(SelfClose, cancel).ConfigureAwait(false);
		}
	}

	internal override async ValueTask RenderAsync(PipeWriter utf8Pipe, CancellationToken cancel)
	{
		await utf8Pipe.WriteAsync(LessThan, cancel).ConfigureAwait(false);
		await utf8Pipe.WriteAsync(this.name, cancel).ConfigureAwait(false);
		foreach (var attribute in Attributes)
		{
			await utf8Pipe.WriteAsync(attribute, cancel).ConfigureAwait(false);
		}

		if (!SelfClosing || this.Children.Count != 0)
		{
			await utf8Pipe.WriteAsync(GreaterThan, cancel).ConfigureAwait(false);

			foreach (var child in this.Children)
			{
				await child.Render(utf8Pipe, cancel).ConfigureAwait(false);
			}

			await utf8Pipe.WriteAsync(LessThan, cancel).ConfigureAwait(false);
			await utf8Pipe.WriteAsync(Slash, cancel).ConfigureAwait(false);
			await utf8Pipe.WriteAsync(this.name, cancel).ConfigureAwait(false);
			await utf8Pipe.WriteAsync(GreaterThan, cancel).ConfigureAwait(false);
		}
		else
		{
			await utf8Pipe.WriteAsync(SelfClose, cancel).ConfigureAwait(false);
		}
	}

	/// <inheritdoc />
	public override void Render(IBufferWriter<byte> utf8Buffer)
	{
		ArgumentNullException.ThrowIfNull(utf8Buffer);
		utf8Buffer.Write(LessThan.Span);
		utf8Buffer.Write(this.name.Span);
		foreach (var attribute in Attributes)
		{
			utf8Buffer.Write(attribute.Span);
		}

		if (!SelfClosing || this.Children.Count != 0)
		{
			utf8Buffer.Write(GreaterThan.Span);

			foreach (var child in this.Children)
			{
				child.Render(utf8Buffer);
			}

			utf8Buffer.Write(LessThan.Span);
			utf8Buffer.Write(Slash.Span);
			utf8Buffer.Write(this.name.Span);
			utf8Buffer.Write(GreaterThan.Span);
		}
		else
		{
			utf8Buffer.Write(SelfClose.Span);
		}
	}

	internal override void RecursionCheck(HashSet<object> set)
	{
		if (!set.Add(this))
		{
			throw new RecursiveRelationshipException(Encoding.UTF8.GetString(this.name.Span));
		}
		foreach (var child in this.Children)
		{
			child.RecursionCheck(set);
		}
		set.Remove(this);
	}
}
