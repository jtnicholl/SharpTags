// SPDX-License-Identifier: LGPL-3.0-or-later

namespace SharpTags.Built;

internal union PlainDateTimeFunction<TModel>(
	Func<TModel, DateTime>,
	Func<TModel, DateOnly>
);
