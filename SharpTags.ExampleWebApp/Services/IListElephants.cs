using SharpTags.ExampleWebApp.Models;

namespace SharpTags.ExampleWebApp.Services;

internal interface IListElephants
{
	ValueTask<IList<ElephantListing>> ListElephants(CancellationToken cancel);
}
