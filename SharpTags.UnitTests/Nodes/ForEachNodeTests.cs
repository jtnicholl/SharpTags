// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Buffers;
using System.IO.Pipelines;
using System.Text;
using SharpTags.Nodes;
using SharpTags.Nodes.Html.Elements;
using SharpTags.UnitTests.TestModels;

namespace SharpTags.UnitTests.Nodes;

[TestFixture]
internal sealed class ForEachNodeTests
{
	private static CancellationToken Cancel => TestContext.CurrentContext.CancellationToken;

	private static IEnumerable<TestCaseData> TestCases() => [
		new(
			new Elephant
			{
				Name = "Brinkley",
				Birthday = new(2004, 8, 30),
				FavoriteFoods = ["Fish Food", "Star Potions", "Pizza", "Ice Cream", "Fried Chicken"],
			},
			"<li>Fish Food</li>" +
			"<li>Star Potions</li>" +
			"<li>Pizza</li>" +
			"<li>Ice Cream</li>" +
			"<li>Fried Chicken</li>"
		),
		new(
			new Elephant
			{
				Name = "Brinkley",
				Birthday = new(2004, 8, 30),
				FavoriteFoods = [],
			},
			""
		),
	];

	private static ForEachNode<Elephant, string> Page() => new(
		el => el.FavoriteFoods,
		[new ListItem<string>(new TextNode<string>(s => s))]
	);

	[TestCaseSource(nameof(TestCases))]
	public async Task RenderStream(Elephant elephant, string expected)
	{
		await using MemoryStream memStream = new();

		await Page().Render(memStream, elephant, Cancel);
		var result = Encoding.UTF8.GetString(memStream.ToArray());

		Assert.That(result, Is.EqualTo(expected));
	}

	[TestCaseSource(nameof(TestCases))]
	public async Task RenderPipeWriter(Elephant elephant, string expected)
	{
		await using MemoryStream memStream = new();

		await Page().Render(PipeWriter.Create(memStream), elephant, Cancel);
		var result = Encoding.UTF8.GetString(memStream.ToArray());

		Assert.That(result, Is.EqualTo(expected));
	}

	[TestCaseSource(nameof(TestCases))]
	public void RenderBufferWriter(Elephant elephant, string expected)
	{
		ArrayBufferWriter<byte> buffer = new();

		Page().Render(buffer, elephant);
		var result = Encoding.UTF8.GetString(buffer.WrittenSpan);

		Assert.That(result, Is.EqualTo(expected));
	}

	[TestCaseSource(nameof(TestCases))]
	public async Task RenderBuiltStream(Elephant elephant, string expected)
	{
		await using MemoryStream memStream = new();

		await Page().Build().Render(memStream, elephant, Cancel);
		var result = Encoding.UTF8.GetString(memStream.ToArray());

		Assert.That(result, Is.EqualTo(expected));
	}

	[TestCaseSource(nameof(TestCases))]
	public async Task RenderBuiltPipeWriter(Elephant elephant, string expected)
	{
		await using MemoryStream memStream = new();

		await Page().Build().Render(PipeWriter.Create(memStream), elephant, Cancel);
		var result = Encoding.UTF8.GetString(memStream.ToArray());

		Assert.That(result, Is.EqualTo(expected));
	}

	[TestCaseSource(nameof(TestCases))]
	public void RenderBuiltBufferWriter(Elephant elephant, string expected)
	{
		ArrayBufferWriter<byte> buffer = new();

		Page().Build().Render(buffer, elephant);
		var result = Encoding.UTF8.GetString(buffer.WrittenSpan);

		Assert.That(result, Is.EqualTo(expected));
	}
}
