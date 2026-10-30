// SPDX-License-Identifier: LGPL-3.0-or-later

using SharpTags.Built;
using SharpTags.Built.Attributes;
using SharpTags.Nodes.Html.Attributes.Values;

namespace SharpTags.Nodes.Html.Attributes;

internal sealed class TernaryAttribute<TModel, TAttribute> : HtmlAttribute<TModel>
	where TAttribute : ITernaryAttribute
{
	public TernaryAttributeValue<TModel> Value { get; }

	protected override ReadOnlyMemory<byte> Name => TAttribute.Name;

	internal override bool RequiresModel => this.Value.Value is Delegate;

	public TernaryAttribute(TernaryAttributeValue<TModel> value)
	{
		this.Value = value;
	}

	internal override AttributeResult Evaluate(TModel model)
	{
		return TAttribute.Evaluate(Value.Evaluate(model));
	}

	internal override IEnumerable<BuiltMarkupSegment<TModel>> Build()
	{
		return Value switch
		{
			true => [TAttribute.TrueValue],
			false => [TAttribute.FalseValue],
			Func<TModel, bool?> f => [new BuiltTernaryAttribute<TModel, TAttribute>(f)],
			Func<TModel, bool> f => [new BuiltTernaryAttribute<TModel, TAttribute>(f)],
			null => [],
		};
	}
}
