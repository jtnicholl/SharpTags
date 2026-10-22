// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Text.Json.Serialization;

namespace SharpTags.SourceGenerator;

[JsonConverter(typeof(JsonStringEnumConverter<ListSeparator>))]
internal enum ListSeparator
{
	Whitespace,
	Comma,
}
