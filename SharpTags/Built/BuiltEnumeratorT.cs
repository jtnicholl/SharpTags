// SPDX-License-Identifier: LGPL-3.0-or-later

namespace SharpTags.Built;

internal sealed class BuiltEnumerator<TModel, TIterated> : BuiltEnumerator<TModel>
{
	private readonly ModelEnumerator<TModel, TIterated> iterator;
	private readonly BuiltMarkup<TIterated> iterated;

	public BuiltEnumerator(
		ModelEnumerator<TModel, TIterated> iterator,
		BuiltMarkup<TIterated> iterated
	)
	{
		this.iterator = iterator;
		this.iterated = iterated;
	}

	public override long Evaluate(TModel model, PooledList<ReadOnlyMemory<byte>> list)
	{
		long output = 0;
		foreach (var item in this.iterator.Invoke(model))
		{
			output += this.iterated.Evaluate(item, list);
		}
		return output;
	}
}
