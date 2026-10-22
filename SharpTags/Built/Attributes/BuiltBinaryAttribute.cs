// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace SharpTags.Built.Attributes;

internal sealed class BuiltBinaryAttribute<TModel> : IBuiltSegment<TModel>
{
	private readonly ReadOnlyMemory<byte> name;
	private readonly Func<TModel, bool> predicate;

	public BuiltBinaryAttribute(ReadOnlyMemory<byte> name, Func<TModel, bool> predicate)
	{
		Contract.Assert(name.Span.StartsWith(" "u8));
		Contract.Assert(!name.Span.EndsWith("\""u8));
		Contract.Assert(!name.Span.EndsWith("="u8));
		this.name = name;
		this.predicate = predicate;
	}

	[Pure]
	public IEnumerable<ReadOnlyMemory<byte>> Evaluate(TModel model)
	{
		return this.predicate.Invoke(model) ? [this.name] : [];
	}

	[Pure]
	private bool Equals([NotNullWhen(true)] BuiltBinaryAttribute<TModel>? other)
	{
		return other != null &&
			this.predicate.Equals(other.predicate) &&
			this.name.Span.SequenceEqual(other.name.Span);
	}

	[Pure]
	public bool Equals([NotNullWhen(true)] IBuiltSegment<TModel>? obj)
	{
		return obj is BuiltBinaryAttribute<TModel> other && Equals(other);
	}

	[Pure]
	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		return obj is BuiltBinaryAttribute<TModel> other && Equals(other);
	}

	[Pure]
	public override int GetHashCode()
	{
		HashCode output = new();

		output.Add(this.predicate);
		output.AddBytes(this.name.Span);

		return output.ToHashCode();
	}

	public static bool operator ==(
		BuiltBinaryAttribute<TModel>? left,
		BuiltBinaryAttribute<TModel>? right
	)
	{
		return left?.Equals(right) ?? right is null;
	}

	public static bool operator !=(
		BuiltBinaryAttribute<TModel>? left,
		BuiltBinaryAttribute<TModel>? right
	)
	{
		return !(left == right);
	}
}
