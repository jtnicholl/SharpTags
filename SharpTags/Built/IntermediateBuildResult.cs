// SPDX-License-Identifier: LGPL-3.0-or-later

namespace SharpTags.Built;

internal readonly union IntermediateBuildResult<TModel>(
	Range,
	IBuiltSegment<TModel>,
	SkipCondition<TModel>,
	BuiltEnumerator<TModel>
) : IEquatable<IntermediateBuildResult<TModel>>
{
	public BuiltMarkupSegment<TModel> Build(ReadOnlyMemory<byte> finalBuffer)
	{
		return this switch
		{
			Range r => finalBuffer[r],
			IBuiltSegment<TModel> b => b,
			SkipCondition<TModel> s => s,
			BuiltEnumerator<TModel> e => e,
			null => throw new InvalidOperationException(),
		};
	}

	public bool Equals(IntermediateBuildResult<TModel> other)
	{
		return this switch
		{
			Range r => r.Equals(other.Value),
			IBuiltSegment<TModel> b => b.Equals(other.Value),
			SkipCondition<TModel> s => s.Equals(other.Value),
			BuiltEnumerator<TModel> e => e.Equals(other.Value),
			null => other.Value is null,
		};
	}

	public override bool Equals(object? obj)
	{
		return (obj is IntermediateBuildResult<TModel> other && Equals(other)) ||
			(obj is null && this is null);
	}

	public override int GetHashCode()
	{
		return Value?.GetHashCode() ?? 0;
	}

	public static bool operator ==(
		IntermediateBuildResult<TModel> left,
		IntermediateBuildResult<TModel> right
	)
	{
		return left.Equals(right);
	}

	public static bool operator !=(
		IntermediateBuildResult<TModel> left,
		IntermediateBuildResult<TModel> right
	)
	{
		return !left.Equals(right);
	}
}
