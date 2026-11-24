// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.Contracts;
using System.Text;

namespace SharpTags.SourceGenerator;

internal sealed class AttributeGenerator
{
	private static readonly byte[] attributesNamespace = [.."SharpTags.Nodes.Html.Attributes"u8];

	private readonly string directory;

	public AttributeGenerator(string projectPath)
	{
		this.directory = Path.Join(projectPath, "Nodes", "Html", "Attributes");
	}

	public async Task WriteAttributeNames(IEnumerable<HtmlAttributeDefinition> attributes)
	{
		var filePath = Path.Join(this.directory, "AttributeNames.gen.cs");
		await using var generator = new SourceGenerator(
			File.Open(filePath, FileMode.Create, FileAccess.Write),
			"\t"u8.ToArray(),
			true
		);

		await generator.WriteHeader();
		await generator.DeclareNamespace(attributesNamespace);
		await generator.Write("internal static class AttributeNames");
		await using var block1 = await generator.Block();
		foreach (var name in attributes.Select(a => a.Name).Distinct().Order())
		{
			await generator.WriteReadonlyStaticField("global::System.ReadOnlyMemory<byte>", name);
		}

		await generator.NewLine();
		await generator.Write("static AttributeNames()");
		await using var block2 = await generator.Block();
		await generator.Write("global::System.ReadOnlyMemory<byte> names = \"");

		var combined = "";
		foreach (var attribute in attributes
			.OrderBy(a => a.Type == HtmlAttributeType.Bool)
			.ThenBy(a => a.Attribute ?? a.Name)
		)
		{
			var name = " " +
				(attribute.Attribute ?? attribute.Name).ToLowerOrdinal() +
				(attribute.Type != HtmlAttributeType.Bool ? "=\"" : "");
			if (!combined.Contains(name))
			{
				combined += name;
			}
		}
		await generator.Write(combined.Replace("\"", "\\\""));
		await generator.Write("\"u8.ToArray();");
		await generator.NewLine();

		foreach (var attribute in attributes.DistinctBy(a => a.Name).OrderBy(a => a.Name))
		{
			var name = " " +
				(attribute.Attribute ?? attribute.Name).ToLowerOrdinal() +
				(attribute.Type != HtmlAttributeType.Bool ? "=\"" : "");
			var index = combined.IndexOf(name, StringComparison.Ordinal);

			await generator.NewLine();
			await generator.Write(attribute.Name);
			await generator.Write(" = names[");
			await generator.Write(index.ToString());
			await generator.Write("..");
			await generator.Write((index + name.Length).ToString());
			await generator.Write("];");
		}
	}

	public async Task WriteAttributeStaticClass(HtmlAttributeDefinition attribute)
	{
		Contract.Assert(attribute.Type is HtmlAttributeType.Enum or HtmlAttributeType.EnumList);

		var enumName = Encoding.UTF8.GetBytes(attribute.Enum ?? attribute.Name);

		var filePath = Path.Join(
			this.directory,
			$"{attribute.Enum ?? attribute.Name}Attribute.gen.cs"
		);
		await using var fileStream = File.Open(filePath, FileMode.Create, FileAccess.Write);
		await using var generator = new SourceGenerator(fileStream, "\t"u8.ToArray());

		await generator.WriteHeader();
		await generator.DeclareNamespace(attributesNamespace);

		await generator.Write("internal sealed partial class ");
		await generator.Write(enumName);
		await generator.Write("Attribute");
		await generator.Write(" : ");
		await generator.Write("global::");
		await generator.Write(attributesNamespace);
		await generator.Write(".IEnumAttribute<global::");
		await generator.Write(attributesNamespace);
		await generator.Write(".Values.");
		await generator.Write(enumName);
		await generator.Write(">");

		await using var classBlock = await generator.Block();

		await generator.Write("public static global::System.ReadOnlyMemory<byte> Name => global::");
		await generator.Write(attributesNamespace);
		await generator.Write(".AttributeNames.");
		await generator.Write(attribute.Name);
		await generator.Write(";");
		await generator.NewLine();
	}
}
