// SPDX-License-Identifier: LGPL-3.0-or-later

using SharpTags.Built;
using SharpTags.Built.Attributes;
using SharpTags.Nodes.Html.Attributes.Values;

namespace SharpTags.Nodes.Html.Attributes;

internal sealed class PlainDateTimeAttribute<TModel> : HtmlAttribute<TModel>
{
	public PlainDateTimeAttributeValue<TModel> Value { get; }

	protected override ReadOnlyMemory<byte> Name => AttributeNames.DateTime;

	internal override bool RequiresModel => this.Value.Value is Delegate;

	public PlainDateTimeAttribute(PlainDateTimeAttributeValue<TModel> value)
	{
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
			DateOnly d => d == default
				? []
				: [
					AttributeNames.DateTime,
					new PlainText(d).Utf8Bytes,
					AttributeNames.DateTime[^1..],
				],
			DateTime dt => dt == default
				? []
				: [
					AttributeNames.DateTime,
					new PlainText(dt).Utf8Bytes,
					AttributeNames.DateTime[^1..],
				],
			Func<TModel, DateTime?> f => [new BuiltOptionalPlainDateTimeAttribute<TModel>(f)],
			Func<TModel, DateOnly?> f => [new BuiltOptionalPlainDateTimeAttribute<TModel>(f)],
			Func<TModel, DateTime> f =>
			[
				AttributeNames.DateTime,
				new BuiltPlainDateTimeValue<TModel>(f),
				AttributeNames.DateTime[^1..],
			],
			Func<TModel, DateOnly> f =>
			[
				AttributeNames.DateTime,
				new BuiltPlainDateTimeValue<TModel>(f),
				AttributeNames.DateTime[^1..],
			],
			null => [],
		};
	}
}
