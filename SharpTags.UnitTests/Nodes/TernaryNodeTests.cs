// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Buffers;
using System.IO.Pipelines;
using System.Text;
using SharpTags.Nodes;
using SharpTags.UnitTests.TestModels;

namespace SharpTags.UnitTests.Nodes;

[TestFixture]
internal sealed class TernaryNodeTests
{
	private static CancellationToken Cancel => TestContext.CurrentContext.CancellationToken;

	private static IEnumerable<TestCaseData> TestCases() =>
	[
		new(
			new Elephant
			{
				Name = "Dumbo",
				Birthday = new(1941, 10, 23),
				FavoriteFoods = ["Peanuts"],
			},
			"I'm old!"
		),
		new(
			new Elephant
			{
				Name = "Dumbo (2019 Live Action Remake Nobody Asked For)",
				Birthday = new(2019, 3, 29),
				FavoriteFoods = ["Peanuts"],
			},
			"I'm young!"
		),
		new(
			new Elephant
			{
				Name = "Brinkley",
				Birthday = new(2004, 8, 30),
				FavoriteFoods = ["Fish Food", "Star Potions", "Pizza", "Ice Cream", "Fried Chicken"],
			},
			"I exist."
		),
	];

	private static TernaryNode<Elephant> Page() => new(
		[el => el.Birthday.Year < 1960, el => el.Birthday.Year > 2010],
		[[new TextNode<Elephant>("I'm old!")], [new TextNode<Elephant>("I'm young!")]],
		[new TextNode<Elephant>("I exist.")]
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
