// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.Contracts;
using SharpTags.Nodes.Html.Attributes.Values;

namespace SharpTags.Nodes.Html.Attributes;

partial class TableHeaderScopeAttribute
{
	[Pure]
	public static AttributeResult Evaluate(TableHeaderScope value)
	{
		return value switch
		{
			TableHeaderScope.Row => new(MiscAttributeValues.Row),
			TableHeaderScope.Column => new(MiscAttributeValues.Column),
			TableHeaderScope.RowGroup => new(MiscAttributeValues.RowGroup),
			TableHeaderScope.ColumnGroup => new(MiscAttributeValues.ColumnGroup),
			_ => AttributeResult.NotPresent,
		};
	}
}
