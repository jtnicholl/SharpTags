// SPDX-License-Identifier: LGPL-3.0-or-later

namespace SharpTags.Nodes.Html.Attributes.Values;

/// <summary>
/// Union of possible values for an integer HTML attribute.
/// </summary>
/// <typeparam name="TModel">The type of model, which can vary value.</typeparam>
public readonly union IntegerAttributeValue<TModel>(
	int,
	Func<TModel, int>,
	Func<TModel, int?>
)
{
	/// <summary>
	/// Get the value of this <see cref="IntegerAttributeValue{TModel}" /> for
	/// the given <paramref name="value" />.
	/// </summary>
	/// <param name="value">The <typeparamref name="TModel" /> to evaluate.</param>
	/// <returns>The value of this <see cref="IntegerAttributeValue{TModel}" />.</returns>
	public int? Evaluate(TModel value)
	{
		return this switch
		{
			int v => v,
			Func<TModel, int> f => f.Invoke(value),
			Func<TModel, int?> f => f.Invoke(value),
			null => null,
		};
	}

	/// <summary>
	/// Implicitly convert a nullable <see langword="int" /> to a
	/// <see cref="IntegerAttributeValue{TModel}" />.
	/// </summary>
	public static implicit operator IntegerAttributeValue<TModel>(int? value)
	{
		return value is int i ? new(i) : default;
	}
}
