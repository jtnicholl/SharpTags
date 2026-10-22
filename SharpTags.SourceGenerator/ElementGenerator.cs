// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Text;

namespace SharpTags.SourceGenerator;

internal sealed class ElementGenerator
{
	private static readonly ReadOnlyMemory<byte>[] usings =
	[
		"SharpTags.Nodes.Html.Attributes"u8.ToArray(),
		"SharpTags.Nodes.Html.Attributes.Values"u8.ToArray(),
	];
	private static readonly byte[] elementsNamespace =
	[
		.."SharpTags.Nodes.Html.Elements"u8,
	];

	private static readonly byte[] seeAlsoPrefix =
	[
		.."https://developer.mozilla.org/en-US/docs/Web/HTML/Reference/Elements/"u8,
	];
	private static readonly byte[] globalSeeAlsoPrefix =
	[
		.."https://developer.mozilla.org/en-US/docs/Web/HTML/Reference/Global_attributes/"u8,
	];

	private readonly IReadOnlyList<HtmlAttributeDefinition> globalAttributes;
	private readonly string directory;

	public ElementGenerator(
		IReadOnlyList<HtmlAttributeDefinition> globalAttributes,
		string projectPath
	)
	{
		this.globalAttributes = globalAttributes;
		this.directory = Path.Join(projectPath, "Nodes", "Html", "Elements");
	}

	public async Task WriteBaseHtmlElement()
	{
		var elementPath = Path.Join(this.directory, "HtmlElement.gen.cs");
		await using var fileStream = File.Open(elementPath, FileMode.Create, FileAccess.Write);
		await using var generator = new SourceGenerator(fileStream, "\t"u8.ToArray());

		await generator.WriteHeader();
		await generator.WriteUsings(usings);
		await generator.DeclareNamespace(elementsNamespace);

		await generator.Write(
			"public closed partial class HtmlElement<TModel>"u8.ToArray()
		);

		await using var _ = await generator.Block();
		foreach (var attribute in this.globalAttributes)
		{
			await WriteGlobalAttributeProperty(generator, attribute);
		}
	}

	public async Task WriteHtmlElement(HtmlElementDefinition element)
	{
		var elementPath = Path.Join(this.directory, $"{element.Name}.gen.cs");
		var name = Encoding.UTF8.GetBytes(element.Name);
		await using var fileStream = File.Open(elementPath, FileMode.Create, FileAccess.Write);
		await using var generator = new SourceGenerator(fileStream, "\t"u8.ToArray());

		await generator.WriteHeader();
		await generator.WriteUsings(usings);
		await generator.DeclareNamespace(elementsNamespace);

		if (!string.IsNullOrEmpty(element.Summary))
		{
			await generator.WriteSummary(element.Summary);
		}
		await generator.SeeAlsoUrl(
			seeAlsoPrefix,
			Encoding.UTF8.GetBytes((element.Element ?? element.Name).ToLower())
		);

		await generator.Write("public sealed partial class ");
		await generator.Write(name);
		await generator.Write("<TModel> : HtmlElement<TModel>");

		await using var _ = await generator.Block();
		foreach (var attribute in element.Attributes ?? [])
		{
			await WriteHtmlAttributeProperty(generator, element, attribute);
		}

		await generator.Write(
			"private protected override IEnumerable<HtmlAttribute<TModel>> HtmlAttributes"
		);
		await using (await generator.Block())
		{
			await generator.Write("get");
			await using (await generator.Block())
			{
				foreach (var attribute in (element.Attributes ?? [])
					.Concat(this.globalAttributes)
					.OrderBy(a => a.Attribute ?? a.Name)
				)
				{
					await WriteAttributeYield(generator, attribute);
				}
			}
		}
		await generator.NewLine();

		await generator.WriteSummary(
			"Construct a new <see cref=\""u8.ToArray(),
			name,
			"{TModel}\" />."u8.ToArray()
		);
		await generator.Write("public ");
		await generator.Write(name);
		if (element.Void)
		{
			await generator.Write("() : base(ElementNames.");
			await generator.Write(name);
			await generator.Write(", true, new VoidElementChildren<TModel>(ElementNames.");
			await generator.Write(name);
			await generator.Write(")) { }");
		}
		else
		{
			await generator.Write("(params ReadOnlySpan<");
			await generator.Write("MarkupNode<TModel>> children) : base(ElementNames.");
			await generator.Write(name);
			await generator.Write(", false, children.ToList()) { }");

			await generator.NewLine();
			await generator.NewLine();

			await generator.WriteSummary(
				"Construct a new <see cref=\""u8.ToArray(),
				name,
				"{TModel}\" />."u8.ToArray()
			);
			await generator.Write("public ");
			await generator.Write(name);
			await generator.Write("(IEnumerable<");
			await generator.Write("MarkupNode<TModel>> children) : base(ElementNames.");
			await generator.Write(name);
			await generator.Write(", false, children.ToList()) { }");
		}
	}

	private static async Task WriteGlobalAttributeProperty(
		SourceGenerator writeTo,
		HtmlAttributeDefinition attribute
	)
	{
		if (!string.IsNullOrEmpty(attribute.Summary))
		{
			await writeTo.WriteSummary(Encoding.UTF8.GetBytes(attribute.Summary));
		}
		await writeTo.SeeAlsoUrl(
			globalSeeAlsoPrefix,
			Encoding.UTF8.GetBytes((attribute.Attribute ?? attribute.Name).ToLower())
		);

		await writeTo.WriteProperty(attribute.GetValueType(), attribute.Name);

		await writeTo.NewLine();
	}

	private static async Task WriteHtmlAttributeProperty(
		SourceGenerator writeTo,
		HtmlElementDefinition element,
		HtmlAttributeDefinition attribute
	)
	{
		if (!string.IsNullOrEmpty(attribute.Summary))
		{
			await writeTo.WriteSummary(attribute.Summary);
		}

		await writeTo.SeeAlsoUrl(
			seeAlsoPrefix,
			Encoding.UTF8.GetBytes((element.Element ?? element.Name).ToLower()),
			"#"u8.ToArray(),
			Encoding.UTF8.GetBytes((attribute.Attribute ?? attribute.Name).ToLower())
		);

		if (attribute.Type == HtmlAttributeType.SourceDocument)
		{
			await writeTo.Write(
				"[global::System.Diagnostics.CodeAnalysis.Experimental(\"STAGS_001\")]"
			);
			await writeTo.NewLine();
		}

		await writeTo.WriteProperty(attribute.GetValueType(), attribute.Name);

		await writeTo.NewLine();
	}

	private static async Task WriteAttributeYield(
		SourceGenerator writeTo,
		HtmlAttributeDefinition attribute
	)
	{
		var name = Encoding.UTF8.GetBytes(attribute.Name);

		await writeTo.Write("if (");
		await writeTo.Write(name);
		await writeTo.Write(".Value is not ");
		if (attribute.Type == HtmlAttributeType.Bool)
		{
			await writeTo.Write("(null or false))");
		}
		else if (attribute.Type == HtmlAttributeType.Enum)
		{
			await writeTo.Write("(null or ((Html.Attributes.Values.");
			await writeTo.Write(attribute.Enum ?? attribute.Name);
			await writeTo.Write(")default)))");
		}
		else
		{
			await writeTo.Write("null)");
		}

		await using var _ = await writeTo.Block();

		await writeTo.Write("yield return new ");
		await writeTo.Write(attribute.GetClassName());
		await writeTo.Write("(");
		if (attribute.Type is not (HtmlAttributeType.Enum
			or HtmlAttributeType.Ternary
			or HtmlAttributeType.PlainDateTime
			or HtmlAttributeType.SourceDocument
			or HtmlAttributeType.EnumList)
		)
		{
			await writeTo.Write("AttributeNames.");
			await writeTo.Write(name);
			await writeTo.Write(", ");
		}

		await writeTo.Write(name);
		if (attribute.Type is HtmlAttributeType.List or HtmlAttributeType.EnumList)
		{
			await writeTo.Write(", ListAttribute.");
			await writeTo.Write(attribute.List == ListSeparator.Comma ? "Comma" : "Whitespace");
			await writeTo.Write("Separator");
		}
		await writeTo.Write(");");
	}

	public async Task WriteElementNames(IEnumerable<HtmlElementDefinition> elements)
	{
		var filePath = Path.Join(this.directory, "ElementNames.gen.cs");
		await using var fileStream = File.Open(filePath, FileMode.Create, FileAccess.Write);
		await using var generator = new SourceGenerator(fileStream, "\t"u8.ToArray());

		await generator.WriteHeader();
		await generator.DeclareNamespace(elementsNamespace);
		await generator.Write("internal static class ElementNames");
		await using var block1 = await generator.Block();
		foreach (var name in elements.Select(e => e.Name).Order())
		{
			await generator.Write("public static readonly ReadOnlyMemory<byte> ");
			await generator.Write(name);
			await generator.Write(";");
			await generator.NewLine();
		}

		await generator.Write("static ElementNames()");
		await using var block2 = await generator.Block();
		var combined = "";
		foreach (var name in elements
			.Select(e => (e.Element ?? e.Name).ToLower())
			.OrderByDescending(v => v.Length)
			.ThenBy(v => v)
			.Distinct()
		)
		{
			if (!combined.Contains(name))
			{
				combined += name;
			}
		}
		await generator.Write("ReadOnlyMemory<byte> names = \"");
		await generator.Write(combined);
		await generator.Write("\"u8.ToArray();");
		await generator.NewLine();
		await generator.NewLine();

		foreach (var element in elements.OrderBy(e => e.Name))
		{
			var name = (element.Element ?? element.Name).ToLower();
			var index = combined.IndexOf(name, StringComparison.Ordinal);

			await generator.Write(element.Name);
			await generator.Write(" = names[");
			await generator.Write(index.ToString());
			await generator.Write(".."u8.ToArray());
			await generator.Write((index + name.Length).ToString());
			await generator.Write("];"u8.ToArray());
			await generator.NewLine();
		}
	}
}
