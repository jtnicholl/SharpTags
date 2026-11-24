// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace SharpTags.Nodes.Html;

internal sealed class VoidElementChildren<TModel> : IList<MarkupNode<TModel>>
{
	int ICollection<MarkupNode<TModel>>.Count => 0;
	bool ICollection<MarkupNode<TModel>>.IsReadOnly => true;

	MarkupNode<TModel> IList<MarkupNode<TModel>>.this[int index]
	{
		[DoesNotReturn]
		get => throw new ArgumentOutOfRangeException(nameof(index));

		[DoesNotReturn]
		set => throw ThrowException();
	}

	private readonly ReadOnlyMemory<byte> name;

	internal VoidElementChildren(ReadOnlyMemory<byte> name)
	{
		this.name = name;
	}

	private static IEnumerator<MarkupNode<TModel>> GetEnumerator()
	{
		return Enumerable.Empty<MarkupNode<TModel>>().GetEnumerator();
	}

	IEnumerator<MarkupNode<TModel>> IEnumerable<MarkupNode<TModel>>.GetEnumerator()
	{
		return GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	[DoesNotReturn]
	void ICollection<MarkupNode<TModel>>.Add(MarkupNode<TModel> item) => throw ThrowException();

	[DoesNotReturn]
	void ICollection<MarkupNode<TModel>>.Clear() => throw ThrowException();

	bool ICollection<MarkupNode<TModel>>.Contains(MarkupNode<TModel> item) => false;

	void ICollection<MarkupNode<TModel>>.CopyTo(MarkupNode<TModel>[] array, int arrayIndex) { }

	[DoesNotReturn]
	bool ICollection<MarkupNode<TModel>>.Remove(MarkupNode<TModel> item) => throw ThrowException();

	int IList<MarkupNode<TModel>>.IndexOf(MarkupNode<TModel> item) => -1;

	[DoesNotReturn]
	void IList<MarkupNode<TModel>>.Insert(int index, MarkupNode<TModel> item)
	{
		throw ThrowException();
	}

	[DoesNotReturn]
	void IList<MarkupNode<TModel>>.RemoveAt(int index) => throw ThrowException();

	[DoesNotReturn]
	private InvalidOperationException ThrowException()
	{
		throw new InvalidOperationException(
			Encoding.ASCII.GetString(this.name.Span) +
				" is a void element and cannot have children."
		);
	}
}
