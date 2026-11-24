// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace SharpTags.Nodes.Html;

internal sealed class VoidElementChildren : IList<MarkupNode>
{
	int ICollection<MarkupNode>.Count => 0;
	bool ICollection<MarkupNode>.IsReadOnly => true;

	MarkupNode IList<MarkupNode>.this[int index]
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

	private static IEnumerator<MarkupNode> GetEnumerator()
	{
		return Enumerable.Empty<MarkupNode>().GetEnumerator();
	}

	IEnumerator<MarkupNode> IEnumerable<MarkupNode>.GetEnumerator()
	{
		return GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	[DoesNotReturn]
	void ICollection<MarkupNode>.Add(MarkupNode item) => throw ThrowException();

	[DoesNotReturn]
	void ICollection<MarkupNode>.Clear() => throw ThrowException();

	bool ICollection<MarkupNode>.Contains(MarkupNode item) => false;

	void ICollection<MarkupNode>.CopyTo(MarkupNode[] array, int arrayIndex) { }

	[DoesNotReturn]
	bool ICollection<MarkupNode>.Remove(MarkupNode item) => throw ThrowException();

	int IList<MarkupNode>.IndexOf(MarkupNode item) => -1;

	[DoesNotReturn]
	void IList<MarkupNode>.Insert(int index, MarkupNode item)
	{
		throw ThrowException();
	}

	[DoesNotReturn]
	void IList<MarkupNode>.RemoveAt(int index) => throw ThrowException();

	[DoesNotReturn]
	private InvalidOperationException ThrowException()
	{
		throw new InvalidOperationException(
			Encoding.ASCII.GetString(this.name.Span) +
				" is a void element and cannot have children."
		);
	}
}
