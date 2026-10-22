// SPDX-License-Identifier: LGPL-3.0-or-later

using SharpTags.Built;
using SharpTags.Built.Attributes;
using SharpTags.Nodes.Html.Attributes.Values;

namespace SharpTags.Nodes.Html.Attributes;

internal sealed class ListAttribute<TModel> : HtmlAttribute<TModel>
{
	public ListAttributeValue<TModel> Value { get; }

	protected override ReadOnlyMemory<byte> Name { get; }

	internal override bool RequiresModel => this.Value.Value is Delegate;

	private readonly PlainText separator;

	public ListAttribute(
		ReadOnlyMemory<byte> name,
		ListAttributeValue<TModel> value,
		PlainText separator
	)
	{
		Name = name;
		Value = value;
		this.separator = separator;
	}

	internal override AttributeResult Evaluate(TModel model)
	{
		return Value.Evaluate(model) is IEnumerable<PlainText> ie
			? PlainText.Join(this.separator, ie)
			: AttributeResult.NotPresent;
	}

	internal override IEnumerable<BuiltMarkupSegment<TModel>> Build()
	{
		return Value switch
		{
			PlainText pt => [Name, pt.Utf8Bytes, Name[^1..]],
			Func<TModel, PlainText> f => [Name, new BuiltText<TModel>(f), Name[^1..]],
			Func<TModel, PlainText?> f => [new BuiltStringAttribute<TModel>(Name, f)],
			Func<TModel, IEnumerable<PlainText>?> f =>
				[new BuiltListAttribute<TModel>(f, this.separator, Name)],
			Func<TModel, string?> f => [new BuiltStringAttribute<TModel>(Name, f)],
			Func<TModel, IEnumerable<string>?> f =>
				[new BuiltListAttribute<TModel>(f, this.separator, Name)],
			null => [],
		};
	}
}
