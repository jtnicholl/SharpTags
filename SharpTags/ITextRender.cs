// SPDX-License-Identifier: LGPL-3.0-or-later

namespace SharpTags;

internal interface ITextRender<in TModel> :
	IStreamTextRender<TModel>,
	IPipeWriterTextRender<TModel>,
	IBufferWriterTextRender<TModel>;
