using System.Diagnostics.Contracts;
using SharpTags.Built;
using SharpTags.Built.Attributes;
using SharpTags.Nodes.Html.Attributes.Values;

namespace SharpTags.Nodes.Html.Attributes;

internal sealed class EnumAttribute<TModel, TEnum, TEnumAttribute> : HtmlAttribute<TModel>
	where TEnum : struct, Enum
	where TEnumAttribute : IEnumAttribute<TEnum>
{
	public EnumAttributeValue<TModel, TEnum> Value { get; }

	protected override ReadOnlyMemory<byte> Name => TEnumAttribute.Name;

	internal override bool RequiresModel => Value.Value is Delegate;

	public EnumAttribute(EnumAttributeValue<TModel, TEnum> value)
	{
		this.Value = value;
	}

	internal override AttributeResult Evaluate(TModel model)
	{
		return TEnumAttribute.Evaluate(Value.Evaluate(model));
	}

	internal override IEnumerable<BuiltMarkupSegment<TModel>> Build()
	{
		return Value switch
		{
			TEnum e => Build(e),
			Func<TModel, TEnum> f => [new BuiltEnumAttribute<TModel, TEnum, TEnumAttribute>(f)],
			null => [],
		};
	}

	[Pure]
	internal static IEnumerable<BuiltMarkupSegment<TModel>> Build(TEnum value)
	{
		var result = TEnumAttribute.Evaluate(value);
		return result.Result switch
		{
			AttributeResult.ResultType.HasValue =>
				[TEnumAttribute.Name, result.Value, TEnumAttribute.Name[^1..]],
			AttributeResult.ResultType.NoValue => [TEnumAttribute.Name[^2..]],
			_ => [],
		};
	}
}
