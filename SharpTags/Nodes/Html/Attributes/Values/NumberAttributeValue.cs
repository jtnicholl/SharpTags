// SPDX-License-Identifier: LGPL-3.0-or-later

namespace SharpTags.Nodes.Html.Attributes.Values;

/// <summary>
/// Union of possible values for a numeric HTML attribute.
/// </summary>
/// <typeparam name="TModel">The type of model, which can vary value.</typeparam>
public readonly union NumberAttributeValue<TModel>(
	double,
	int,
	Func<TModel, int>,
	Func<TModel, double>,
	Func<TModel, int?>,
	Func<TModel, double?>
)
{
	/// <summary>
	/// Get the value of this <see cref="NumberAttributeValue{TModel}" /> for
	/// the given <paramref name="value" />.
	/// </summary>
	/// <param name="value">The <typeparamref name="TModel" /> to evaluate.</param>
	/// <returns>The value of this <see cref="NumberAttributeValue{TModel}" />.</returns>
	public double? Evaluate(TModel value)
	{
		return this switch
		{
			double v => v,
			int v => v,
			Func<TModel, double> f => f.Invoke(value),
			Func<TModel, double?> f => f.Invoke(value),
			Func<TModel, int> f => f.Invoke(value),
			Func<TModel, int?> f => f.Invoke(value),
			null => null,
		};
	}
}
