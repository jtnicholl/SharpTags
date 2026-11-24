// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Text;

namespace SharpTags.SourceGenerator;

internal sealed class ElementGenerator
{
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

	public async Task WriteGenericBaseHtmlElement()
	{
		var elementPath = Path.Join(this.directory, "HtmlElementT.gen.cs");
		await using var fileStream = File.Open(elementPath, FileMode.Create, FileAccess.Write);
		await using var generator = new SourceGenerator(fileStream, "\t"u8.ToArray());

		await generator.WriteHeader();
		await generator.DeclareNamespace(elementsNamespace);

		await generator.Write("public closed partial class HtmlElement<TModel>");

		await using var _ = await generator.Block();
		foreach (var attribute in this.globalAttributes)
		{
			await WriteGlobalAttributeProperty(generator, attribute, true);
		}
	}

	public async Task WriteNonGenericBaseHtmlElement()
	{
		var elementPath = Path.Join(this.directory, "HtmlElement.gen.cs");
		await using var fileStream = File.Open(elementPath, FileMode.Create, FileAccess.Write);
		await using var generator = new SourceGenerator(fileStream, "\t"u8.ToArray());

		await generator.WriteHeader();
		await generator.DeclareNamespace(elementsNamespace);

		await generator.Write("public closed partial class HtmlElement");

		await using var _ = await generator.Block();
		foreach (var attribute in this.globalAttributes)
		{
			await WriteGlobalAttributeProperty(generator, attribute, false);
		}
	}

	public async Task WriteGenericHtmlElement(HtmlElementDefinition element)
	{
		var elementPath = Path.Join(this.directory, $"{element.Name}T.gen.cs");
		var name = Encoding.UTF8.GetBytes(element.Name);
		await using var fileStream = File.Open(elementPath, FileMode.Create, FileAccess.Write);
		await using var generator = new SourceGenerator(fileStream, "\t"u8.ToArray());

		await generator.WriteHeader();
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
			await WriteHtmlAttributeProperty(generator, element, attribute, true);
		}

		await generator.Write(
			"private protected override global::System.Collections.Generic." +
			"IEnumerable<global::SharpTags.Nodes.Html.Attributes." +
			"HtmlAttribute<TModel>> HtmlAttributes"
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
					await WriteGenericAttributeYield(generator, attribute);
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

	public async Task WriteNonGenericHtmlElement(HtmlElementDefinition element)
	{
		var elementPath = Path.Join(this.directory, $"{element.Name}.gen.cs");
		var name = Encoding.UTF8.GetBytes(element.Name);
		await using var fileStream = File.Open(elementPath, FileMode.Create, FileAccess.Write);
		await using var generator = new SourceGenerator(fileStream, "\t"u8.ToArray());

		await generator.WriteHeader();
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
		await generator.Write(" : HtmlElement");

		await using var _ = await generator.Block();
		foreach (var attribute in element.Attributes ?? [])
		{
			await WriteHtmlAttributeProperty(generator, element, attribute, false);
		}

		await generator.Write(
			"private protected override global::System.Collections.Generic." +
			"IEnumerable<global::System.ReadOnlyMemory<byte>> Attributes"
		);
		await using (await generator.Block())
		{
			await generator.Write("get");
			await using (await generator.Block())
			{
				await generator.Write("global::SharpTags.Nodes.Html.Attributes.AttributeResult result;");
				await generator.NewLine();

				foreach (var attribute in (element.Attributes ?? [])
					.Concat(this.globalAttributes)
					.OrderBy(a => a.Attribute ?? a.Name)
				)
				{
					await WriteNonGenericAttributeYield(generator, attribute);
				}
			}
		}
		await generator.NewLine();

		await generator.WriteSummary(
			"Construct a new <see cref=\""u8.ToArray(),
			name,
			"\" />."u8.ToArray()
		);
		await generator.Write("public ");
		await generator.Write(name);
		if (element.Void)
		{
			await generator.Write("() : base(ElementNames.");
			await generator.Write(name);
			await generator.Write(", true, new VoidElementChildren(ElementNames.");
			await generator.Write(name);
			await generator.Write(")) { }");
		}
		else
		{
			await generator.Write("(params ReadOnlySpan<");
			await generator.Write("MarkupNode> children) : base(ElementNames.");
			await generator.Write(name);
			await generator.Write(", false, children.ToList()) { }");

			await generator.NewLine();
			await generator.NewLine();

			await generator.WriteSummary(
				"Construct a new <see cref=\""u8.ToArray(),
				name,
				"\" />."u8.ToArray()
			);
			await generator.Write("public ");
			await generator.Write(name);
			await generator.Write("(IEnumerable<");
			await generator.Write("MarkupNode> children) : base(ElementNames.");
			await generator.Write(name);
			await generator.Write(", false, children.ToList()) { }");
		}
		await generator.NewLine();
		await generator.NewLine();

		await generator.Write("/// <inheritdoc />");
		await generator.NewLine();
		await generator.Write("public override ");
		await generator.Write(name);
		await generator.Write("<TModel> AsTyped<TModel>()");
		await using (await generator.Block())
		{
			await generator.Write("return new(");
			if (!element.Void)
			{
				await generator.Write("Children.Select(c => c.AsTyped<TModel>())");
			}
			await generator.Write(")");
			await using (await generator.Block(true))
			{
				foreach (var attribute in (element.Attributes ?? [])
					.Concat(this.globalAttributes)
					.Where(a => a.Type is not HtmlAttributeType.SourceDocument)
					.OrderBy(a => a.Name)
				)
				{
					await generator.Write(attribute.Name);
					await generator.Write(" = ");
					await generator.Write(attribute.Name);
					await generator.Write(",");
					await generator.NewLine();
				}
			}
		}
	}

	private static async Task WriteGlobalAttributeProperty(
		SourceGenerator writeTo,
		HtmlAttributeDefinition attribute,
		bool generic
	)
	{
		if (!string.IsNullOrEmpty(attribute.Summary))
		{
			await writeTo.WriteSummary(attribute.Summary);
		}
		await writeTo.SeeAlsoUrl(
			globalSeeAlsoPrefix,
			Encoding.UTF8.GetBytes((attribute.Attribute ?? attribute.Name).ToLower())
		);

		await writeTo.WriteProperty(
			generic ? attribute.GetGenericValueType() : attribute.GetNonGenericValueType(),
			attribute.Name
		);

		await writeTo.NewLine();
	}

	private static async Task WriteHtmlAttributeProperty(
		SourceGenerator writeTo,
		HtmlElementDefinition element,
		HtmlAttributeDefinition attribute,
		bool generic
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

		await writeTo.WriteProperty(
			generic ? attribute.GetGenericValueType() : attribute.GetNonGenericValueType(),
			attribute.Name
		);

		await writeTo.NewLine();
	}

	private static async Task WriteGenericAttributeYield(
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
			await writeTo.Write("(null or default(global::SharpTags.Nodes.Html.Attributes.Values.");
			await writeTo.Write(attribute.Enum ?? attribute.Name);
			await writeTo.Write(")))");
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
			await writeTo.Write("global::SharpTags.Nodes.Html.Attributes.AttributeNames.");
			await writeTo.Write(name);
			await writeTo.Write(", ");
		}

		await writeTo.Write(name);
		if (attribute.Type is HtmlAttributeType.List or HtmlAttributeType.EnumList)
		{
			await writeTo.Write(", global::SharpTags.Nodes.Html.Attributes.ListAttribute.");
			await writeTo.Write(attribute.List == ListSeparator.Comma ? "Comma" : "Whitespace");
			await writeTo.Write("Separator");
		}
		await writeTo.Write(");");
	}

	private static async Task WriteNonGenericAttributeYield(
		SourceGenerator writeTo,
		HtmlAttributeDefinition attribute
	)
	{
		var name = Encoding.UTF8.GetBytes(attribute.Name);

		if (attribute.Type == HtmlAttributeType.Bool)
		{
			await writeTo.Write("if (");
			await writeTo.Write(name);
			await writeTo.Write(")");

			await using var yield = await writeTo.Block();
			await writeTo.Write(
				"yield return global::SharpTags.Nodes.Html.Attributes.AttributeNames."
			);
			await writeTo.Write(attribute.Name);
			await writeTo.Write(";");
		}
		else if (attribute.Type == HtmlAttributeType.Ternary)
		{
			await writeTo.Write("if (");
			await writeTo.Write(name);
			await writeTo.Write(" is true)");

			await using (await writeTo.Block())
			{
				await writeTo.Write("yield return global::SharpTags.Nodes.Html.Attributes.");
				await writeTo.Write(attribute.Name);
				await writeTo.Write("Attribute.TrueValue;");
			}

			await writeTo.Write("else if (");
			await writeTo.Write(name);
			await writeTo.Write(" is false)");

			await using (await writeTo.Block())
			{
				await writeTo.Write("yield return global::SharpTags.Nodes.Html.Attributes.");
				await writeTo.Write(attribute.Name);
				await writeTo.Write("Attribute.FalseValue;");
			}
		}
		else if (attribute.Type is HtmlAttributeType.Enum)// or HtmlAttributeType.EnumList)
		{
			await writeTo.Write("result = global::SharpTags.Nodes.Html.Attributes.");
			await writeTo.Write(attribute.Enum ?? attribute.Name);
			await writeTo.Write("Attribute.Evaluate(");
			await writeTo.Write(attribute.Name);
			await writeTo.Write(");");
			await writeTo.NewLine();

			await writeTo.Write("if (result.HasValue)");
			await using (await writeTo.Block())
			{
				await writeTo.Write(
					"yield return global::SharpTags.Nodes.Html.Attributes.AttributeNames."
				);
				await writeTo.Write(attribute.Name);
				await writeTo.Write(";");
				await writeTo.NewLine();

				await writeTo.Write("yield return result.Value;");
				await writeTo.NewLine();

				await writeTo.Write(
					"yield return global::SharpTags.Nodes.Html.Attributes.AttributeNames."
				);
				await writeTo.Write(attribute.Name);
				await writeTo.Write("[^1..];");
				await writeTo.NewLine();
			}

			await writeTo.Write("else if (result.IsPresent)");
			await using (await writeTo.Block())
			{
				await writeTo.Write(
					"yield return global::SharpTags.Nodes.Html.Attributes.AttributeNames."
				);
				await writeTo.Write(attribute.Name);
				await writeTo.Write("[..^2];");
			}
		}
		else
		{
			if (attribute.Type is HtmlAttributeType.List or HtmlAttributeType.EnumList)
			{
				await writeTo.Write("if (");
				await writeTo.Write(attribute.Name);
				await writeTo.Write(".Enumerable is { } enumerable");
				await writeTo.Write(attribute.Name);
				await writeTo.Write(")");
			}
			else if (attribute.Type is HtmlAttributeType.PlainDateTime)
			{
				await writeTo.Write("if (");
				await writeTo.Write(attribute.Name);
				await writeTo.Write(".Text is global::SharpTags.PlainText text)");
			}
			else
			{
				await writeTo.Write("if (");
				await writeTo.Write(attribute.Name);
				await writeTo.Write(" is not null)");
			}

			await using (await writeTo.Block())
			{
				await writeTo.Write(
					"yield return global::SharpTags.Nodes.Html.Attributes.AttributeNames."
				);
				await writeTo.Write(attribute.Name);
				await writeTo.Write(";");
				await writeTo.NewLine();

				if (attribute.Type is HtmlAttributeType.List or HtmlAttributeType.EnumList)
				{
					await writeTo.Write("using var enumerator = enumerable");
					await writeTo.Write(attribute.Name);
					await writeTo.Write(".GetEnumerator();");
					await writeTo.NewLine();

					await writeTo.Write("if (enumerator.MoveNext())");
					await using (await writeTo.Block())
					{
						if (attribute.Type == HtmlAttributeType.EnumList)
						{
							await writeTo.Write(
								"yield return global::SharpTags.Nodes.Html.Attributes."
							);
							await writeTo.Write(attribute.Name);
							await writeTo.Write("Attribute.Evaluate(enumerator.Current).Value;");
							await writeTo.NewLine();
						}
						else
						{
							await writeTo.Write("yield return enumerator.Current.Utf8Bytes;");
						}

						await writeTo.Write("while (enumerator.MoveNext())");
						await using (await writeTo.Block())
						{
							await writeTo.Write(
								"yield return global::SharpTags.Nodes.Html." +
								"Attributes.ListAttribute."
							);
							await writeTo.Write(
								attribute.List == ListSeparator.Comma ? "Comma" : "Whitespace"
							);
							await writeTo.Write("Separator.Utf8Bytes;");
							await writeTo.NewLine();

							if (attribute.Type == HtmlAttributeType.EnumList)
							{
								await writeTo.Write(
									"yield return global::SharpTags.Nodes.Html.Attributes."
								);
								await writeTo.Write(attribute.Name);
								await writeTo.Write(
									"Attribute.Evaluate(enumerator.Current).Value;"
								);
							}
							else
							{
								await writeTo.Write("yield return enumerator.Current.Utf8Bytes;");
							}
						}
					}
				}
				else
				{
					await writeTo.Write("yield return ");
					if (attribute.GetNonGenericValueType().EndsWith("PlainText?"u8))
					{
						await writeTo.Write(attribute.Name);
						await writeTo.Write(".Value.Utf8Bytes;");
					}
					else if (attribute.Type is HtmlAttributeType.PlainDateTime)
					{
						await writeTo.Write("text.Utf8Bytes;");
					}
					else
					{
						await writeTo.Write("new global::SharpTags.PlainText(");
						await writeTo.Write(attribute.Name);
						await writeTo.Write(").Utf8Bytes;");
					}
					await writeTo.NewLine();
				}

				await writeTo.Write(
					"yield return global::SharpTags.Nodes.Html.Attributes.AttributeNames."
				);
				await writeTo.Write(attribute.Name);
				await writeTo.Write("[^1..];");
			}
		}
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
			await generator.Write("public static readonly global::System.ReadOnlyMemory<byte> ");
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
		await generator.Write("global::System.ReadOnlyMemory<byte> names = \"");
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
