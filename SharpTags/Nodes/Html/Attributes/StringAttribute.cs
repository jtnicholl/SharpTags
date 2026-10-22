// SPDX-License-Identifier: LGPL-3.0-or-later

using SharpTags.Built;
using SharpTags.Built.Attributes;
using SharpTags.Nodes.Html.Attributes.Values;

namespace SharpTags.Nodes.Html.Attributes;

internal sealed class StringAttribute<TModel> : HtmlAttribute<TModel>
{
	public StringAttributeValue<TModel> Value { get; }

	protected override ReadOnlyMemory<byte> Name { get; }

	internal override bool RequiresModel => Value.Value is Delegate;

	public StringAttribute(ReadOnlyMemory<byte> name, StringAttributeValue<TModel> value)
	{
		this.Name = name;
		this.Value = value;
	}

	internal override AttributeResult Evaluate(TModel model)
	{
		return Value.Evaluate(model) ?? AttributeResult.NotPresent;
	}

	internal override IEnumerable<BuiltMarkupSegment<TModel>> Build()
	{
		return Value switch
		{
			PlainText pt => [Name, pt.Utf8Bytes, Name[^1..]],
			Func<TModel, PlainText> f => [Name, new BuiltText<TModel>(f), Name[^1..]],
			Func<TModel, PlainText?> f => [new BuiltStringAttribute<TModel>(Name, f)],
			Func<TModel, string?> f => [new BuiltStringAttribute<TModel>(Name, f)],
			null => [],
		};
	}
}
