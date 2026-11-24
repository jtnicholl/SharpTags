// SPDX-License-Identifier: LGPL-3.0-or-later

namespace SharpTags.Built.Attributes;

internal readonly union NumberFunction<TModel>(
	Func<TModel, int>,
	Func<TModel, double>
);
