// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Text;
using SharpTags.Nodes;
using SharpTags.Nodes.Html.Elements;
using SharpTags.UnitTests.TestModels;

namespace SharpTags.UnitTests.Nodes;

[TestFixture]
internal sealed class ForEachNodeTests
{
	private static CancellationToken Cancel => TestContext.CurrentContext.CancellationToken;

	[Test]
	public async Task Render_ListItems_RendersEach()
	{
		Elephant elephant = new()
		{
			Name = "Brinkley",
			Birthday = new(2004, 8, 30),
			FavoriteFoods = ["Fish Food", "Star Potions", "Pizza", "Ice Cream", "Fried Chicken"],
		};
		var page = new ForEachNode<Elephant, string>(
			el => el.FavoriteFoods,
			[new ListItem<string>(new TextNode<string>(s => s))]
		);

		string result;
		await using (MemoryStream memStream = new())
		{
			await page.Render(memStream, elephant, Cancel);
			result = Encoding.UTF8.GetString(memStream.ToArray());
		}

		const string expected =
			"<li>Fish Food</li>" +
			"<li>Star Potions</li>" +
			"<li>Pizza</li>" +
			"<li>Ice Cream</li>" +
			"<li>Fried Chicken</li>";
		Assert.That(result, Is.EqualTo(expected));
	}

	[Test]
	public async Task Render_EmptyList_DoesNotRender()
	{
		Elephant elephant = new()
		{
			Name = "Brinkley",
			Birthday = new(2004, 8, 30),
			FavoriteFoods = [],
		};
		var page = new ForEachNode<Elephant, string>(
			el => el.FavoriteFoods,
			[new ListItem<string>(new TextNode<string>(s => s))]
		);

		string result;
		await using (MemoryStream memStream = new())
		{
			await page.Render(memStream, elephant, Cancel);
			result = Encoding.UTF8.GetString(memStream.ToArray());
		}

		Assert.That(result, Is.Empty);
	}
}
