using Microsoft.AspNetCore.Mvc;
using SharpTags.AspNetCore;
using SharpTags.ExampleWebApp.Models;
using SharpTags.ExampleWebApp.Services;
using SharpTags.Nodes;
using SharpTags.Nodes.Html.Elements;

namespace SharpTags.ExampleWebApp;

internal static class Pages
{
	private static readonly RequestHandler<BasePageModel> home = new(
		Layouts.Default(
			Layouts.AppName,
			new Heading1<BasePageModel>(Layouts.AppName),
			new Paragraph<BasePageModel>($"Hello and welcome to {Layouts.AppName}!")
		)
	);

	private static readonly RequestHandler<ElephantListPageModel> elephants = new(
		Layouts.Default(
			Layouts.AppName,
			new Heading2<ElephantListPageModel>("Elephants"),
			new UnorderedList<ElephantListPageModel>(
				new ListItem<ElephantListing>(
					new Anchor<ElephantListing>
					{
						Class = "link",
						Href = new(e => $"/elephants/{e.Id}"),
						Children = { new TextNode<ElephantListing>(e => e.Name) },
					}
				).ForEach((ElephantListPageModel m) => m.Elephants)
			)
		)
	);

	private static readonly RequestHandler<ElephantDetailPageModel> elephant = new(
		Layouts.Default(
			Layouts.AppName,
			new Heading2<ElephantDetailPageModel>(
				new TextNode<ElephantDetailPageModel>(e => e.Elephant.Name)
			),
			new Anchor<ElephantDetailPageModel>("Back")
			{
				Class = "link",
				Href = "/elephants"
			},
			new Table<ElephantDetailPageModel>(
				new TableBody<ElephantDetailPageModel>(
					new TableRow<ElephantDetailPageModel>(
						new TableHeader<ElephantDetailPageModel>("Birthday"),
						new TableData<ElephantDetailPageModel>(
							new Time<ElephantDetailPageModel>
							{
								DateTime = new(e => e.Elephant.Birthday),
								Children =
								{
									new TextNode<ElephantDetailPageModel>(e => e.Elephant.Birthday),
								},
							}
						)
					),
					new TableRow<ElephantDetailPageModel>(
						new TableHeader<ElephantDetailPageModel>("Favorite Foods"),
						new TableData<ElephantDetailPageModel>(
							new UnorderedList<ElephantDetailPageModel>(
								new ListItem<string>(
									new TextNode<string>(s => s)
								).ForEach((ElephantDetailPageModel m) => m.Elephant.FavoriteFoods)
							)
						)
					)
				)
			)
		)
	);

	private static readonly RequestHandler<BasePageModel> privacy = new(
		Layouts.Default(
			Layouts.AppName,
			new Heading2<BasePageModel>("Privacy Policy"),
			new Paragraph<BasePageModel>("Use this page to share your website’s privacy policy.")
		)
	);

	public static IResult Home()
	{
		BasePageModel model = new()
		{
			Username = "J. Doe",
		};
		return home.Page(model);
	}

	public static async Task<IResult> Elephants(
		[FromServices] IListElephants service,
		CancellationToken cancel
	)
	{
		ElephantListPageModel model = new()
		{
			Username = "Elephant Fan",
			Elephants = await service.ListElephants(cancel),
		};
		return elephants.Page(model);
	}

	public static async Task<IResult> Elephant(
		[FromRoute] int id,
		[FromServices] IGetElephant service,
		CancellationToken cancel
	)
	{
		var foundElephant = await service.TryGetElephant(id, cancel);
		if (foundElephant == null)
		{
			return TypedResults.NotFound();
		}

		ElephantDetailPageModel model = new()
		{
			Username = $"{foundElephant.Name}’s No. 1 Fan",
			Elephant = foundElephant,
		};
		return elephant.Page(model);
	}

	public static IResult Privacy()
	{
		BasePageModel model = new()
		{
			Username = "Concerned Citizen",
		};
		return privacy.Page(model);
	}
}
