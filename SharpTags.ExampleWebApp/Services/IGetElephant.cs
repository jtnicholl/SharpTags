using SharpTags.ExampleWebApp.Models;

namespace SharpTags.ExampleWebApp.Services;

internal interface IGetElephant
{
	ValueTask<ElephantDetail?> TryGetElephant(int id, CancellationToken cancel);
}
