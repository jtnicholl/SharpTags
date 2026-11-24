// SPDX-License-Identifier: LGPL-3.0-or-later

namespace SharpTags.Nodes.Html.Attributes.Values;

/// <summary>
/// Union of possible values for an HTML attribute which can be true, false, or entirely absent.
/// </summary>
/// <typeparam name="TModel">The type of model, which can vary value.</typeparam>
public readonly union TernaryAttributeValue<TModel>(
	bool,
	Func<TModel, bool?>,
	Func<TModel, bool>
)
{
	/// <summary>
	/// Get the value of this <see cref="TernaryAttributeValue{TModel}" /> for
	/// the given <paramref name="value" />.
	/// </summary>
	/// <param name="value">The <typeparamref name="TModel" /> to evaluate.</param>
	/// <returns>The value of this <see cref="TernaryAttributeValue{TModel}" />.</returns>
	public bool? Evaluate(TModel value)
	{
		return this switch
		{
			bool b => b,
			Func<TModel, bool?> f => f.Invoke(value),
			Func<TModel, bool> f => f.Invoke(value),
			null => null,
		};
	}

	/// <summary>
	/// Implicitly convert a <see langword="bool?" /> to a
	/// <see cref="TernaryAttributeValue{TModel}" />.
	/// </summary>
	public static implicit operator TernaryAttributeValue<TModel>(bool? value)
	{
		return value is bool b ? new(b) : default;
	}
}
