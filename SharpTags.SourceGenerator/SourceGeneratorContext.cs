// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SharpTags.SourceGenerator;

[JsonSerializable(typeof(Elements))]
[JsonSerializable(typeof(GlobalAttributes))]
internal sealed partial class SourceGeneratorContext : JsonSerializerContext
{
	public static readonly SourceGeneratorContext File = new(
		new()
		{
			Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
			PropertyNameCaseInsensitive = true,
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		}
	);
}
