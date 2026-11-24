// SPDX-License-Identifier: LGPL-3.0-or-later

namespace SharpTags.Nodes.Html.Attributes.Values;

/// <summary>
/// Union of possible values for an HTML datetime attribute, which optionally
/// has a time, but never time zone information.
/// </summary>
/// <typeparam name="TModel">The type of model, which can vary value.</typeparam>
public readonly union PlainDateTimeAttributeValue<TModel>(
	DateTime,
	DateOnly,
	Func<TModel, DateTime>,
	Func<TModel, DateOnly>,
	Func<TModel, DateTime?>,
	Func<TModel, DateOnly?>
)
{
	/// <summary>
	/// Get the value of this <see cref="PlainDateTimeAttributeValue{TModel}" />
	/// for the given <paramref name="value" />.
	/// </summary>
	/// <param name="value">The <typeparamref name="TModel" /> to evaluate.</param>
	/// <returns>The value of this <see cref="PlainDateTimeAttributeValue{TModel}" />.</returns>
	public PlainText? Evaluate(TModel value)
	{
		return this switch
		{
			DateTime dt => dt == default ? null : new(dt),
			DateOnly d => d == default ? null : new(d),
			Func<TModel, DateTime> f => new(f.Invoke(value)),
			Func<TModel, DateOnly> f => new(f.Invoke(value)),
			Func<TModel, DateTime?> f => f.Invoke(value) is DateTime dt ? new(dt) : null,
			Func<TModel, DateOnly?> f => f.Invoke(value) is DateOnly d ? new(d) : null,
			null => null,
		};
	}

	/// <summary>
	/// Implicitly convert a <see cref="PlainDateTimeAttributeValue" /> to a
	/// typed <see cref="PlainDateTimeAttributeValue{TModel}" />.
	/// </summary>
	public static implicit operator PlainDateTimeAttributeValue<TModel>(
		PlainDateTimeAttributeValue value
	)
	{
		return value switch
		{
			DateTime dt => dt == default ? default(PlainDateTimeAttributeValue<TModel>) : dt,
			DateOnly d => d == default ? default(PlainDateTimeAttributeValue<TModel>) : d,
		};
	}
}
