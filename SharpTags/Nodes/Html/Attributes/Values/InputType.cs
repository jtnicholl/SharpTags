// SPDX-License-Identifier: LGPL-3.0-or-later

using SharpTags.Nodes.Html.Elements;

namespace SharpTags.Nodes.Html.Attributes.Values;

/// <summary>
/// The different types for an <see cref="Input{TModel}" />.
/// </summary>
/// <seealso href="https://developer.mozilla.org/en-US/docs/Web/HTML/Reference/Elements/input#input_types" />
public enum InputType
{
	/// <summary>
	/// Single-line text input, the default if no type is specified.
	/// </summary>
	/// <seealso href="https://developer.mozilla.org/en-US/docs/Web/HTML/Reference/Elements/input/text" />
	Text,

	/// <summary>
	/// A button with no default behavior.
	/// </summary>
	/// <remarks>
	/// Using a <see cref="Button{TModel}" /> with a
	/// <see cref="Button{TModel}.Type" /> of <see cref="ButtonType.Button" />
	/// is typically recommended instead.
	/// </remarks>
	/// <seealso href="https://developer.mozilla.org/en-US/docs/Web/HTML/Reference/Elements/input/button" />
	Button,

	/// <summary>
	/// A checkbox.
	/// </summary>
	/// <seealso href="https://developer.mozilla.org/en-US/docs/Web/HTML/Reference/Elements/input/checkbox" />
	Checkbox,

	/// <summary>
	/// A color selector, usually provided by the operating system.
	/// </summary>
	/// <seealso href="https://developer.mozilla.org/en-US/docs/Web/HTML/Reference/Elements/input/color" />
	Color,

	/// <summary>
	/// A calendar date without time or time zone information.
	/// </summary>
	/// <seealso href="https://developer.mozilla.org/en-US/docs/Web/HTML/Reference/Elements/input/date" />
	PlainDate,

	/// <summary>
	/// A calendar date and time of day, with one-minute precision, with no time
	/// zone information.
	/// </summary>
	/// <remarks>
	/// The default precision is one minute, but it can be overridden using
	/// <see cref="Input{TModel}.Step" /> for a custom precision, up to one
	/// millisecond.
	/// </remarks>
	/// <seealso href="https://developer.mozilla.org/en-US/docs/Web/HTML/Reference/Elements/input/datetime-local" />
	PlainDateTime,

	/// <summary>
	/// An email address, with validation.
	/// </summary>
	/// <seealso href="https://developer.mozilla.org/en-US/docs/Web/HTML/Reference/Elements/input/email" />
	Email,

	/// <summary>
	/// Allows for file uploads.
	/// </summary>
	/// <seealso href="https://developer.mozilla.org/en-US/docs/Web/HTML/Reference/Elements/input/file" />
	File,

	/// <summary>
	/// A hidden input which cannot be seen nor modified, typically pre-populated on page load.
	/// </summary>
	/// <seealso href="https://developer.mozilla.org/en-US/docs/Web/HTML/Reference/Elements/input/hidden" />
	Hidden,

	/// <summary>
	/// A submit button with a custom image as its appearance.
	/// </summary>
	/// <seealso href="https://developer.mozilla.org/en-US/docs/Web/HTML/Reference/Elements/input/image" />
	Image,

	/// <summary>
	/// A calendar year and month without day, time, or time zone information.
	/// </summary>
	/// <seealso href="https://developer.mozilla.org/en-US/docs/Web/HTML/Reference/Elements/input/month" />
	PlainMonth,

	/// <summary>
	/// Allows inputting numbers, which may or may not be integers depending on
	/// the value of <see cref="Input{TModel}.Step" />.
	/// </summary>
	/// <seealso href="https://developer.mozilla.org/en-US/docs/Web/HTML/Reference/Elements/input/number" />
	Number,

	/// <summary>
	/// Single-line text input obscuring the text. Browsers often add additional
	/// restrictions, such as not allowing text to be copied out.
	/// </summary>
	/// <seealso href="https://developer.mozilla.org/en-US/docs/Web/HTML/Reference/Elements/input/password" />
	Password,

	/// <summary>
	/// Allows selecting one and only one item out of a group, grouped by their
	/// <see cref="Input{TModel}.Name" />.
	/// </summary>
	/// <seealso href="https://developer.mozilla.org/en-US/docs/Web/HTML/Reference/Elements/input/radio" />
	Radio,

	/// <summary>
	/// Allows inputting a number within a range, typically presented as a slider.
	/// </summary>
	/// <seealso href="https://developer.mozilla.org/en-US/docs/Web/HTML/Reference/Elements/input/range" />
	Range,

	/// <summary>
	/// A button which resets all other <see cref="Input{TModel}" /> in its
	/// containing <see cref="Form{TModel}" /> to their original values.
	/// </summary>
	/// <seealso href="https://developer.mozilla.org/en-US/docs/Web/HTML/Reference/Elements/input/reset" />
	Reset,

	/// <summary>
	/// Single-line text input specifically intended for a search query.
	/// </summary>
	/// <seealso href="https://developer.mozilla.org/en-US/docs/Web/HTML/Reference/Elements/input/search" />
	Search,

	/// <summary>
	/// A button to submit the <see cref="Form{TModel}" />.
	/// </summary>
	/// <remarks>
	/// Using a <see cref="Button{TModel}" /> with a
	/// <see cref="Button{TModel}.Type" /> of <see cref="ButtonType.Submit" />
	/// is typically recommended instead.
	/// </remarks>
	/// <seealso href="https://developer.mozilla.org/en-US/docs/Web/HTML/Reference/Elements/input/submit" />
	Submit,

	/// <summary>
	/// Single-line text input specifically intended for a phone number.
	/// </summary>
	/// <remarks>
	/// Due to the variability of phone number formats in different regions,
	/// this does not provide validation. If needed, implement your own, such as
	/// with <see cref="Input{TModel}.Pattern" />.
	/// </remarks>
	/// <seealso href="https://developer.mozilla.org/en-US/docs/Web/HTML/Reference/Elements/input/tel" />
	Telephone,

	/// <summary>
	/// Allows inputting a time of day, without any date or time zone information.
	/// </summary>
	/// <remarks>
	/// The default precision is one minute, but it can be overridden using
	/// <see cref="Input{TModel}.Step" /> for a custom precision, up to one
	/// millisecond.
	/// </remarks>
	/// <seealso href="https://developer.mozilla.org/en-US/docs/Web/HTML/Reference/Elements/input/time" />
	PlainTime,

	/// <summary>
	/// Single-line input for a URL, with validation.
	/// </summary>
	/// <seealso href="https://developer.mozilla.org/en-US/docs/Web/HTML/Reference/Elements/input/url" />
	Url,

	/// <summary>
	/// Input for a year and an ISO 8601 week number, with no time zone information.
	/// </summary>
	/// <seealso href="https://developer.mozilla.org/en-US/docs/Web/HTML/Reference/Elements/input/week" />
	PlainWeek,
}
