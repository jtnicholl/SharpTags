// SPDX-License-Identifier: LGPL-3.0-or-later

namespace SharpTags.Nodes.Html.Attributes.Values;

/// <summary>
/// Union of possible values for an HTML datetime attribute, which optionally
/// has a time, but never time zone information.
/// </summary>
/// <typeparam name="TModel">The type of model, which can vary value.</typeparam>
public union PlainDateTimeAttributeValue<TModel>(
	DateTime,
	DateOnly,
	Func<TModel, DateTime>,
	Func<TModel, DateOnly>,
	Func<TModel, DateTime?>,
	Func<TModel, DateOnly?>
);
