// SPDX-License-Identifier: LGPL-3.0-or-later

namespace SharpTags.Built;

internal readonly union TextFunction<TModel>(
	Func<TModel, PlainText>,
	Func<TModel, string?>
);
