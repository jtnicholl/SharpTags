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
	/// A Pascal-cased name that, when converted to lowercase, is the actual name
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

	/// <summary>
	/// Returns the name of the <c>HtmlAttribute&lt;TModel&gt;</c>
	/// implementation in UTF-8 with its fully qualified namespace.
	/// </summary>
	public byte[] GetClassName()
	{
		return
		[
			.."global::SharpTags.Nodes.Html.Attributes."u8,
			..(byte[])(Type switch
			{
				HtmlAttributeType.String => [.."StringAttribute<TModel>"u8],
				HtmlAttributeType.Enum =>
				[
					.."EnumAttribute<TModel, "u8,
					.."global::SharpTags.Nodes.Html.Attributes.Values."u8,
					..Encoding.UTF8.GetBytes(Enum ?? Name),
					..", "u8,
					.."global::SharpTags.Nodes.Html.Attributes."u8,
					..Encoding.UTF8.GetBytes(Enum ?? Name),
					.."Attribute>"u8,
				],
				HtmlAttributeType.Bool => [.."BooleanAttribute<TModel>"u8],
				HtmlAttributeType.Ternary =>
				[
					.."TernaryAttribute<TModel, "u8,
					.."global::SharpTags.Nodes.Html.Attributes."u8,
					..Encoding.UTF8.GetBytes(Name),
					.."Attribute>"u8,
				],
				HtmlAttributeType.Int => [.."IntegerAttribute<TModel>"u8],
				HtmlAttributeType.Number => [.."NumberAttribute<TModel>"u8],
				HtmlAttributeType.List => [.."ListAttribute<TModel>"u8],
				HtmlAttributeType.EnumList =>
				[
					.."EnumListAttribute<TModel, "u8,
					.."global::SharpTags.Nodes.Html.Attributes.Values."u8,
					..Encoding.UTF8.GetBytes(Enum ?? Name),
					..", "u8,
					.."global::SharpTags.Nodes.Html.Attributes."u8,
					..Encoding.UTF8.GetBytes(Enum ?? Name),
					.."Attribute>"u8,
				],
				HtmlAttributeType.Char => [.."CharAttribute<TModel>"u8],
				HtmlAttributeType.Uri => [.."StringAttribute<TModel>"u8],
				HtmlAttributeType.MimeType => [.."StringAttribute<TModel>"u8],
				HtmlAttributeType.PlainDateTime => [.."PlainDateTimeAttribute<TModel>"u8],
				HtmlAttributeType.SourceDocument => [.."SourceDocumentAttribute<TModel>"u8],
				_ => throw new(),
			}),
		];
	}

	public byte[] GetGenericValueType()
	{
		return Type switch
		{
			HtmlAttributeType.Enum =>
			[
				.."global::SharpTags.Nodes.Html.Attributes.Values.EnumAttributeValue"u8,
				.."<TModel, global::SharpTags.Nodes.Html.Attributes.Values."u8,
				..Encoding.UTF8.GetBytes(Enum ?? Name),
				(byte)'>',
			],
			HtmlAttributeType.Bool =>
			[
				.."global::SharpTags.Nodes.Html.Attributes.Values.BooleanAttributeValue<TModel>"u8,
			],
			HtmlAttributeType.Ternary =>
			[
				.."global::SharpTags.Nodes.Html.Attributes.Values.TernaryAttributeValue<TModel>"u8,
			],
			HtmlAttributeType.Int =>
			[
				.."global::SharpTags.Nodes.Html.Attributes.Values.IntegerAttributeValue<TModel>"u8,
			],
			HtmlAttributeType.Number =>
			[
				.."global::SharpTags.Nodes.Html.Attributes.Values.NumberAttributeValue<TModel>"u8,
			],
			HtmlAttributeType.List =>
			[
				.."global::SharpTags.Nodes.Html.Attributes.Values.ListAttributeValue<TModel>"u8,
			],
			HtmlAttributeType.EnumList =>
			[
				.."global::SharpTags.Nodes.Html.Attributes.Values.EnumListAttributeValue"u8,
				.."<TModel, global::SharpTags.Nodes.Html.Attributes.Values."u8,
				..Encoding.UTF8.GetBytes(Enum ?? Name),
				..">"u8,
			],
			HtmlAttributeType.Char =>
			[
				.."global::SharpTags.Nodes.Html.Attributes.Values.CharAttributeValue<TModel>"u8,
			],
			HtmlAttributeType.PlainDateTime =>
			[
				.."global::SharpTags.Nodes.Html.Attributes.Values."u8,
				.."PlainDateTimeAttributeValue<TModel>"u8,
			],
			HtmlAttributeType.SourceDocument =>
			[
				.."global::SharpTags.Nodes.Html.Attributes.Values."u8,
				.."SourceDocumentAttributeValue<TModel>"u8,
			],
			_ =>
			[
				.."global::SharpTags.Nodes.Html.Attributes.Values.StringAttributeValue<TModel>"u8,
			],
		};
	}

	public byte[] GetNonGenericValueType()
	{
		return Type switch
		{
			HtmlAttributeType.Enum =>
			[
				.."global::SharpTags.Nodes.Html.Attributes.Values."u8,
				..Encoding.UTF8.GetBytes(Enum ?? Name),
			],
			HtmlAttributeType.Bool => [.."bool"u8],
			HtmlAttributeType.Ternary => [.."bool?"u8],
			HtmlAttributeType.Int => [.."int?"u8],
			HtmlAttributeType.Number => [.."double?"u8],
			HtmlAttributeType.List =>
			[
				.."global::SharpTags.Nodes.Html.Attributes.Values.ListAttributeValue"u8,
			],
			HtmlAttributeType.EnumList =>
			[
				.."global::SharpTags.Nodes.Html.Attributes.Values.EnumListAttributeValue"u8,
				.."<global::SharpTags.Nodes.Html.Attributes.Values."u8,
				..Encoding.UTF8.GetBytes(Enum ?? Name),
				..">"u8,
			],
			HtmlAttributeType.Char => [.."char?"u8],
			HtmlAttributeType.PlainDateTime =>
			[
				.."global::SharpTags.Nodes.Html.Attributes.Values.PlainDateTimeAttributeValue"u8,
			],
			_ => [.."global::SharpTags.PlainText?"u8],
		};
	}
}
