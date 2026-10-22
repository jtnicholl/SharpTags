// SPDX-License-Identifier: LGPL-3.0-or-later

namespace SharpTags.Built.Attributes;

internal union ListFunction<TModel>(
	Func<TModel, IEnumerable<PlainText>?>,
	Func<TModel, IEnumerable<string>?>
);
