// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using SharpTags.Nodes.Html.Attributes;

namespace SharpTags.Built.Attributes;

internal sealed class BuiltStringAttribute<TModel> : IBuiltSegment<TModel>
{
	private readonly ReadOnlyMemory<byte> name;
	private readonly OptionalTextFunction<TModel> function;

	public BuiltStringAttribute(
		ReadOnlyMemory<byte> name,
		OptionalTextFunction<TModel> function
	)
	{
		Contract.Assert(name.Span.StartsWith(" "u8));
		Contract.Assert(name.Span.EndsWith("=\""u8));
		this.name = name;
		this.function = function;
	}

	public IEnumerable<ReadOnlyMemory<byte>> Evaluate(TModel model)
	{
		Contract.Assert(function is not null);

		AttributeResult value = this.function switch
		{
			Func<TModel, PlainText?> f => f.Invoke(model) ?? AttributeResult.NotPresent,
			Func<TModel, string?> f => f.Invoke(model) is string str
				? new PlainText(str)
				: AttributeResult.NotPresent,
		};

		return value.HasValue ? [this.name, value.Value, this.name[^1..]] : [];
	}

	[Pure]
	public bool Equals([NotNullWhen(true)] BuiltStringAttribute<TModel>? other)
	{
		Contract.Assert(this.function.Value is not null);
		return other is not null &&
			this.function.Value.Equals(other.function.Value) &&
			this.name.Span.SequenceEqual(other.name.Span);
	}

	[Pure]
	public bool Equals([NotNullWhen(true)] IBuiltSegment<TModel>? obj)
	{
		return obj is BuiltStringAttribute<TModel> other && Equals(other);
	}

	[Pure]
	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		return obj is BuiltStringAttribute<TModel> other && Equals(other);
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
		BuiltStringAttribute<TModel>? left,
		BuiltStringAttribute<TModel>? right
	)
	{
		return left?.Equals(right) ?? right is null;
	}

	public static bool operator !=(
		BuiltStringAttribute<TModel>? left,
		BuiltStringAttribute<TModel>? right
	)
	{
		return !(left == right);
	}
}
