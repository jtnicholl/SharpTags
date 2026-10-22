// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using SharpTags.Nodes.Html.Attributes;

namespace SharpTags.Built.Attributes;

internal sealed class BuiltEnumAttribute<TModel, TEnum, TEnumAttribute> : IBuiltSegment<TModel>
	where TEnum : struct, Enum
	where TEnumAttribute : IEnumAttribute<TEnum>
{
	private readonly Func<TModel, TEnum> function;

	public BuiltEnumAttribute(Func<TModel, TEnum> function)
	{
		this.function = function;
	}

	[Pure]
	public IEnumerable<ReadOnlyMemory<byte>> Evaluate(TModel model)
	{
		var result = TEnumAttribute.Evaluate(this.function.Invoke(model));

		return result.Result switch
		{
			AttributeResult.ResultType.HasValue =>
				[TEnumAttribute.Name, result.Value, TEnumAttribute.Name[^1..]],
			AttributeResult.ResultType.NoValue => [TEnumAttribute.Name[..^2]],
			_ => [],
		};
	}

	[Pure]
	public bool Equals([NotNullWhen(true)] BuiltEnumAttribute<TModel, TEnum, TEnumAttribute>? other)
	{
		return other is not null && this.function.Equals(other.function);
	}

	[Pure]
	public bool Equals([NotNullWhen(true)] IBuiltSegment<TModel>? obj)
	{
		return obj is BuiltEnumAttribute<TModel, TEnum, TEnumAttribute> other && Equals(other);
	}

	[Pure]
	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		return obj is BuiltEnumAttribute<TModel, TEnum, TEnumAttribute> other && Equals(other);
	}

	[Pure]
	public override int GetHashCode()
	{
		return this.function.GetHashCode();
	}

	public static bool operator ==(
		BuiltEnumAttribute<TModel, TEnum, TEnumAttribute>? left,
		BuiltEnumAttribute<TModel, TEnum, TEnumAttribute>? right
	)
	{
		return left?.Equals(right) ?? right is null;
	}

	public static bool operator !=(
		BuiltEnumAttribute<TModel, TEnum, TEnumAttribute>? left,
		BuiltEnumAttribute<TModel, TEnum, TEnumAttribute>? right
	)
	{
		return !(left == right);
	}
}
