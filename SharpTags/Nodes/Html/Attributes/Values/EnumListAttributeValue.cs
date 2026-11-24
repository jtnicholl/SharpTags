// SPDX-License-Identifier: LGPL-3.0-or-later

namespace SharpTags.Nodes.Html.Attributes.Values;

/// <summary>
/// Union of possible values for an enumerated list HTML attribute.
/// </summary>
/// <typeparam name="TModel">The type of model, which can vary the value.</typeparam>
/// <typeparam name="TEnum">The type of enum.</typeparam>
public readonly union EnumListAttributeValue<TModel, TEnum>(
	TEnum,
	IEnumerable<TEnum>,
	Func<TModel, TEnum>,
	Func<TModel, IEnumerable<TEnum>?>
) where TEnum : struct, Enum
{
	/// <summary>
	/// Get the value of this
	/// <see cref="EnumListAttributeValue{TModel, TEnum}" /> for the given
	/// <paramref name="value" />.
	/// </summary>
	/// <param name="value">The <typeparamref name="TModel" /> to evaluate.</param>
	/// <returns>The value of this <see cref="EnumListAttributeValue{TModel, TEnum}" />.</returns>
	public IEnumerable<TEnum>? Evaluate(TModel value)
	{
		return this switch
		{
			TEnum e => !e.Equals(default(TEnum)) ? [e] : null,
			IEnumerable<TEnum> e => e,
			Func<TModel, TEnum> f =>
				f.Invoke(value) is var e && !e.Equals(default(TEnum)) ? [e] : null,
			Func<TModel, IEnumerable<TEnum>?> f => f.Invoke(value),
			null => null,
		};
	}

	/// <summary>
	/// Implicitly convert a nullable <typeparamref name="TEnum" /> to a
	/// <see cref="EnumAttributeValue{TModel, TValue}" />.
	/// </summary>
	public static implicit operator EnumListAttributeValue<TModel, TEnum>(TEnum? value)
	{
		return value is TEnum e ? new(e) : default;
	}
}
