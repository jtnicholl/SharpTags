// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Buffers;
using System.Diagnostics.Contracts;
using System.IO.Pipelines;

namespace SharpTags.Built;

/// <summary>
/// Optimized markup nodes where text has been pre-evaluated, converted to
/// UTF-8, and memory-compacted where possible.
/// </summary>
/// <typeparam name="TModel">
/// The type of model, which can be used to vary rendered output.
/// </typeparam>
public sealed class BuiltMarkup<TModel> :
	IStreamTextRender<TModel>,
	IBufferWriterTextRender<TModel>,
	IPipeWriterTextRender<TModel>
{
	internal readonly BuiltMarkupSegment<TModel>[] segments;

	internal BuiltMarkup(BuiltMarkupSegment<TModel>[] segments)
	{
		this.segments = segments;
	}

	/// <inheritdoc />
	public ValueTask Render(Stream utf8Stream, TModel model, CancellationToken cancel = default)
	{
		ArgumentNullException.ThrowIfNull(utf8Stream);
		return RenderAsync(utf8Stream, model, cancel);
	}

	internal async ValueTask RenderAsync(Stream utf8Stream, TModel model, CancellationToken cancel)
	{
		using var outputText = new PooledList<ReadOnlyMemory<byte>>(this.segments.Length * 2);

		Evaluate(model, outputText);

		foreach (var text in outputText)
		{
			await utf8Stream.WriteAsync(text, cancel).ConfigureAwait(false);
		}
	}

	/// <inheritdoc />
	public void Render(IBufferWriter<byte> utf8Buffer, TModel model)
	{
		ArgumentNullException.ThrowIfNull(utf8Buffer);

		using var outputText = new PooledList<ReadOnlyMemory<byte>>(this.segments.Length * 2);

		Evaluate(model, outputText);

		foreach (var text in outputText)
		{
			utf8Buffer.Write(text.Span);
		}
	}

	/// <inheritdoc />
	public ValueTask Render(PipeWriter utf8Pipe, TModel model, CancellationToken cancel = default)
	{
		ArgumentNullException.ThrowIfNull(utf8Pipe);
		return RenderAsync(utf8Pipe, model, cancel);
	}

	internal async ValueTask RenderAsync(
		PipeWriter utf8Pipe,
		TModel model,
		CancellationToken cancel
	)
	{
		using var outputText = new PooledList<ReadOnlyMemory<byte>>(this.segments.Length * 2);

		Evaluate(model, outputText);

		foreach (var text in outputText)
		{
			await utf8Pipe.WriteAsync(text, cancel).ConfigureAwait(false);
		}
	}

	internal long Evaluate(TModel model, PooledList<ReadOnlyMemory<byte>> list)
	{
		long totalLength = 0;

		for (int i = 0; i < this.segments.Length; i++)
		{
			var segment = this.segments[i];
			if (segment.Value is ReadOnlyMemory<byte> text)
			{
				list.Add(text);
				totalLength += text.Length;
			}
			else if (segment.Value is IBuiltSegment<TModel> built)
			{
				var items = built.Evaluate(model);
				if (items.TryGetNonEnumeratedCount(out var count))
				{
					list.EnsureCapacity(list.Count + count);
				}
				foreach (var item in items)
				{
					list.Add(item);
					totalLength += item.Length;
				}
			}
			else if (segment.Value is SkipCondition<TModel> skip)
			{
				i += skip.Evaluate(model);
			}
			else
			{
				Contract.Assert(segment.Value is BuiltEnumerator<TModel>);
				totalLength += ((BuiltEnumerator<TModel>)segment.Value).Evaluate(model, list);
			}
		}

		return totalLength;
	}
}
