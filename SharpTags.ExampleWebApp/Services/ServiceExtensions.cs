namespace SharpTags.ExampleWebApp.Services;

internal static class ServiceExtensions
{
	public static void AddDummyElephantServices(this IServiceCollection services)
	{
		services.AddSingleton(_ => new DummyElephantsList([
			new()
			{
				Id = 1,
				Name = "Brinkley",
				Birthday = new(2004, 8, 30),
				FavoriteFoods = [
					"Fish Food",
					"Star Potions",
					"Pizza",
					"Ice Cream",
					"Fried Chicken",
				],
			},
			new()
			{
				Id = 2,
				Name = "Dumbo",
				Birthday = new(1941, 10, 23),
				FavoriteFoods = ["Peanuts"],
			},
			new()
			{
				Id = 3,
				Name = "Horton",
				Birthday = new(1940, 6, 19),
				FavoriteFoods = ["Beezlenuts"],
			},
		]));

		services.AddSingleton<IListElephants>(
			provider => provider.GetRequiredService<DummyElephantsList>()
		);
		services.AddSingleton<IGetElephant>(
			provider => provider.GetRequiredService<DummyElephantsList>()
		);
	}
}
