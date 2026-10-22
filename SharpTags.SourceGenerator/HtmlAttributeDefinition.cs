// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Text;

namespace SharpTags.SourceGenerator;

internal sealed record HtmlAttributeDefinition
{
	/// <summary>
	/// Full, descriptive name of the attribute in Pascal-case.
	/// </summary>
	public required string Name { get; init; }

	/// <summary>
	/// A Pascal-case name that, when converted to lowercase, is the actual name
	/// used for this attribute in HTML.<br />
	/// If absent, <see cref="Name" /> is used instead.
	/// </summary>
	public string? Attribute { get; init; }

	/// <summary>
	/// The type of attribute, which defines what values it can have.
	/// </summary>
	public HtmlAttributeType Type { get; init; }

	/// <summary>
	/// The name of the C# enum type to use when <see cref="Type" /> is
	/// <see cref="HtmlAttributeType.Enum" /> or
	/// <see cref="HtmlAttributeType.EnumList" />.<br />
	/// If absent, <see cref="Name" /> is used instead.
	/// </summary>
	public string? Enum { get; init; }

	/// <summary>
	/// The separator used for multiple values when <see cref="Type" /> is
	/// <see cref="HtmlAttributeType.List" /> or
	/// <see cref="HtmlAttributeType.EnumList" />.
	/// </summary>
	public ListSeparator List { get; init; }

	/// <summary>
	/// Provides a summary for the C# XML doc comments.
	/// </summary>
	public string? Summary { get; init; }

	internal byte[] GetClassName()
	{
		return Type switch
		{
			HtmlAttributeType.String => "StringAttribute<TModel>"u8.ToArray(),
			HtmlAttributeType.Enum =>
				Encoding.UTF8.GetBytes($"EnumAttribute<TModel, {Enum ?? Name}, {Enum ?? Name}Attribute>"),
			HtmlAttributeType.Bool => "BooleanAttribute<TModel>"u8.ToArray(),
			HtmlAttributeType.Ternary =>
				Encoding.UTF8.GetBytes($"TernaryAttribute<TModel, {Name}Attribute>"),
			HtmlAttributeType.Int => "IntegerAttribute<TModel>"u8.ToArray(),
			HtmlAttributeType.Number => "NumberAttribute<TModel>"u8.ToArray(),
			HtmlAttributeType.List => "ListAttribute<TModel>"u8.ToArray(),
			HtmlAttributeType.EnumList =>
				Encoding.UTF8.GetBytes($"EnumListAttribute<TModel, {Enum ?? Name}, {Enum ?? Name}Attribute>"),
			HtmlAttributeType.Char => "CharAttribute<TModel>"u8.ToArray(),
			HtmlAttributeType.Uri => "StringAttribute<TModel>"u8.ToArray(),
			HtmlAttributeType.MimeType => "StringAttribute<TModel>"u8.ToArray(),
			HtmlAttributeType.PlainDateTime => "PlainDateTimeAttribute<TModel>"u8.ToArray(),
			HtmlAttributeType.SourceDocument => "SourceDocumentAttribute<TModel>"u8.ToArray(),
			_ => throw new(),
		};
	}

	internal byte[] GetValueType()
	{
		return Type switch
		{
			HtmlAttributeType.Enum =>
			[
				.."EnumAttributeValue<TModel, global::SharpTags.Nodes.Html.Attributes.Values."u8,
				..Encoding.UTF8.GetBytes(Enum ?? Name),
				(byte)'>'
			],
			HtmlAttributeType.Bool => "BooleanAttributeValue<TModel>"u8.ToArray(),
			HtmlAttributeType.Ternary => "TernaryAttributeValue<TModel>"u8.ToArray(),
			HtmlAttributeType.Int => "IntegerAttributeValue<TModel>"u8.ToArray(),
			HtmlAttributeType.Number => "NumberAttributeValue<TModel>"u8.ToArray(),
			HtmlAttributeType.List => "ListAttributeValue<TModel>"u8.ToArray(),
			HtmlAttributeType.EnumList =>
			[
				.."EnumListAttributeValue<TModel, global::SharpTags.Nodes.Html.Attributes.Values."u8,
				..Encoding.UTF8.GetBytes(Enum ?? Name),
				..">"u8,
			],
			HtmlAttributeType.Char => "CharAttributeValue<TModel>"u8.ToArray(),
			HtmlAttributeType.PlainDateTime => "PlainDateTimeAttributeValue<TModel>"u8.ToArray(),
			HtmlAttributeType.SourceDocument => "SourceDocumentAttributeValue<TModel>"u8.ToArray(),
			_ => "StringAttributeValue<TModel>"u8.ToArray(),
		};
	}
}
