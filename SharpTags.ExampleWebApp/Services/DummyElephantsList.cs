using System.Collections.Frozen;
using SharpTags.ExampleWebApp.Models;

namespace SharpTags.ExampleWebApp.Services;

internal sealed class DummyElephantsList : IListElephants, IGetElephant
{
	private readonly FrozenDictionary<int, ElephantDetail> elephants;

	public DummyElephantsList(IEnumerable<ElephantDetail> elephants)
	{
		this.elephants = elephants.ToFrozenDictionary(e => e.Id);
	}

	public ValueTask<IList<ElephantListing>> ListElephants(CancellationToken cancel)
	{
		IList<ElephantListing> result = this.elephants.Values
			.Select(e => new ElephantListing { Id = e.Id, Name = e.Name })
			.ToList();
		return ValueTask.FromResult(result);
	}

	public ValueTask<ElephantDetail?> TryGetElephant(int id, CancellationToken cancel)
	{
		return ValueTask.FromResult(this.elephants.GetValueOrDefault(id));
	}
}
