// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace SharpTags;

internal static class SpanExtensions
{
	// I don't know why this isn't in the default libraries
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Pure]
	public static List<T> ToList<T>(this ReadOnlySpan<T> span)
	{
		List<T> output = new(span.Length);
		output.AddRange(span);
		return output;
	}

	// The default Span.ToArray() uses a zero-initialized array, just to
	// overwrite the whole thing. I am assuming that is an oversight, this
	// should be faster on large value type spans
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Pure]
	public static T[] ToArrayFast<T>(this ReadOnlySpan<T> span)
	{
		if (span.IsEmpty)
		{
			return [];
		}

		var output = GC.AllocateUninitializedArray<T>(span.Length);
		span.CopyTo(output);
		return output;
	}
}
