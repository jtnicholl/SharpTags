// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.Contracts;

namespace SharpTags.Built;

internal union BuiltMarkupSegment<TModel>(
	ReadOnlyMemory<byte>,
	IBuiltSegment<TModel>,
	SkipCondition<TModel>,
	BuiltEnumerator<TModel>
) : IEquatable<BuiltMarkupSegment<TModel>>
{
	public readonly bool Equals(BuiltMarkupSegment<TModel> other)
	{
		return this switch
		{
			ReadOnlyMemory<byte> mem =>
				other.Value is ReadOnlyMemory<byte> otherMem &&
				mem.Span.SequenceEqual(otherMem.Span),
			IBuiltSegment<TModel> b => b.Equals(other.Value),
			SkipCondition<TModel> s => s.Equals(other.Value),
			BuiltEnumerator<TModel> e => e.Equals(other.Value),
			null => other is null,
		};
	}

	/// <inheritdoc />
	public readonly override bool Equals(object? obj)
	{
		return (obj is BuiltMarkupSegment<TModel> other && Equals(other)) ||
			(obj == null && this is null);
	}

	/// <inheritdoc />
	[Pure]
	public override int GetHashCode()
	{
		return Value?.GetHashCode() ?? 0;
	}

	public static bool operator ==(
		BuiltMarkupSegment<TModel> left,
		BuiltMarkupSegment<TModel> right
	)
	{
		return left.Equals(right);
	}

	public static bool operator !=(
		BuiltMarkupSegment<TModel> left,
		BuiltMarkupSegment<TModel> right
	)
	{
		return !left.Equals(right);
	}
}
