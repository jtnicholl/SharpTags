// SPDX-License-Identifier: LGPL-3.0-or-later

using SharpTags.Built;
using SharpTags.Built.Attributes;
using SharpTags.Nodes.Html.Attributes.Values;

namespace SharpTags.Nodes.Html.Attributes;

internal sealed class NumberAttribute<TModel> : HtmlAttribute<TModel>
{
	protected override ReadOnlyMemory<byte> Name { get; }

	public NumberAttributeValue<TModel> Value { get; }

	internal override bool RequiresModel => this.Value.Value is Delegate;

	public NumberAttribute(ReadOnlyMemory<byte> name, NumberAttributeValue<TModel> value)
	{
		Name = name;
		Value = value;
	}

	internal override AttributeResult Evaluate(TModel model)
	{
		return Value.Evaluate(model) is double d ? new(d) : AttributeResult.NotPresent;
	}

	internal override IEnumerable<BuiltMarkupSegment<TModel>> Build()
	{
		return Value switch
		{
			int i => [Name, new PlainText(i).Utf8Bytes, Name[^1..]],
			double d => [Name, new PlainText(d).Utf8Bytes, Name[^1..]],
			Func<TModel, int> f => [Name, new BuiltNumberValue<TModel>(f), Name[^1..]],
			Func<TModel, int?> f => [new BuiltOptionalNumberAttribute<TModel>(Name, f)],
			Func<TModel, double> f => [Name, new BuiltNumberValue<TModel>(f), Name[^1..]],
			Func<TModel, double?> f => [new BuiltOptionalNumberAttribute<TModel>(Name, f)],
			null => [],
		};
	}
}
