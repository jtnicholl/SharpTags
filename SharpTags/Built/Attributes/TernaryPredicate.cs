// SPDX-License-Identifier: LGPL-3.0-or-later

namespace SharpTags.Built.Attributes;

internal union TernaryPredicate<TModel>(
	Func<TModel, bool?>,
	Func<TModel, bool>
);
