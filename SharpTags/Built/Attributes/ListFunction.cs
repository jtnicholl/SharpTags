// SPDX-License-Identifier: LGPL-3.0-or-later

namespace SharpTags.Built.Attributes;

internal readonly union ListFunction<TModel>(
	Func<TModel, IEnumerable<PlainText>?>,
	Func<TModel, IEnumerable<string>?>
);
