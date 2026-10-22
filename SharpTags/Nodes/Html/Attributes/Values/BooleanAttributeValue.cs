// SPDX-License-Identifier: LGPL-3.0-or-later

namespace SharpTags.Nodes.Html.Attributes.Values;

/// <summary>
/// Union of possible values for a boolean HTML attribute.
/// </summary>
/// <typeparam name="TModel">The type of model, which can vary the value.</typeparam>
public union BooleanAttributeValue<TModel>(
	bool,
	Func<TModel, bool>
)
{
	/// <summary>
	/// Get the value of this <see cref="BooleanAttributeValue{TModel}" /> for
	/// the given <paramref name="value" />.
	/// </summary>
	/// <param name="value">The <typeparamref name="TModel" /> to evaluate.</param>
	/// <returns>The value of this <see cref="BooleanAttributeValue{TModel}" />.</returns>
	public readonly bool Evaluate(TModel value)
	{
		return this switch
		{
			true => true,
			false => false,
			Func<TModel, bool> f => f.Invoke(value),
			null => false,
		};
	}
}
