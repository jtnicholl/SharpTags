// SPDX-License-Identifier: LGPL-3.0-or-later

namespace SharpTags.Built.Attributes;

internal readonly union OptionalTextFunction<TModel>(
	Func<TModel, PlainText?>,
	Func<TModel, string?>
);
