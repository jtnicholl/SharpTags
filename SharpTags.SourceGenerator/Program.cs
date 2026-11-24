// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Text;
using System.Text.Json;

namespace SharpTags.SourceGenerator;

internal static class Program
{
	public static async Task Main(string[] _)
	{
		const string projectDirectory = "../SharpTags";

		var attributeGenerator = new AttributeGenerator(projectDirectory);

		GlobalAttributes globalAttributes;
		await using (var fileStream = File.OpenRead("GlobalAttributes.json"))
		{
			globalAttributes = (await JsonSerializer.DeserializeAsync(
				fileStream,
				SourceGeneratorContext.File.GlobalAttributes
			))!;
		}

		var elementGenerator = new ElementGenerator(globalAttributes.Html, projectDirectory);

		Elements elements;
		await using (var fileStream = File.OpenRead("Elements.json"))
		{
			elements = (await JsonSerializer.DeserializeAsync(
				fileStream,
				SourceGeneratorContext.File.Elements
			))!;
		}

		await elementGenerator.WriteElementNames(elements.Html);

		await attributeGenerator.WriteAttributeNames(
			elements.Html
				.SelectMany(e => e.Attributes ?? [])
				.Concat(globalAttributes.Html)
				.Distinct()
		);

		await elementGenerator.WriteGenericBaseHtmlElement();
		await elementGenerator.WriteNonGenericBaseHtmlElement();
		foreach (var element in elements.Html)
		{
			await elementGenerator.WriteGenericHtmlElement(element);
			await elementGenerator.WriteNonGenericHtmlElement(element);
		}

		foreach (
			var attribute in elements.Html
				.SelectMany(e => e.Attributes ?? [])
				.DistinctBy(a => Encoding.UTF8.GetString(a.GetClassName()))
				.Concat(globalAttributes.Html)
				.Where(a => a.Type is HtmlAttributeType.Enum or HtmlAttributeType.EnumList)
		)
		{
			await attributeGenerator.WriteAttributeStaticClass(attribute);
		}
	}
}
