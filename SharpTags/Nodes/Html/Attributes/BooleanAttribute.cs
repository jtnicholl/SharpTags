// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.Contracts;
using SharpTags.Built;
using SharpTags.Built.Attributes;
using SharpTags.Nodes.Html.Attributes.Values;

namespace SharpTags.Nodes.Html.Attributes;

internal sealed class BooleanAttribute<TModel> : HtmlAttribute<TModel>
{
	public BooleanAttributeValue<TModel> Value { get; }

	protected override ReadOnlyMemory<byte> Name { get; }

	internal override bool RequiresModel => Value.Value is Delegate;

	public BooleanAttribute(ReadOnlyMemory<byte> name, BooleanAttributeValue<TModel> value)
	{
		this.Name = name;
		this.Value = value;
	}

	[Pure]
	internal override IEnumerable<BuiltMarkupSegment<TModel>> Build()
	{
		return Value switch
		{
			true => [Name],
			Func<TModel, bool> f => [new BuiltBinaryAttribute<TModel>(Name, f)],
			false => [],
			null => [],
		};
	}

	[Pure]
	internal override AttributeResult Evaluate(TModel model)
	{
		return Value.Evaluate(model) ? AttributeResult.NoValue : AttributeResult.NotPresent;
	}
}
