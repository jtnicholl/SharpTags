// SPDX-License-Identifier: LGPL-3.0-or-later

namespace SharpTags.Built;

internal readonly union PlainDateTimeFunction<TModel>(
	Func<TModel, DateTime>,
	Func<TModel, DateOnly>
);
