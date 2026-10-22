// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace SharpTags.Built.Attributes;

internal sealed class BuiltNumberValue<TModel> : IBuiltSegment<TModel>
{
	private readonly NumberFunction<TModel> function;

	public BuiltNumberValue(NumberFunction<TModel> function)
	{
		Contract.Assert(function.Value is not null);
		this.function = function;
	}

	public IEnumerable<ReadOnlyMemory<byte>> Evaluate(TModel model)
	{
		Contract.Assert(function is not null);
		var value = this.function switch
		{
			Func<TModel, int> f => new PlainText(f.Invoke(model)),
			Func<TModel, double> f => new PlainText(f.Invoke(model)),
		};

		return [value.Utf8Bytes];
	}

	[Pure]
	public bool Equals([NotNullWhen(true)] BuiltNumberValue<TModel>? other)
	{
		Contract.Assert(this.function.Value is not null);
		return other is not null && this.function.Value.Equals(other.function.Value);
	}

	[Pure]
	public bool Equals([NotNullWhen(true)] IBuiltSegment<TModel>? obj)
	{
		return obj is BuiltNumberValue<TModel> other && Equals(other);
	}

	[Pure]
	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		return obj is BuiltNumberValue<TModel> other && Equals(other);
	}

	[Pure]
	public override int GetHashCode()
	{
		Contract.Assert(this.function.Value is not null);
		return this.function.Value.GetHashCode();
	}

	public static bool operator ==(BuiltNumberValue<TModel>? left, BuiltNumberValue<TModel>? right)
	{
		return left?.Equals(right) ?? right is null;
	}

	public static bool operator !=(BuiltNumberValue<TModel>? left, BuiltNumberValue<TModel>? right)
	{
		return !(left == right);
	}
}
