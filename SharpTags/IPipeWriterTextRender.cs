// SPDX-License-Identifier: LGPL-3.0-or-later

using System.IO.Pipelines;

namespace SharpTags;

/// <summary>
/// Allows rendering UTF-8 text to a <see cref="PipeWriter" />. The rendered
/// text varies depending on the provided <typeparamref name="TModel" />.
/// </summary>
public interface IPipeWriterTextRender<in TModel>
{
	/// <summary>
	/// Write UTF-8 text to a <see cref="PipeWriter" />.
	/// </summary>
	/// <param name="utf8Pipe">The pipe to write text to.</param>
	/// <param name="model">The model object to vary the text based on.</param>
	/// <param name="cancel">A token used to cancel the operation.</param>
	ValueTask Render(PipeWriter utf8Pipe, TModel model, CancellationToken cancel = default);
}
