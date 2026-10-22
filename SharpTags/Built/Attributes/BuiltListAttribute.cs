// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace SharpTags.Built.Attributes;

internal sealed class BuiltListAttribute<TModel> : IBuiltSegment<TModel>
{
	private readonly ListFunction<TModel> function;
	private readonly PlainText separator;
	private readonly ReadOnlyMemory<byte> name;

	public BuiltListAttribute(
		ListFunction<TModel> function,
		PlainText separator,
		ReadOnlyMemory<byte> name
	)
	{
		Contract.Assert(name.Span.StartsWith(" "u8));
		Contract.Assert(name.Span.EndsWith("=\""u8));
		this.function = function;
		this.separator = separator;
		this.name = name;
	}

	public IEnumerable<ReadOnlyMemory<byte>> Evaluate(TModel model)
	{
		Contract.Assert(function is not null);

		var result = this.function switch
		{
			Func<TModel, IEnumerable<PlainText>?> f => f.Invoke(model),
			Func<TModel, IEnumerable<string>?> f => f.Invoke(model)?.Select(s => new PlainText(s)),
		};

		return result is null ? [] : Evaluate(this.name, this.separator.Utf8Bytes, result);
	}

	private static IEnumerable<ReadOnlyMemory<byte>> Evaluate(
		ReadOnlyMemory<byte> name,
		ReadOnlyMemory<byte> separator,
		IEnumerable<PlainText> values
	)
	{
		yield return name;
		using var enumerator = values.GetEnumerator();
		if (!enumerator.MoveNext())
		{
			yield return name[^1..];
			yield break;
		}

		yield return enumerator.Current.Utf8Bytes;
		while (enumerator.MoveNext())
		{
			yield return separator;
			yield return enumerator.Current.Utf8Bytes;
		}

		yield return name[^1..];
	}

	[Pure]
	public bool Equals([NotNullWhen(true)] BuiltListAttribute<TModel>? other)
	{
		Contract.Assert(this.function.Value is not null);
		return other is not null &&
			this.function.Value.Equals(other.function.Value) &&
			this.separator.Equals(other.separator);
	}

	[Pure]
	public bool Equals([NotNullWhen(true)] IBuiltSegment<TModel>? obj)
	{
		return obj is BuiltListAttribute<TModel> other && Equals(other);
	}

	[Pure]
	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		return obj is BuiltListAttribute<TModel> other && Equals(other);
	}

	[Pure]
	public override int GetHashCode()
	{
		HashCode output = new();

		output.Add(this.function.Value);
		output.Add(this.separator);
		output.AddBytes(this.name.Span);

		return output.ToHashCode();
	}

	public static bool operator ==(
		BuiltListAttribute<TModel>? left,
		BuiltListAttribute<TModel>? right
	)
	{
		return left?.Equals(right) ?? right is null;
	}

	public static bool operator !=(
		BuiltListAttribute<TModel>? left,
		BuiltListAttribute<TModel>? right
	)
	{
		return !(left == right);
	}
}
