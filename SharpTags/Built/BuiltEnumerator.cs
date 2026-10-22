// SPDX-License-Identifier: LGPL-3.0-or-later

namespace SharpTags.Built;

internal abstract class BuiltEnumerator<TModel>
{
	public abstract long Evaluate(TModel model, PooledList<ReadOnlyMemory<byte>> list);
}
