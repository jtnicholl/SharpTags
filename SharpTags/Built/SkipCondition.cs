// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace SharpTags.Built;

internal sealed class SkipCondition<TModel> : IEquatable<SkipCondition<TModel>>
{
	private readonly Func<TModel, bool>? predicate;
	private readonly int trueSkip;
	private readonly int falseSkip;

	public SkipCondition(Func<TModel, bool>? predicate, int trueSkip, int falseSkip)
	{
		Contract.Assert(trueSkip >= 0);
		Contract.Assert(falseSkip >= 0);
		Contract.Assert(trueSkip > 0 || falseSkip > 0);
		this.predicate = predicate;
		this.trueSkip = trueSkip;
		this.falseSkip = falseSkip;
	}

	public int Evaluate(TModel model)
	{
		return this.predicate?.Invoke(model) is true ? this.trueSkip : this.falseSkip;
	}

	[Pure]
	public bool Equals([NotNullWhen(true)] SkipCondition<TModel>? other)
	{
		return other is not null &&
			(this.predicate?.Equals(other.predicate) ?? other.predicate is null) &&
			this.trueSkip == other.trueSkip &&
			this.falseSkip == other.falseSkip;
	}

	[Pure]
	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		return obj is SkipCondition<TModel> other && Equals(other);
	}

	[Pure]
	public override int GetHashCode()
	{
		return HashCode.Combine(this.predicate, this.trueSkip, this.falseSkip);
	}

	public static bool operator ==(SkipCondition<TModel>? left, SkipCondition<TModel>? right)
	{
		return left?.Equals(right) ?? right is null;
	}

	public static bool operator !=(SkipCondition<TModel>? left, SkipCondition<TModel>? right)
	{
		return !(left == right);
	}
}
