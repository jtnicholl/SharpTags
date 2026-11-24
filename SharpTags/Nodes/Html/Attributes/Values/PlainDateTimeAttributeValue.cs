// SPDX-License-Identifier: LGPL-3.0-or-later

namespace SharpTags.Nodes.Html.Attributes.Values;

/// <summary>
/// Union of possible values for an HTML datetime attribute, which optionally
/// has a time, but never time zone information.
/// </summary>
public readonly union PlainDateTimeAttributeValue(
	DateTime,
	DateOnly
)
{
	/// <summary>
	/// This <see cref="PlainDateTimeAttributeValue" /> as a <see cref="PlainText" />.
	/// </summary>
	public PlainText? Text
	{
		get
		{
			return this switch
			{
				DateTime dt => dt == default ? null : new(dt),
				DateOnly d => d == default ? null : new(d),
			};
		}
	}
}
