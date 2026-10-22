// SPDX-License-Identifier: LGPL-3.0-or-later

namespace SharpTags.Nodes.Html.Attributes.Values;

/// <summary>
/// Union of possible values for a string HTML attribute.
/// </summary>
/// <typeparam name="TModel">The type of model, which can vary value.</typeparam>
public union StringAttributeValue<TModel>(
	PlainText,
	Func<TModel, PlainText>,
	Func<TModel, PlainText?>,
	Func<TModel, string?>
)
{
	/// <summary>
	/// Get the value of this <see cref="StringAttributeValue{TModel}" /> for
	/// the given <paramref name="value" />.
	/// </summary>
	/// <param name="value">The <typeparamref name="TModel" /> to evaluate.</param>
	/// <returns>The value of this <see cref="StringAttributeValue{TModel}" />.</returns>
	public readonly PlainText? Evaluate(TModel value)
	{
		return this switch
		{
			PlainText pt => pt,
			Func<TModel, PlainText> f => f.Invoke(value),
			Func<TModel, PlainText?> f => f.Invoke(value),
			Func<TModel, string?> f => f.Invoke(value) is string str ? new PlainText(str) : null,
			null => null,
		};
	}

	/// <summary>
	/// Implicitly convert a <see cref="string" /> to a <see cref="StringAttributeValue{TModel}" />.
	/// </summary>
	public static implicit operator StringAttributeValue<TModel>(string? value)
	{
		return value is null ? default(StringAttributeValue<TModel>) : new PlainText(value);
	}

	/// <summary>
	/// Implicitly convert a <see cref="ReadOnlySpan{T}" /> to a
	/// <see cref="StringAttributeValue{TModel}" />.
	/// </summary>
	public static implicit operator StringAttributeValue<TModel>(ReadOnlySpan<byte> value)
	{
		return new PlainText(value);
	}
}
