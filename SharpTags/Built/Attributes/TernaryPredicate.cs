// SPDX-License-Identifier: LGPL-3.0-or-later

namespace SharpTags.Built.Attributes;

internal readonly union TernaryPredicate<TModel>(
	Func<TModel, bool?>,
	Func<TModel, bool>
);
