using System.Collections.Immutable;

namespace SharpTags.ExampleWebApp.Models;

internal record ElephantDetail
{
	public required int Id { get; init; }

	public required string Name { get; init; }

	public required DateOnly Birthday { get; init; }

	public required ImmutableArray<string> FavoriteFoods { get; init; }
}
