// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace SharpTags.Built;

internal sealed class BuiltPlainDateTimeValue<TModel> : IBuiltSegment<TModel>
{
	private readonly PlainDateTimeFunction<TModel> function;

	public BuiltPlainDateTimeValue(PlainDateTimeFunction<TModel> function)
	{
		Contract.Assert(function.Value is not null);
		this.function = function;
	}

	public IEnumerable<ReadOnlyMemory<byte>> Evaluate(TModel model)
	{
		Contract.Assert(this.function is not null);
		PlainText value = this.function switch
		{
			Func<TModel, DateTime> f => new(f.Invoke(model)),
			Func<TModel, DateOnly> f => new(f.Invoke(model)),
		};

		return [value.Utf8Bytes];
	}

	[Pure]
	public bool Equals([NotNullWhen(true)] BuiltPlainDateTimeValue<TModel>? other)
	{
		Contract.Assert(this.function.Value is not null);
		return other is not null && this.function.Value.Equals(other.function.Value);
	}

	[Pure]
	public bool Equals([NotNullWhen(true)] IBuiltSegment<TModel>? obj)
	{
		return obj is BuiltPlainDateTimeValue<TModel> other && Equals(other);
	}

	[Pure]
	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		return obj is BuiltPlainDateTimeValue<TModel> other && Equals(other);
	}

	[Pure]
	public override int GetHashCode()
	{
		Contract.Assert(this.function.Value is not null);
		return this.function.Value.GetHashCode();
	}

	public static bool operator ==(
		BuiltPlainDateTimeValue<TModel>? left,
		BuiltPlainDateTimeValue<TModel>? right
	)
	{
		return left?.Equals(right) ?? right is null;
	}

	public static bool operator !=(
		BuiltPlainDateTimeValue<TModel>? left,
		BuiltPlainDateTimeValue<TModel>? right
	)
	{
		return !(left == right);
	}
}
