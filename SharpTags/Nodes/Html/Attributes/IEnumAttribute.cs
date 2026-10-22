using System.Diagnostics.Contracts;

namespace SharpTags.Nodes.Html.Attributes;

internal interface IEnumAttribute<in TEnum> where TEnum : struct, Enum
{
	static abstract ReadOnlyMemory<byte> Name { get; }

	[Pure]
	static abstract AttributeResult Evaluate(TEnum value);
}
