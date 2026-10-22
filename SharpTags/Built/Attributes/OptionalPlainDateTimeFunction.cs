// SPDX-License-Identifier: LGPL-3.0-or-later

namespace SharpTags.Built.Attributes;

internal union OptionalPlainDateTimeFunction<TModel>(
	Func<TModel, DateTime?>,
	Func<TModel, DateOnly?>
);
