// SPDX-License-Identifier: LGPL-3.0-or-later

namespace SharpTags.Built.Attributes;

internal union OptionalNumberFunction<TModel>(
	Func<TModel, int?>,
	Func<TModel, double?>
);
