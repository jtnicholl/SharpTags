// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.CodeAnalysis;

namespace SharpTags.Nodes.Html.Attributes.Values;

/// <summary>
/// Not yet implemented. Do not use.
/// </summary>
[Experimental("STAGS_001")]
public readonly union SourceDocumentAttributeValue<TModel>(
	PlainText,
	Func<TModel, PlainText>
);
