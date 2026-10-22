// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Text.Json.Serialization;

namespace SharpTags.SourceGenerator;

[JsonConverter(typeof(JsonStringEnumConverter<HtmlAttributeType>))]
internal enum HtmlAttributeType
{
	Undefined,
	String,
	Enum,
	Bool,
	Ternary,
	Int,
	Number,
	List,
	EnumList,
	Char,
	Uri,
	SourceDocument,
	MimeType,
	PlainDateTime,
}
