// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.Contracts;

namespace SharpTags;

internal static class MemoryExtensions
{
	public static ReadOnlyMemory<byte> FindSlice(
		this ReadOnlyMemory<byte> from,
		ReadOnlySpan<byte> find
	)
	{
		var index = from.Span.IndexOf(find);
		Contract.Assert(index >= 0);
		return from.Slice(index, find.Length);
	}
}
