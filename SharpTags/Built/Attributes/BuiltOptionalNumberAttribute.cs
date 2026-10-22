// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using SharpTags.Nodes.Html.Attributes;

namespace SharpTags.Built.Attributes;

internal sealed class BuiltOptionalNumberAttribute<TModel> : IBuiltSegment<TModel>
{
	private readonly ReadOnlyMemory<byte> name;
	private readonly OptionalNumberFunction<TModel> function;

	public BuiltOptionalNumberAttribute(
		ReadOnlyMemory<byte> name,
		OptionalNumberFunction<TModel> function
	)
	{
		Contract.Assert(name.Span.StartsWith(" "u8));
		Contract.Assert(name.Span.EndsWith("=\""u8));
		this.function = function;
		this.name = name;
	}

	public IEnumerable<ReadOnlyMemory<byte>> Evaluate(TModel model)
	{
		Contract.Assert(function is not null);

		AttributeResult result = this.function switch
		{
			Func<TModel, int?> f => new(f.Invoke(model)),
			Func<TModel, double?> f => new(f.Invoke(model)),
		};

		return result.HasValue ? [this.name, result.Value, this.name[^1..]] : [];
	}

	[Pure]
	public bool Equals([NotNullWhen(true)] BuiltOptionalNumberAttribute<TModel>? other)
	{
		Contract.Assert(this.function.Value is not null);
		return other is not null &&
			this.function.Value.Equals(other.function.Value) &&
			this.name.Span.SequenceEqual(other.name.Span);
	}

	[Pure]
	public bool Equals([NotNullWhen(true)] IBuiltSegment<TModel>? obj)
	{
		return obj is BuiltOptionalNumberAttribute<TModel> other && Equals(other);
	}

	[Pure]
	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		return obj is BuiltOptionalNumberAttribute<TModel> other && Equals(other);
	}

	[Pure]
	public override int GetHashCode()
	{
		HashCode output = new();

		output.Add(this.function.Value);
		output.AddBytes(this.name.Span);

		return output.ToHashCode();
	}

	public static bool operator ==(
		BuiltOptionalNumberAttribute<TModel>? left,
		BuiltOptionalNumberAttribute<TModel>? right
	)
	{
		return left?.Equals(right) ?? right is null;
	}

	public static bool operator !=(
		BuiltOptionalNumberAttribute<TModel>? left,
		BuiltOptionalNumberAttribute<TModel>? right
	)
	{
		return !(left == right);
	}
}
