// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Buffers;
using System.IO.Pipelines;
using System.Text;

namespace SharpTags.Nodes;

/// <summary>
/// An element in an HTML or XML document.
/// </summary>
/// <typeparam name="TModel">
/// The type of model, which can be used to vary rendered output.
/// </typeparam>
public closed class MarkupElement<TModel> : MarkupNode<TModel>
{
	internal override bool RequiresModel => Children.Any(c => c.RequiresModel);

	/// <summary>
	/// How this node renders when it has no children.
	/// </summary>
	public bool SelfClosing { get; }

	/// <summary>
	/// Child nodes of the element.
	/// </summary>
	public IList<MarkupNode<TModel>> Children { get; }

	private protected abstract IEnumerable<ITextRender<TModel>> Attributes { get; }

	private protected readonly ReadOnlyMemory<byte> name;

	private protected MarkupElement(
		ReadOnlyMemory<byte> name,
		bool selfClosing,
		IList<MarkupNode<TModel>> children
	)
	{
		this.name = name;
		this.SelfClosing = selfClosing;
		this.Children = children;
	}

	/// <inheritdoc />
	public override ValueTask Render(
		Stream utf8Stream,
		TModel model,
		CancellationToken cancel = default
	)
	{
		ArgumentNullException.ThrowIfNull(utf8Stream);
		return RenderAsync(utf8Stream, model, cancel);
	}

	private async ValueTask RenderAsync(Stream utf8Stream, TModel model, CancellationToken cancel)
	{
		await utf8Stream.WriteAsync(MarkupElement.LessThan, cancel).ConfigureAwait(false);
		await utf8Stream.WriteAsync(this.name, cancel).ConfigureAwait(false);
		foreach (var attribute in Attributes)
		{
			await attribute.Render(utf8Stream, model, cancel).ConfigureAwait(false);
		}

		if (!SelfClosing || this.Children.Count != 0)
		{
			await utf8Stream.WriteAsync(MarkupElement.GreaterThan, cancel).ConfigureAwait(false);

			foreach (var child in this.Children)
			{
				await child.Render(utf8Stream, model, cancel).ConfigureAwait(false);
			}

			await utf8Stream.WriteAsync(MarkupElement.LessThan, cancel).ConfigureAwait(false);
			await utf8Stream.WriteAsync(MarkupElement.Slash, cancel).ConfigureAwait(false);
			await utf8Stream.WriteAsync(this.name, cancel).ConfigureAwait(false);
			await utf8Stream.WriteAsync(MarkupElement.GreaterThan, cancel).ConfigureAwait(false);
		}
		else
		{
			await utf8Stream.WriteAsync(MarkupElement.SelfClose, cancel).ConfigureAwait(false);
		}
	}

	/// <inheritdoc />
	public override ValueTask Render(
		PipeWriter utf8Pipe,
		TModel model,
		CancellationToken cancel = default
	)
	{
		ArgumentNullException.ThrowIfNull(utf8Pipe);
		return RenderAsync(utf8Pipe, model, cancel);
	}

	private async ValueTask RenderAsync(PipeWriter utf8Pipe, TModel model, CancellationToken cancel)
	{
		await utf8Pipe.WriteAsync(MarkupElement.LessThan, cancel).ConfigureAwait(false);
		await utf8Pipe.WriteAsync(this.name, cancel).ConfigureAwait(false);
		foreach (var attribute in Attributes)
		{
			await attribute.Render(utf8Pipe, model, cancel).ConfigureAwait(false);
		}

		if (!SelfClosing || this.Children.Count != 0)
		{
			await utf8Pipe.WriteAsync(MarkupElement.GreaterThan, cancel).ConfigureAwait(false);

			foreach (var child in this.Children)
			{
				await child.Render(utf8Pipe, model, cancel).ConfigureAwait(false);
			}

			await utf8Pipe.WriteAsync(MarkupElement.LessThan, cancel).ConfigureAwait(false);
			await utf8Pipe.WriteAsync(MarkupElement.Slash, cancel).ConfigureAwait(false);
			await utf8Pipe.WriteAsync(this.name, cancel).ConfigureAwait(false);
			await utf8Pipe.WriteAsync(MarkupElement.GreaterThan, cancel).ConfigureAwait(false);
		}
		else
		{
			await utf8Pipe.WriteAsync(MarkupElement.SelfClose, cancel).ConfigureAwait(false);
		}
	}

	/// <inheritdoc />
	public override void Render(IBufferWriter<byte> utf8Buffer, TModel model)
	{
		ArgumentNullException.ThrowIfNull(utf8Buffer);
		utf8Buffer.Write(MarkupElement.LessThan.Span);
		utf8Buffer.Write(this.name.Span);
		foreach (var attribute in Attributes)
		{
			attribute.Render(utf8Buffer, model);
		}

		if (!SelfClosing || this.Children.Count != 0)
		{
			utf8Buffer.Write(MarkupElement.GreaterThan.Span);

			foreach (var child in this.Children)
			{
				child.Render(utf8Buffer, model);
			}

			utf8Buffer.Write(MarkupElement.LessThan.Span);
			utf8Buffer.Write(MarkupElement.Slash.Span);
			utf8Buffer.Write(this.name.Span);
			utf8Buffer.Write(MarkupElement.GreaterThan.Span);
		}
		else
		{
			utf8Buffer.Write(MarkupElement.SelfClose.Span);
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
