// SPDX-License-Identifier: LGPL-3.0-or-later

namespace SharpTags;

/// <summary>
/// Allows rendering UTF-8 text to a <see cref="Stream" />. The rendered text
/// varies depending on the provided <typeparamref name="TModel" />.
/// </summary>
public interface IStreamTextRender<in TModel>
{
	/// <summary>
	/// Write UTF-8 text to a <see cref="Stream" />.
	/// </summary>
	/// <param name="utf8Stream">The stream to write text to.</param>
	/// <param name="model">The model object to vary the text based on.</param>
	/// <param name="cancel">A token used to cancel the operation.</param>
	ValueTask Render(Stream utf8Stream, TModel model, CancellationToken cancel = default);
}
