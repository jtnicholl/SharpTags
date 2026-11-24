// SPDX-License-Identifier: LGPL-3.0-or-later

namespace SharpTags.Nodes.Html.Attributes.Values;

/// <summary>
/// Union type for lists of text strings.
/// </summary>
public readonly union ListAttributeValue<TModel>(
	PlainText,
	IEnumerable<PlainText>,
	IEnumerable<string>,
	Func<TModel, PlainText>,
	Func<TModel, PlainText?>,
	Func<TModel, IEnumerable<PlainText>?>,
	Func<TModel, string?>,
	Func<TModel, IEnumerable<string>?>
)
{
	/// <summary>
	/// Get the value of this <see cref="ListAttributeValue{TModel}" /> for the
	/// given <paramref name="value" />.
	/// </summary>
	/// <param name="value">The <typeparamref name="TModel" /> to evaluate.</param>
	/// <returns>The value of this <see cref="ListAttributeValue{TModel}" />.</returns>
	public IEnumerable<PlainText>? Evaluate(TModel value)
	{
		return this switch
		{
			PlainText pt => [pt],
			IEnumerable<PlainText> ie => ie,
			IEnumerable<string> ie => ie.Select(s => new PlainText(s)),
			Func<TModel, PlainText> f => [f.Invoke(value)],
			Func<TModel, PlainText?> f => f.Invoke(value) is PlainText t ? [t] : null,
			Func<TModel, IEnumerable<PlainText>?> f => f.Invoke(value),
			Func<TModel, string?> f => f.Invoke(value) is string s ? [new(s)] : null,
			Func<TModel, IEnumerable<string>?> f => f.Invoke(value)?.Select(s => new PlainText(s)),
			null => null,
		};
	}

	/// <summary>
	/// Implicitly convert a <see cref="string" /> to a <see cref="ListAttributeValue{TModel}" />.
	/// </summary>
	public static implicit operator ListAttributeValue<TModel>(string? value)
	{
		return value is null ? default(ListAttributeValue<TModel>) : new PlainText(value);
	}

	/// <summary>
	/// Implicitly convert a <see cref="ReadOnlySpan{T}" /> to a
	/// <see cref="ListAttributeValue{TModel}" />.
	/// </summary>
	public static implicit operator ListAttributeValue<TModel>(ReadOnlySpan<byte> value)
	{
		return new PlainText(value);
	}

	/// <summary>
	/// Implicitly convert a nullable <see cref="PlainText" /> to a
	/// <see cref="ListAttributeValue{TModel}" />.
	/// </summary>
	public static implicit operator ListAttributeValue<TModel>(PlainText? value)
	{
		return value is PlainText pt ? new(pt) : default;
	}

	/// <summary>
	/// Implicitly convert a <see cref="ListAttributeValue" /> to a
	/// <see cref="ListAttributeValue{TModel}" />.
	/// </summary>
	public static implicit operator ListAttributeValue<TModel>(ListAttributeValue value)
	{
		return value switch
		{
			PlainText pt => new(pt),
			IEnumerable<PlainText> ie => new(ie),
			IEnumerable<string> ie => new(ie),
			null => default,
		};
	}
}
