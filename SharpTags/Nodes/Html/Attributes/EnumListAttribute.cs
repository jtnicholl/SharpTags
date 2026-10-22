using SharpTags.Built;
using SharpTags.Built.Attributes;
using SharpTags.Nodes.Html.Attributes.Values;

namespace SharpTags.Nodes.Html.Attributes;

internal sealed class EnumListAttribute<TModel, TEnum, TEnumAttribute> : HtmlAttribute<TModel>
	where TEnum : struct, Enum
	where TEnumAttribute : IEnumAttribute<TEnum>
{
	protected override ReadOnlyMemory<byte> Name => TEnumAttribute.Name;
	public EnumListAttributeValue<TModel, TEnum> Value { get; }
	public PlainText Separator { get; }
	internal override bool RequiresModel => Value.Value is Delegate;

	public EnumListAttribute(
		EnumListAttributeValue<TModel, TEnum> value,
		PlainText separator
	)
	{
		this.Value = value;
		this.Separator = separator;
	}

	internal override AttributeResult Evaluate(TModel model)
	{
		var value = Value.Evaluate(model);
		if (value != null)
		{
			return PlainText.Join(
				Separator,
				value
					.Select(TEnumAttribute.Evaluate)
					.Where(r => r.HasValue)
					.Select(r => PlainText.UnsafeReference(r.Value))
			);
		}
		return AttributeResult.NotPresent;
	}

	internal override IEnumerable<BuiltMarkupSegment<TModel>> Build()
	{
		return Value.Value switch
		{
			TEnum e => EnumAttribute<TModel, TEnum, TEnumAttribute>.Build(e),
			Func<TModel, TEnum> f => [new BuiltEnumAttribute<TModel, TEnum, TEnumAttribute>(f)],
			Func<TModel, IEnumerable<TEnum>?> f =>
				[new BuiltEnumListAttribute<TModel, TEnum, TEnumAttribute>(f, Separator)],
			_ => [],
		};
	}
}
