// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.Contracts;
using SharpTags.Built;
using SharpTags.Built.Attributes;
using SharpTags.Nodes.Html.Attributes.Values;

namespace SharpTags.Nodes.Html.Attributes;

internal sealed class IntegerAttribute<TModel> : HtmlAttribute<TModel>
{
	protected override ReadOnlyMemory<byte> Name { get; }

	public IntegerAttributeValue<TModel> Value { get; }

	internal override bool RequiresModel => this.Value.Value is Delegate;

	public IntegerAttribute(ReadOnlyMemory<byte> name, IntegerAttributeValue<TModel> value)
	{
		Contract.Assert(name.Span.StartsWith(" "u8));
		Contract.Assert(name.Span.EndsWith("=\""u8));
		Name = name;
		Value = value;
	}

	internal override AttributeResult Evaluate(TModel model)
	{
		return new(Value.Evaluate(model));
	}

	internal override IEnumerable<BuiltMarkupSegment<TModel>> Build()
	{
		return Value switch
		{
			int i => [Name, new PlainText(i).Utf8Bytes, Name[^1..]],
			Func<TModel, int> f => [Name, new BuiltNumberValue<TModel>(f), Name[^1..]],
			Func<TModel, int?> f => [new BuiltOptionalNumberAttribute<TModel>(Name, f)],
			null => [],
		};
	}
}
