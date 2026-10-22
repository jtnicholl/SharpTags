// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.Contracts;

namespace SharpTags.Built;

internal interface IBuiltSegment<TModel> : IEquatable<IBuiltSegment<TModel>>
{
	[Pure]
	IEnumerable<ReadOnlyMemory<byte>> Evaluate(TModel model);
}
