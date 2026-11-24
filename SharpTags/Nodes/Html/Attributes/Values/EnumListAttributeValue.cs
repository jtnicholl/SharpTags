// SPDX-License-Identifier: LGPL-3.0-or-later

namespace SharpTags.Nodes.Html.Attributes.Values;

/// <summary>
/// Union of possible values for an enumerated list HTML attribute.
/// </summary>
/// <typeparam name="TEnum">The type of enum.</typeparam>
public readonly union EnumListAttributeValue<TEnum>(
	TEnum,
	IEnumerable<TEnum>
) where TEnum : struct, Enum
{
	/// <summary>
	/// This <see cref="EnumListAttributeValue{TEnum}" /> as an <see cref="IEnumerable{T}" />.
	/// </summary>
	public IEnumerable<TEnum>? Enumerable
	{
		get
		{
			return this switch
			{
				TEnum e => !e.Equals(default(TEnum)) ? [e] : null,
				IEnumerable<TEnum> e => e,
				null => null,
			};
		}
	}
}
