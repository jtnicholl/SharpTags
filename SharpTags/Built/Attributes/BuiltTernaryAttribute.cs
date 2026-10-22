// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using SharpTags.Nodes.Html.Attributes;

namespace SharpTags.Built.Attributes;

internal sealed class BuiltTernaryAttribute<TModel, TAttribute> : IBuiltSegment<TModel>
	where TAttribute : ITernaryAttribute
{
	// ReSharper disable once StaticMemberInGenericType
	private static readonly ReadOnlyMemory<byte>[] trueValue = [TAttribute.TrueValue];
	// ReSharper disable once StaticMemberInGenericType
	private static readonly ReadOnlyMemory<byte>[] falseValue = [TAttribute.FalseValue];

	private readonly TernaryPredicate<TModel> predicate;

	public BuiltTernaryAttribute(TernaryPredicate<TModel> predicate)
	{
		this.predicate = predicate;
	}

	public IEnumerable<ReadOnlyMemory<byte>> Evaluate(TModel model)
	{
		Contract.Assert(this.predicate is not null);

		return this.predicate switch
		{
			Func<TModel, bool?> f => f.Invoke(model) switch
			{
				true => trueValue,
				false => falseValue,
				null => [],
			},
			Func<TModel, bool> f => f.Invoke(model) ? trueValue : falseValue,
		};
	}

	[Pure]
	public bool Equals([NotNullWhen(true)] BuiltTernaryAttribute<TModel, TAttribute>? other)
	{
		Contract.Assert(this.predicate.Value is not null);
		return other is not null && this.predicate.Value.Equals(other.predicate.Value);
	}

	[Pure]
	public bool Equals([NotNullWhen(true)] IBuiltSegment<TModel>? obj)
	{
		return obj is BuiltTernaryAttribute<TModel, TAttribute> other && Equals(other);
	}

	[Pure]
	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		return obj is BuiltTernaryAttribute<TModel, TAttribute> other && Equals(other);
	}

	[Pure]
	public override int GetHashCode()
	{
		Contract.Assert(this.predicate.Value is not null);
		return this.predicate.Value.GetHashCode();
	}

	public static bool operator ==(
		BuiltTernaryAttribute<TModel, TAttribute>? left,
		BuiltTernaryAttribute<TModel, TAttribute>? right
	)
	{
		return left?.Equals(right) ?? right is null;
	}

	public static bool operator !=(
		BuiltTernaryAttribute<TModel, TAttribute>? left,
		BuiltTernaryAttribute<TModel, TAttribute>? right
	)
	{
		return !(left == right);
	}
}
