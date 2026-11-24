// SPDX-License-Identifier: LGPL-3.0-or-later

namespace SharpTags.Nodes.Html.Attributes.Values;

/// <summary>
/// Union of possible values for a single-character HTML attribute.
/// </summary>
/// <typeparam name="TModel">The type of model, which can vary the value.</typeparam>
public readonly union CharAttributeValue<TModel>(
	char,
	Func<TModel, char>,
	Func<TModel, char?>
)
{
	/// <summary>
	/// Get the value of this <see cref="CharAttributeValue{TModel}" /> for the
	/// given <paramref name="value" />.
	/// </summary>
	/// <param name="value">The <typeparamref name="TModel" /> to evaluate.</param>
	/// <returns>The value of this <see cref="CharAttributeValue{TModel}" />.</returns>
	public char? Evaluate(TModel value)
	{
		var output = this switch
		{
			char c => c,
			Func<TModel, char> f => f.Invoke(value),
			Func<TModel, char?> f => f.Invoke(value),
			null => null,
		};
		return output == '\0' ? null : output;
	}

	/// <summary>
	/// Implicitly convert a <see langword="char?" /> to a
	/// <see cref="CharAttributeValue{TModel}" />.
	/// </summary>
	public static implicit operator CharAttributeValue<TModel>(char? value)
	{
		return value is char c ? new(c) : default;
	}
}
