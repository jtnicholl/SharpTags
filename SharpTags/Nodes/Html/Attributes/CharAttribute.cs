// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.Contracts;
using SharpTags.Built;
using SharpTags.Built.Attributes;
using SharpTags.Nodes.Html.Attributes.Values;

namespace SharpTags.Nodes.Html.Attributes;

internal sealed class CharAttribute<TModel> : HtmlAttribute<TModel>
{
	public CharAttributeValue<TModel> Value { get; }

	internal override bool RequiresModel => this.Value.Value is Delegate;
	protected override ReadOnlyMemory<byte> Name { get; }

	public CharAttribute(ReadOnlyMemory<byte> name, CharAttributeValue<TModel> value)
	{
		Name = name;
		Value = value;
	}

	[Pure]
	internal override AttributeResult Evaluate(TModel model)
	{
		return Value.Evaluate(model) is char c
			? new(MarkupEscape.Escape([c]))
			: AttributeResult.NotPresent;
	}

	internal override IEnumerable<BuiltMarkupSegment<TModel>> Build()
	{
		return Value switch
		{
			char c => [Name, (ReadOnlyMemory<byte>)MarkupEscape.Escape([c]), Name[^1..]],
			Func<TModel, char?> f => [new BuiltOptionalCharAttribute<TModel>(Name, f)],
			Func<TModel, char> f => [Name, new BuiltCharValue<TModel>(f), Name[^1..]],
			null => [],
		};
	}
}
