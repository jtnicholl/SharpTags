// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.Contracts;
using SharpTags.Nodes.Html.Attributes.Values;

namespace SharpTags.Nodes.Html.Attributes;

partial class ScriptTypeAttribute
{
	[Pure]
	public static AttributeResult Evaluate(ScriptType value)
	{
		return value switch
		{
			ScriptType.ImportMap => new(MiscAttributeValues.ImportMap),
			ScriptType.Module => new(MiscAttributeValues.Module),
			_ => AttributeResult.NotPresent,
		};
	}
}
