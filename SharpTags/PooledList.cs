// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Buffers;
using System.Collections;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace SharpTags;

/// <summary>
/// Implements an <see cref="IList{T}" /> using an <see cref="ArrayPool{T}" />
/// to reduce heap allocations.
/// </summary>
/// <typeparam name="T">The type of elements in the list.</typeparam>
internal sealed class PooledList<T> : IList<T>, IReadOnlyList<T>, IDisposable
	where T : IEquatable<T>?
{
	/// <inheritdoc cref="IList{T}.this" />
	public T this[int index]
	{
		get
		{
			ArgumentOutOfRangeException.ThrowIfNegative(index);
			ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, Count);
			return this.array[index];
		}
		set
		{
			ObjectDisposedException.ThrowIf(Disposed, this);
			ArgumentOutOfRangeException.ThrowIfNegative(index);
			ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, Count);
#if DEBUG || CONTRACTS_FULL
			this.version++;
#endif
			this.array[index] = value;
		}
	}

	/// <inheritdoc cref="ICollection{T}.Count" />
	public int Count
	{
		get;
		private set
		{
#if DEBUG || CONTRACTS_FULL
			this.version++;
#endif
			field = value;
		}
	}

	bool ICollection<T>.IsReadOnly => Disposed;

	internal int Capacity => this.array.Length;
	internal bool Disposed { get; private set; }

	private readonly ArrayPool<T> arrayPool;
	internal T[] array;

#if DEBUG || CONTRACTS_FULL
	private int version;
#endif

	private struct Enumerator : IEnumerator<T>
	{
		public readonly T Current
		{
			get
			{
#if DEBUG || CONTRACTS_FULL
				// Debug only because this class is internal
				if (this.version != this.list.version)
				{
					throw new InvalidOperationException("List contents were modified");
				}
#endif
				return this.list[this.index];
			}
		}

		readonly object? IEnumerator.Current => Current;

		private readonly PooledList<T> list;
#if DEBUG || CONTRACTS_FULL
		private readonly int version;
#endif
		private int index = -1;

		public Enumerator(PooledList<T> list)
		{
			this.list = list;
#if DEBUG || CONTRACTS_FULL
			this.version = list.version;
#endif
		}

		public bool MoveNext()
		{
#if DEBUG || CONTRACTS_FULL
			if (this.version != this.list.version)
			{
				throw new InvalidOperationException("List contents were modified");
			}
#endif
			this.index++;
			return this.index < this.list.Count;
		}

		public void Reset()
		{
			this.index = -1;
		}

		void IDisposable.Dispose() { }
	}

	public PooledList(ArrayPool<T> from, int initialCapacity = 16)
	{
		this.arrayPool = from;
		this.array = initialCapacity == 0 ? [] : this.arrayPool.Rent(initialCapacity);
	}

	public PooledList(int initialCapacity = 16) : this(ArrayPool<T>.Shared, initialCapacity) { }

	public PooledList(ArrayPool<T> from, IEnumerable<T> collection)
	{
		this.arrayPool = from;
		if (collection.TryGetNonEnumeratedCount(out var count))
		{
			if (count == 0)
			{
				this.array = [];
				return;
			}

			this.array = this.arrayPool.Rent(count);
		}
		else
		{
			this.array = this.arrayPool.Rent(16);
		}

		AddRange(collection);
	}

	public PooledList(IEnumerable<T> collection) : this(ArrayPool<T>.Shared, collection) { }

	public PooledList(ArrayPool<T> from, params ReadOnlySpan<T> span)
	{
		this.arrayPool = from;
		this.array = span.IsEmpty ? [] : this.arrayPool.Rent(span.Length);
		span.CopyTo(this.array);
	}

	public PooledList(params ReadOnlySpan<T> span) : this(ArrayPool<T>.Shared, span) { }

	~PooledList()
	{
		ReturnCleared();
	}

	/// <inheritdoc />
	[Pure]
	public IEnumerator<T> GetEnumerator() => new Enumerator(this);

	[Pure]
	IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

	/// <summary>Adds an item to the list.</summary>
	/// <param name="item">The item to add.</param>
	/// <exception cref="ObjectDisposedException">If the list was disposed.</exception>
	public void Add(T item)
	{
		ObjectDisposedException.ThrowIf(Disposed, this);
		if (Capacity == Count)
		{
			var newArray = this.arrayPool.Rent(Capacity * 2);
			Array.Copy(this.array, newArray, Count);
			ReturnCleared();
			this.array = newArray;
		}
		this.array[Count++] = item;
	}

	public void EnsureCapacity(int minCapacity)
	{
		if (minCapacity > Capacity)
		{
			var newArray = this.arrayPool.Rent(Math.Max(minCapacity, Capacity * 2));
			Array.Copy(this.array, newArray, Count);
			ReturnCleared();
			this.array = newArray;
		}
	}

	public void AddRange(IEnumerable<T> items)
	{
		if (items is T[] itemArray)
		{
			AddRange(itemArray.AsSpan());
		}
		else if (items.TryGetNonEnumeratedCount(out var count))
		{
			if (count == 0)
			{
				return;
			}
			int size = Count + count;
			EnsureCapacity(size);

			if (items is ICollection<T> collection)
			{
				collection.CopyTo(this.array, Count);
				Count = size;
			}
			else
			{
				foreach (var item in items)
				{
					this.array[Count++] = item;
				}
			}
		}
		else
		{
			foreach (var item in items)
			{
				Add(item);
			}
		}
	}

	public void AddRangeReversed(IEnumerable<T> items)
	{
		if (items is T[] itemArray)
		{
			AddRange(itemArray.AsSpan());
			this.array.AsSpan(Count - itemArray.Length, itemArray.Length).Reverse();
		}
		else if (items.TryGetNonEnumeratedCount(out var count))
		{
			if (count == 0)
			{
				return;
			}
			int size = Count + count;
			EnsureCapacity(size);

			if (items is ICollection<T> collection)
			{
				collection.CopyTo(this.array, Count);
				this.array.AsSpan(Count, count).Reverse();
				Count = size;
			}
			else
			{
				foreach (var item in items)
				{
					this.array[Count++] = item;
				}
				this.array.AsSpan(Count - count, count).Reverse();
			}
		}
		else
		{
			var startCount = Count;
			foreach (var item in items)
			{
				Add(item);
			}
			this.array.AsSpan(startCount, Count - startCount).Reverse();
		}
	}

	public void AddRange(params ReadOnlySpan<T> items)
	{
		int size = Count + items.Length;
		if (size > Capacity)
		{
			var newArray = this.arrayPool.Rent(size * 2);
			this.array.AsSpan(0, Count).CopyTo(newArray);
			ReturnCleared();
			this.array = newArray;
		}

		items.CopyTo(this.array.AsSpan(Count));
		Count = size;
	}

	/// <summary>
	/// Removes all items from the list.
	/// </summary>
	/// <exception cref="ObjectDisposedException">If the list was disposed.</exception>
	public void Clear()
	{
		ObjectDisposedException.ThrowIf(Disposed, this);
		if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
		{
			for (int i = 0; i < Count; i++)
			{
				this.array[i] = default!;
			}
		}
		Count = 0;
	}

	public void Reverse()
	{
		this.array.AsSpan(0, Count).Reverse();
	}

	/// <inheritdoc />
	[Pure]
	public bool Contains(T item)
	{
		for (int i = 0; i < Count; i++)
		{
			if (this.array[i]?.Equals(item) ?? item == null)
			{
				return true;
			}
		}
		return false;
	}

	/// <inheritdoc />
	public void CopyTo(T[] destination, int arrayIndex)
	{
		this.array.AsSpan(0, Count).CopyTo(destination.AsSpan(arrayIndex));
	}

	/// <inheritdoc />
	public bool Remove(T item)
	{
		ObjectDisposedException.ThrowIf(Disposed, this);
		for (int i = 0; i < Count; i++)
		{
			if (this.array[i]?.Equals(item) ?? item == null)
			{
				Count--;
				for (int j = i; j < Count; j++)
				{
					this.array[j] = this.array[j + 1];
				}

				if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
				{
					this.array[Count] = default!;
				}
				return true;
			}
		}
		return false;
	}

	/// <inheritdoc />
	[Pure]
	public int IndexOf(T item)
	{
		for (int i = 0; i < Count; i++)
		{
			if (this.array[i]?.Equals(item) ?? item == null)
			{
				return i;
			}
		}
		return -1;
	}

	/// <inheritdoc />
	public void Insert(int index, T item)
	{
		ObjectDisposedException.ThrowIf(Disposed, this);
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfGreaterThan(index, Count);

		if (this.array.Length != Count)
		{
			for (int i = Count; i > index; i--)
			{
				this.array[i] = this.array[i - 1];
			}

			this.array[index] = item;
		}
		else
		{
			var newArray = this.arrayPool.Rent(this.array.Length * 2);

			this.array.AsSpan(0, index).CopyTo(newArray);
			newArray[index] = item;
			this.array.AsSpan(index).CopyTo(newArray.AsSpan(index + 1));

			ReturnCleared();
			this.array = newArray;
		}
		Count++;
	}

	/// <inheritdoc />
	public void RemoveAt(int index)
	{
		ObjectDisposedException.ThrowIf(Disposed, this);
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, Count);
		Count--;
		for (int i = index; i < Count; i++)
		{
			this.array[i] = this.array[i + 1];
		}

		if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
		{
			this.array[Count] = default!;
		}
	}

	[Pure]
	public T[] ToArray()
	{
		return this.array.AsSpan(0, Count).ToArrayFast();
	}

	/// <summary>
	/// Returns the underlying array to the array pool. While the list can still
	/// be read, it will be empty, and any attempts to modify it will throw an
	/// <see cref="ObjectDisposedException" />.
	/// </summary>
	public void Dispose()
	{
		if (Disposed)
		{
			return;
		}
		Disposed = true;
		ReturnCleared();
		this.array = [];
		Count = 0;
		GC.SuppressFinalize(this);
	}

	private void ReturnCleared()
	{
		if (this.array.Length == 0)
		{
			return;
		}
		if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
		{
			Array.Clear(this.array, 0, Count);
		}
		this.arrayPool.Return(this.array);
	}
}
