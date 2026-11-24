// SPDX-License-Identifier: LGPL-3.0-or-later

namespace SharpTags.Nodes.Html.Attributes.Values;

/// <summary>
/// Union of possible values for an enumerated HTML attribute.
/// </summary>
/// <typeparam name="TModel">The type of model, which can vary the value.</typeparam>
/// <typeparam name="TEnum">The type of enum.</typeparam>
public readonly union EnumAttributeValue<TModel, TEnum>(
	TEnum,
	Func<TModel, TEnum>
) where TEnum : struct, Enum
{
	/// <summary>
	/// Get the value of this <see cref="EnumAttributeValue{TModel, TValue}" />
	/// for the given <paramref name="value" />.
	/// </summary>
	/// <param name="value">The <typeparamref name="TModel" /> to evaluate.</param>
	/// <returns>The value of this <see cref="EnumAttributeValue{TModel, TValue}" />.</returns>
	public TEnum Evaluate(TModel value)
	{
		return this switch
		{
			TEnum v => v,
			Func<TModel, TEnum> f => f.Invoke(value),
			null => default,
		};
	}

	/// <summary>
	/// Implicitly convert a nullable <typeparamref name="TEnum" /> to a
	/// <see cref="EnumAttributeValue{TModel, TValue}" />.
	/// </summary>
	public static implicit operator EnumAttributeValue<TModel, TEnum>(TEnum? value)
	{
		return value is TEnum e ? new(e) : default;
	}
}
