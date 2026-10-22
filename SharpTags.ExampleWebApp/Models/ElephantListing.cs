namespace SharpTags.ExampleWebApp.Models;

internal record ElephantListing
{
	public required int Id { get; init; }

	public required string Name { get; init; }
}
