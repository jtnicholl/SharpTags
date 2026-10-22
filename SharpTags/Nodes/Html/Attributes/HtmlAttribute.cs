// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Buffers;
using System.Diagnostics.Contracts;
using System.IO.Pipelines;
using SharpTags.Built;

namespace SharpTags.Nodes.Html.Attributes;

internal abstract class HtmlAttribute<TModel> : ITextRender<TModel>
{
	protected abstract ReadOnlyMemory<byte> Name { get; }

	internal abstract bool RequiresModel { get; }

	private protected HtmlAttribute() { }

	[Pure]
	internal abstract AttributeResult Evaluate(TModel model);

	[Pure]
	internal abstract IEnumerable<BuiltMarkupSegment<TModel>> Build();

	/// <inheritdoc />
	public async ValueTask Render(Stream utf8Stream, TModel model, CancellationToken cancel)
	{
		var result = Evaluate(model);

		if (result.HasValue)
		{
			Contract.Assert(Name.Span.EndsWith("=\""u8));
			await utf8Stream.WriteAsync(Name, cancel).ConfigureAwait(false);
			await utf8Stream.WriteAsync(result.Value, cancel).ConfigureAwait(false);
			await utf8Stream.WriteAsync(Name[^1..], cancel).ConfigureAwait(false);
		}
		else if (result.IsPresent)
		{
			await utf8Stream.WriteAsync(Name.TrimEnd("=\""u8), cancel).ConfigureAwait(false);
		}
	}

	/// <inheritdoc />
	public async ValueTask Render(PipeWriter utf8Pipe, TModel model, CancellationToken cancel)
	{
		var result = Evaluate(model);

		if (result.HasValue)
		{
			Contract.Assert(Name.Span.EndsWith("=\""u8));
			await utf8Pipe.WriteAsync(Name, cancel).ConfigureAwait(false);
			await utf8Pipe.WriteAsync(result.Value, cancel).ConfigureAwait(false);
			await utf8Pipe.WriteAsync(Name[^1..], cancel).ConfigureAwait(false);
		}
		else if (result.IsPresent)
		{
			await utf8Pipe.WriteAsync(Name.TrimEnd("=\""u8), cancel).ConfigureAwait(false);
		}
	}

	/// <inheritdoc />
	public void Render(IBufferWriter<byte> utf8Buffer, TModel model)
	{
		var result = Evaluate(model);

		if (result.HasValue)
		{
			Contract.Assert(Name.Span.EndsWith("=\""u8));
			utf8Buffer.Write(Name.Span);
			utf8Buffer.Write(result.Value.Span);
			utf8Buffer.Write(Name[^1..].Span);
		}
		else if (result.IsPresent)
		{
			utf8Buffer.Write(Name.TrimEnd("=\""u8).Span);
		}
	}
}
