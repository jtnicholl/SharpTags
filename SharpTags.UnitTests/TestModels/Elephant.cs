// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Immutable;

namespace SharpTags.UnitTests.TestModels;

internal sealed class Elephant
{
	public required string Name { get; init; }

	public required DateOnly Birthday { get; init; }

	public required ImmutableArray<string> FavoriteFoods { get; init; }
}
