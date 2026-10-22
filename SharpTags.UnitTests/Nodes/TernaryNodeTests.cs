// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Text;
using SharpTags.Nodes;
using SharpTags.UnitTests.TestModels;

namespace SharpTags.UnitTests.Nodes;

[TestFixture]
internal sealed class TernaryNodeTests
{
	private static CancellationToken Cancel => TestContext.CurrentContext.CancellationToken;

	[Test]
	public async Task Render_FirstConditionTrue_RendersFirst()
	{
		Elephant elephant = new()
		{
			Name = "Dumbo",
			Birthday = new(1941, 10, 23),
			FavoriteFoods = ["Peanuts"],
		};
		var page = new TernaryNode<Elephant>(
			[el => el.Birthday.Year < 1960, el => el.Birthday.Year > 2010],
			[[new TextNode<Elephant>("I'm old!")], [new TextNode<Elephant>("I'm young!")]],
			[new TextNode<Elephant>("I exist.")]
		);

		string result;
		await using (MemoryStream memStream = new())
		{
			await page.Render(memStream, elephant, Cancel);
			result = Encoding.UTF8.GetString(memStream.ToArray());
		}

		const string expected = "I'm old!";
		Assert.That(result, Is.EqualTo(expected));
	}

	[Test]
	public async Task Render_SecondConditionTrue_RendersSecond()
	{
		Elephant elephant = new()
		{
			Name = "Dumbo (2019 Live Action Remake Nobody Asked For)",
			Birthday = new(2019, 3, 29),
			FavoriteFoods = ["Peanuts"],
		};
		var page = new TernaryNode<Elephant>(
			[el => el.Birthday.Year < 1960, el => el.Birthday.Year > 2010],
			[[new TextNode<Elephant>("I'm old!")], [new TextNode<Elephant>("I'm young!")]],
			[new TextNode<Elephant>("I exist.")]
		);

		string result;
		await using (MemoryStream memStream = new())
		{
			await page.Render(memStream, elephant, Cancel);
			result = Encoding.UTF8.GetString(memStream.ToArray());
		}

		const string expected = "I'm young!";
		Assert.That(result, Is.EqualTo(expected));
	}

	[Test]
	public async Task Render_AllConditionsFalse_RendersFallback()
	{
		Elephant elephant = new()
		{
			Name = "Brinkley",
			Birthday = new(2004, 8, 30),
			FavoriteFoods = ["Fish Food", "Star Potions", "Pizza", "Ice Cream", "Fried Chicken"],
		};
		var page = new TernaryNode<Elephant>(
			[el => el.Birthday.Year < 1960, el => el.Birthday.Year > 2010],
			[[new TextNode<Elephant>("I'm old!")], [new TextNode<Elephant>("I'm young!")]],
			[new TextNode<Elephant>("I exist.")]
		);

		string result;
		await using (MemoryStream memStream = new())
		{
			await page.Render(memStream, elephant, Cancel);
			result = Encoding.UTF8.GetString(memStream.ToArray());
		}

		const string expected = "I exist.";
		Assert.That(result, Is.EqualTo(expected));
	}
}
