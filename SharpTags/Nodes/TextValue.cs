// SPDX-License-Identifier: LGPL-3.0-or-later

namespace SharpTags.Nodes;

/// <summary>
/// A value for plain text within an XML or HTML document.
/// </summary>
/// <typeparam name="TModel">
/// The type of model, which can be used to vary rendered output.
/// </typeparam>
public union TextValue<TModel>(
	PlainText,
	Func<TModel, PlainText>,
	Func<TModel, string?>
)
{
	/// <summary>
	/// Evaluate the content of this <see cref="TextValue{TModel}" />.
	/// </summary>
	/// <param name="model">The <typeparamref name="TModel" /> to vary the text based on.</param>
	/// <returns>The <see cref="PlainText" />.</returns>
	public readonly PlainText Evaluate(TModel model)
	{
		return this switch
		{
			PlainText pt => pt,
			Func<TModel, PlainText> f => f.Invoke(model),
			Func<TModel, string?> f => f.Invoke(model),
			null => PlainText.Empty,
		};
	}

	/// <summary>
	/// Implicitly convert from a <see cref="string" />.
	/// </summary>
	public static implicit operator TextValue<TModel>(string? text)
	{
		return text != null ? new PlainText(text) : (TextValue<TModel>)default;
	}
}
