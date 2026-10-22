// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace SharpTags.Built.Attributes;

internal sealed class BuiltCharValue<TModel> : IBuiltSegment<TModel>
{
	private readonly Func<TModel, char> function;

	public BuiltCharValue(Func<TModel, char> function)
	{
		this.function = function;
	}

	[Pure]
	public IEnumerable<ReadOnlyMemory<byte>> Evaluate(TModel model)
	{
		return [MarkupEscape.Escape([this.function.Invoke(model)])];
	}

	[Pure]
	public bool Equals([NotNullWhen(true)] BuiltCharValue<TModel>? other)
	{
		return other is not null && this.function.Equals(other.function);
	}

	[Pure]
	public bool Equals([NotNullWhen(true)] IBuiltSegment<TModel>? obj)
	{
		return obj is BuiltCharValue<TModel> other && Equals(other);
	}

	[Pure]
	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		return obj is BuiltCharValue<TModel> other && Equals(other);
	}

	[Pure]
	public override int GetHashCode()
	{
		return this.function.GetHashCode();
	}

	public static bool operator ==(
		BuiltCharValue<TModel>? left,
		BuiltCharValue<TModel>? right
	)
	{
		return left?.Equals(right) ?? right is null;
	}

	public static bool operator !=(
		BuiltCharValue<TModel>? left,
		BuiltCharValue<TModel>? right
	)
	{
		return !(left == right);
	}
}
