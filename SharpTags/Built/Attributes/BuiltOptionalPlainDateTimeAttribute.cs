// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using SharpTags.Nodes.Html.Attributes;

namespace SharpTags.Built.Attributes;

internal sealed class BuiltOptionalPlainDateTimeAttribute<TModel> : IBuiltSegment<TModel>
{
	private readonly OptionalPlainDateTimeFunction<TModel> function;

	public BuiltOptionalPlainDateTimeAttribute(OptionalPlainDateTimeFunction<TModel> function)
	{
		this.function = function;
	}

	public IEnumerable<ReadOnlyMemory<byte>> Evaluate(TModel model)
	{
		Contract.Assert(this.function is not null);
		return this.function switch
		{
			Func<TModel, DateTime?> f => f.Invoke(model) is { } dt
				?
				[
					AttributeNames.DateTime,
					new PlainText(dt).Utf8Bytes,
					AttributeNames.DateTime[^1..],
				]
				: [],
			Func<TModel, DateOnly?> f => f.Invoke(model) is { } dt
				?
				[
					AttributeNames.DateTime,
					new PlainText(dt).Utf8Bytes,
					AttributeNames.DateTime[^1..],
				]
				: [],
		};
	}

	[Pure]
	public bool Equals([NotNullWhen(true)] BuiltOptionalPlainDateTimeAttribute<TModel>? other)
	{
		Contract.Assert(this.function.Value is not null);
		return other is not null && this.function.Value.Equals(other.function.Value);
	}

	[Pure]
	public bool Equals([NotNullWhen(true)] IBuiltSegment<TModel>? obj)
	{
		return obj is BuiltOptionalPlainDateTimeAttribute<TModel> other && Equals(other);
	}

	[Pure]
	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		return obj is BuiltOptionalPlainDateTimeAttribute<TModel> other && Equals(other);
	}

	[Pure]
	public override int GetHashCode()
	{
		Contract.Assert(this.function.Value is not null);
		return this.function.Value.GetHashCode();
	}

	public static bool operator ==(
		BuiltOptionalPlainDateTimeAttribute<TModel>? left,
		BuiltOptionalPlainDateTimeAttribute<TModel>? right
	)
	{
		return left?.Equals(right) ?? right is null;
	}

	public static bool operator !=(
		BuiltOptionalPlainDateTimeAttribute<TModel>? left,
		BuiltOptionalPlainDateTimeAttribute<TModel>? right
	)
	{
		return !(left == right);
	}
}
