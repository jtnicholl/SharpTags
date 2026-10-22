// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using SharpTags.Nodes.Html.Attributes;

namespace SharpTags.Built.Attributes;

internal sealed class BuiltEnumListAttribute<TModel, TEnum, TEnumAttribute> : IBuiltSegment<TModel>
	where TEnum : struct, Enum
	where TEnumAttribute : IEnumAttribute<TEnum>
{
	private readonly Func<TModel, IEnumerable<TEnum>?> function;
	private readonly PlainText separator;

	public BuiltEnumListAttribute(Func<TModel, IEnumerable<TEnum>?> function, PlainText separator)
	{
		this.function = function;
		this.separator = separator;
	}

	[Pure]
	public IEnumerable<ReadOnlyMemory<byte>> Evaluate(TModel model)
	{
		var value = this.function.Invoke(model);
		return value is null ? [] : Evaluate(this.separator.Utf8Bytes, value);
	}

	[Pure]
	private static IEnumerable<ReadOnlyMemory<byte>> Evaluate(
		ReadOnlyMemory<byte> separator,
		IEnumerable<TEnum> value
	)
	{
		yield return TEnumAttribute.Name;
		using var enumerator = value.GetEnumerator();

		AttributeResult current;
		do
		{
			if (!enumerator.MoveNext())
			{
				yield return TEnumAttribute.Name[^1..];
				yield break;
			}
			current = TEnumAttribute.Evaluate(enumerator.Current);
			Contract.Assert(current.Result != AttributeResult.ResultType.NoValue);
		} while (!current.HasValue);

		yield return current.Value;

		while (enumerator.MoveNext())
		{
			current = TEnumAttribute.Evaluate(enumerator.Current);
			Contract.Assert(current.Result != AttributeResult.ResultType.NoValue);
			if (current.HasValue)
			{
				yield return separator;
				yield return current.Value;
			}
		}

		yield return TEnumAttribute.Name[^1..];
	}

	[Pure]
	public bool Equals(
		[NotNullWhen(true)] BuiltEnumListAttribute<TModel, TEnum, TEnumAttribute>? other
	)
	{
		return other is not null &&
			this.function.Equals(other.function) &&
			this.separator.Equals(other.separator);
	}

	[Pure]
	public bool Equals([NotNullWhen(true)] IBuiltSegment<TModel>? obj)
	{
		return obj is BuiltEnumListAttribute<TModel, TEnum, TEnumAttribute> other && Equals(other);
	}

	[Pure]
	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		return obj is BuiltEnumListAttribute<TModel, TEnum, TEnumAttribute> other && Equals(other);
	}

	[Pure]
	public override int GetHashCode()
	{
		return HashCode.Combine(this.function, this.separator);
	}

	public static bool operator ==(
		BuiltEnumListAttribute<TModel, TEnum, TEnumAttribute>? left,
		BuiltEnumListAttribute<TModel, TEnum, TEnumAttribute>? right
	)
	{
		return left?.Equals(right) ?? right is null;
	}

	public static bool operator !=(
		BuiltEnumListAttribute<TModel, TEnum, TEnumAttribute>? left,
		BuiltEnumListAttribute<TModel, TEnum, TEnumAttribute>? right
	)
	{
		return !(left == right);
	}
}
