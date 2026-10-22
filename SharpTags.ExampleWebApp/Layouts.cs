using SharpTags.ExampleWebApp.Models;
using SharpTags.Nodes;
using SharpTags.Nodes.Html.Attributes.Values;
using SharpTags.Nodes.Html.Elements;

namespace SharpTags.ExampleWebApp;

internal static class Layouts
{
	public const string AppName = "My SharpTags App";

	public static Html<TModel> Default<TModel>(
		TextNode<TModel> pageTitle,
		params ReadOnlySpan<MarkupNode<TModel>> page
	) where TModel : BasePageModel
	{
		return new(
			new Head<TModel>(
				new Meta<TModel>
				{
					CharacterSet = CharacterSet.Utf8,
				},
				new Meta<TModel>
				{
					Name = "viewport",
					Content = "width=device-width, initial-scale=1.0",
				},
				new Title<TModel>(pageTitle),
				new Link<TModel>
				{
					Relationships = "stylesheet",
					Href = "/css/site.css",
				},
				new Link<TModel>
				{
					Relationships = "icon",
					Href = "/favicon.svg",
				}
			),
			new Body<TModel>(
				new Header<TModel>(
					new Navigation<TModel>(
						new Anchor<TModel>(AppName)
						{
							Class = "app-logo",
							Href = "/",
						},
						new Anchor<TModel>("Home")
						{
							Href = "/",
						},
						new Anchor<TModel>("Elephants")
						{
							Href = "/elephants",
						},
						new Anchor<TModel>("Privacy")
						{
							Href = "/privacy",
						},
						new Div<TModel>
						{
							Class = "user",
							Children =
							{
								new ContentSpan<TModel>(new TextNode<TModel>(m => m.Username)),
								new Image<TModel>
								{
									Source = "/images/user.svg",
									AltText = "User icon",
									Width = 32,
									Height = 32,
								},
							},
						}
					)
				),
				new Main<TModel>
				{
					Role = "main",
					Children =
					{
						new Div<TModel>(page)
						{
							Id = "content",
						}
					}
				}
			),
			new Footer<TModel>(
				new Div<TModel>(
					"© ",

					// Note that below is a literal value and not a delegate.
					// This means it will be evaluated only once on startup.
					new PlainText(DateTime.UtcNow.Year),
					// If you don't want to restart your application once a year, instead use:
					//new TextNode<TModel>(_ => new(DateTime.UtcNow.Year)),

					$" — {AppName} — ",
					new Anchor<TModel>("Privacy")
					{
						Class = "link",
						Href = "/privacy",
					}
				)
			)
		);
	}
}
