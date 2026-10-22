// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace SharpTags.Built;

internal sealed class BuiltText<TModel> : IBuiltSegment<TModel>
{
	private readonly TextFunction<TModel> function;

	public BuiltText(TextFunction<TModel> function)
	{
		this.function = function;
	}

	public IEnumerable<ReadOnlyMemory<byte>> Evaluate(TModel model)
	{
		Contract.Assert(function is not null);
		PlainText text = this.function switch
		{
			Func<TModel, PlainText> f => f.Invoke(model),
			Func<TModel, string?> f => new(f.Invoke(model)),
		};

		return [text.Utf8Bytes];
	}

	[Pure]
	public bool Equals([NotNullWhen(true)] BuiltText<TModel>? other)
	{
		Contract.Assert(this.function.Value is not null);
		return other is not null && this.function.Value.Equals(other.function.Value);
	}

	[Pure]
	public bool Equals([NotNullWhen(true)] IBuiltSegment<TModel>? obj)
	{
		return obj is BuiltText<TModel> other && Equals(other);
	}

	[Pure]
	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		return obj is BuiltText<TModel> other && Equals(other);
	}

	[Pure]
	public override int GetHashCode()
	{
		Contract.Assert(this.function.Value is not null);
		return this.function.Value.GetHashCode();
	}

	public static bool operator ==(
		BuiltText<TModel>? left,
		BuiltText<TModel>? right
	)
	{
		return left?.Equals(right) ?? right is null;
	}

	public static bool operator !=(
		BuiltText<TModel>? left,
		BuiltText<TModel>? right
	)
	{
		return !(left == right);
	}
}
