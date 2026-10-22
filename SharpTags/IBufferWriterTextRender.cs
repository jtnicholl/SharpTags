// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Buffers;

namespace SharpTags;

/// <summary>
/// Allows rendering UTF-8 text to an <see cref="IBufferWriter{T}" />. The
/// rendered text varies depending on the provided
/// <typeparamref name="TModel" />.
/// </summary>
public interface IBufferWriterTextRender<in TModel>
{
	/// <summary>
	/// Write UTF-8 text to an <see cref="IBufferWriter{T}" />.
	/// </summary>
	/// <param name="utf8Buffer">The buffer to write text to.</param>
	/// <param name="model">The model object to vary the text based on.</param>
	void Render(IBufferWriter<byte> utf8Buffer, TModel model);
}
