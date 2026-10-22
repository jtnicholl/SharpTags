using SharpTags.ExampleWebApp.Services;

namespace SharpTags.ExampleWebApp;

internal static class Program
{
	public static async Task Main(string[] args)
	{
		await using var app = BuildWebApplication(args);

		if (app.Environment.IsDevelopment())
		{
			app.MapStaticAssets();
		}

		app.MapGet("/", Pages.Home);
		app.MapGet("/elephants", Pages.Elephants);
		app.MapGet("/elephants/{id:int}", Pages.Elephant);
		app.MapGet("/privacy", Pages.Privacy);

		await app.RunAsync();
	}

	private static WebApplication BuildWebApplication(string[] args)
	{
		var builder = WebApplication.CreateSlimBuilder(args);

		builder.Services.AddDummyElephantServices();

		if (builder.Environment.IsDevelopment())
		{
			builder.WebHost.UseWebRoot("wwwroot");
			builder.WebHost.UseStaticWebAssets();
		}

		return builder.Build();
	}
}
