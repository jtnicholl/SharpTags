// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace SharpTags.Built.Attributes;

internal sealed class BuiltOptionalCharAttribute<TModel> : IBuiltSegment<TModel>
{
	private readonly ReadOnlyMemory<byte> name;
	private readonly Func<TModel, char?> function;

	public BuiltOptionalCharAttribute(
		ReadOnlyMemory<byte> name,
		Func<TModel, char?> function
	)
	{
		Contract.Assert(name.Span.StartsWith(" "u8));
		Contract.Assert(name.Span.EndsWith("=\""u8));
		this.name = name;
		this.function = function;
	}

	[Pure]
	public IEnumerable<ReadOnlyMemory<byte>> Evaluate(TModel model)
	{
		return this.function.Invoke(model) is char c
			? [this.name, MarkupEscape.Escape([c]), this.name[^1..]]
			: [];
	}

	[Pure]
	private bool Equals([NotNullWhen(true)] BuiltOptionalCharAttribute<TModel>? other)
	{
		return other is not null &&
			this.name.Span.SequenceEqual(other.name.Span) &&
			this.function.Equals(other.function);
	}

	[Pure]
	public bool Equals([NotNullWhen(true)] IBuiltSegment<TModel>? obj)
	{
		return obj is BuiltOptionalCharAttribute<TModel> other && Equals(other);
	}

	[Pure]
	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		return obj is BuiltOptionalCharAttribute<TModel> other && Equals(other);
	}

	[Pure]
	public override int GetHashCode()
	{
		HashCode output = new();

		output.Add(this.function);
		output.AddBytes(this.name.Span);

		return output.ToHashCode();
	}

	public static bool operator ==(
		BuiltOptionalCharAttribute<TModel>? left,
		BuiltOptionalCharAttribute<TModel>? right
	)
	{
		return left?.Equals(right) ?? right is null;
	}

	public static bool operator !=(
		BuiltOptionalCharAttribute<TModel>? left,
		BuiltOptionalCharAttribute<TModel>? right
	)
	{
		return !(left == right);
	}
}
