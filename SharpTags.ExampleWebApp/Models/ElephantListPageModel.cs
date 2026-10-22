namespace SharpTags.ExampleWebApp.Models;

internal class ElephantListPageModel : BasePageModel
{
	public required IList<ElephantListing> Elephants { get; init; }
}
