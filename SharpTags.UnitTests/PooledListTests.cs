namespace SharpTags.UnitTests;

[TestFixture]
internal sealed class PooledListTests
{
	[Test]
	public void EnumerableConstructor_FromEmptyList_ConstructsEmpty()
	{
		using var list = new PooledList<int>(new List<int>());

		using var _ = Assert.EnterMultipleScope();
		Assert.That(list, Is.Empty);
		Assert.That(ReferenceEquals(list.array, Array.Empty<int>()));
	}

	[Test]
	public void EnumerableConstructor_FromList_ConstructsWithValues()
	{
		List<int> items = [1, 2, 3, 4];

		using var list = new PooledList<int>(items);

		Assert.That(list, Is.EquivalentTo(items));
	}

	[Test]
	public void Add_EmptyList_Adds()
	{
		using var list = new PooledList<int>();

		list.Add(1);

		Assert.That(list, Is.EquivalentTo([1]));
	}

	[Test]
	public void Add_NonEmptyList_Adds()
	{
		using var list = new PooledList<int>();

		list.Add(1);
		list.Add(2);
		list.Add(3);

		Assert.That(list, Is.EquivalentTo([1, 2, 3]));
	}

	[Test]
	public void AddRange_Array_Adds()
	{
		using var list = new PooledList<int>();
		int[] items = [1, 2, 3, 4];

		list.AddRange(items.AsEnumerable());

		Assert.That(list, Is.EquivalentTo(items));
	}

	[Test]
	public void AddRange_List_Adds()
	{
		using var list = new PooledList<int>();
		List<int> items = [1, 2, 3, 4];

		list.AddRange(items.AsEnumerable());

		Assert.That(list, Is.EquivalentTo(items));
	}

	[Test]
	public void AddRange_EmptyList_DoesNothing()
	{
		using var list = new PooledList<int>();

		list.AddRange(new List<int>());

		Assert.That(list, Is.Empty);
	}

	[Test]
	public void AddRange_Cast_Adds()
	{
		using var list = new PooledList<int>();
		List<object> items = [1, 2, 3, 4];

		list.AddRange(items.Cast<int>());

		Assert.That(list, Is.EquivalentTo(items));
	}

	[Test]
	public void AddRange_WithoutCount_Adds()
	{
		using var list = new PooledList<int>();
		var items = ((List<int>)[1, 2, 3, 4]).Where(i => i > 0);

		list.AddRange(items);

		Assert.That(list, Is.EquivalentTo(items));
	}

	[Test]
	public void AddRangeReversed_Array_AddsReversed()
	{
		using var list = new PooledList<int>();
		int[] items = [1, 2, 3, 4];

		list.AddRangeReversed(items.AsEnumerable());

		Assert.That(list, Is.EquivalentTo(items.AsEnumerable().Reverse()));
	}

	[Test]
	public void AddRangeReversed_List_AddsReversed()
	{
		using var list = new PooledList<int>();
		List<int> items = [1, 2, 3, 4];

		list.AddRangeReversed(items);

		Assert.That(list, Is.EquivalentTo(items.AsEnumerable().Reverse()));
	}

	[Test]
	public void AddRangeReversed_EmptyList_DoesNothing()
	{
		using var list = new PooledList<int>();

		list.AddRangeReversed(new List<int>());

		Assert.That(list, Is.Empty);
	}

	[Test]
	public void AddRangeReversed_Cast_AddsReversed()
	{
		using var list = new PooledList<int>();
		List<object> items = [1, 2, 3, 4];

		list.AddRangeReversed(items.Cast<int>());

		Assert.That(list, Is.EquivalentTo(items.AsEnumerable().Reverse()));
	}

	[Test]
	public void AddRangeReversed_WithoutCount_AddsReversed()
	{
		using var list = new PooledList<int>();
		var items = ((List<int>)[1, 2, 3, 4]).Where(i => i > 0);

		list.AddRangeReversed(items.AsEnumerable());

		Assert.That(list, Is.EquivalentTo(items.AsEnumerable().Reverse()));
	}

	[Test]
	public void Insert_WithinRange_Inserts()
	{
		using var list = new PooledList<int>();
		list.Add(1);
		list.Add(3);

		list.Insert(1, 2);

		Assert.That(list, Is.EquivalentTo([1, 2, 3]));
	}

	[Test]
	public void Insert_AtEnd_Adds()
	{
		using var list = new PooledList<int>();
		list.Add(1);
		list.Add(2);

		list.Insert(2, 3);

		Assert.That(list, Is.EquivalentTo([1, 2, 3]));
	}

	[Test]
	public void Insert_NegativePosition_Throws()
	{
		using var list = new PooledList<int>();
		list.Add(1);
		list.Add(3);

		Assert.That(() => list.Insert(-1, 0), Throws.InstanceOf<ArgumentOutOfRangeException>());
	}

	[Test]
	public void Insert_AfterEnd_Throws()
	{
		using var list = new PooledList<int>();
		list.Add(1);
		list.Add(3);

		Assert.That(() => list.Insert(3, 0), Throws.InstanceOf<ArgumentOutOfRangeException>());
	}

	[Test]
	public void Remove_WhenExists_Removes()
	{
		using var list = new PooledList<int>();
		list.Add(1);
		list.Add(2);
		list.Add(3);

		var returnVal = list.Remove(2);

		using var _ = Assert.EnterMultipleScope();
		Assert.That(returnVal, Is.True);
		Assert.That(list, Is.EquivalentTo([1, 3]));
	}

	[Test]
	public void Remove_WhenDoesNotExists_DoesNothing()
	{
		using var list = new PooledList<int>();
		list.Add(1);
		list.Add(2);
		list.Add(3);

		var returnVal = list.Remove(4);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(returnVal, Is.False);
			Assert.That(list, Is.EquivalentTo([1, 2, 3]));
		}
	}

	[Test]
	public void Contains_DoesNotContain_ReturnsFalse()
	{
		using var list = new PooledList<int>();
		list.Add(1);
		list.Add(2);
		list.Add(3);

		var actual = list.Contains(4);

		Assert.That(actual, Is.False);
	}

	[Test]
	public void Contains_WhenContains_ReturnsTrue()
	{
		using var list = new PooledList<int>();
		list.Add(1);
		list.Add(2);
		list.Add(3);

		var actual = list.Contains(2);

		Assert.That(actual, Is.True);
	}

	[Test]
	public void IndexOf_WhenExists_ReturnsIndex()
	{
		using var list = new PooledList<int>();
		list.Add(1);
		list.Add(2);
		list.Add(3);

		var actual = list.IndexOf(2);

		Assert.That(actual, Is.EqualTo(1));
	}

	[Test]
	public void IndexOf_WhenDoesNotExist_ReturnsNegativeOne()
	{
		using var list = new PooledList<int>();
		list.Add(1);
		list.Add(2);
		list.Add(3);

		var actual = list.IndexOf(4);

		Assert.That(actual, Is.EqualTo(-1));
	}

	[Test]
	public void RemoveAt_InsideRange_Removes()
	{
		using var list = new PooledList<int>();
		list.Add(1);
		list.Add(2);
		list.Add(3);

		list.RemoveAt(1);

		Assert.That(list, Is.EquivalentTo([1, 3]));
	}

	[Test]
	public void Add_BeyondInitialCapacity_DoublesCapacityAndAdds()
	{
		using var list = new PooledList<int>();
		var initialCapacity = list.Capacity;

		for (int i = 0; i < initialCapacity + 5; i++)
		{
			list.Add(i);
		}

		var count = list.Count;
		var expected = Enumerable.Range(0, initialCapacity + 5).ToArray();
		using (Assert.EnterMultipleScope())
		{
			Assert.That(list, Is.EquivalentTo(expected));
			Assert.That(count, Is.EqualTo(initialCapacity + 5));
			Assert.That(list.Capacity, Is.GreaterThanOrEqualTo(initialCapacity * 2));
		}
	}

	[Test]
	public void Insert_AtEndBeyondInitialCapacity_DoublesCapacityAndAdds()
	{
		using var list = new PooledList<int>();
		var initialCapacity = list.Capacity;

		for (int i = 0; i < initialCapacity + 5; i++)
		{
			list.Insert(i, i);
		}

		var count = list.Count;
		var expected = Enumerable.Range(0, initialCapacity + 5).ToArray();
		using (Assert.EnterMultipleScope())
		{
			Assert.That(list, Is.EquivalentTo(expected));
			Assert.That(count, Is.EqualTo(initialCapacity + 5));
			Assert.That(list.Capacity, Is.GreaterThanOrEqualTo(initialCapacity * 2));
		}
	}

	[Test]
	public void Insert_AtStartBeyondInitialCapacity_DoublesCapacityAndInserts()
	{
		using var list = new PooledList<int>();
		var initialCapacity = list.Capacity;

		for (int i = initialCapacity + 4; i >= 0; i--)
		{
			list.Insert(0, i);
		}

		var count = list.Count;
		var expected = Enumerable.Range(0, initialCapacity + 5).ToArray();
		using (Assert.EnterMultipleScope())
		{
			Assert.That(list, Is.EquivalentTo(expected));
			Assert.That(count, Is.EqualTo(initialCapacity + 5));
			Assert.That(list.Capacity, Is.GreaterThanOrEqualTo(initialCapacity * 2));
		}
	}

	[Test]
	public void Insert_InMiddleBeyondInitialCapacity_Inserts()
	{
		using var list = new PooledList<int>();
		var initialCapacity = list.Capacity;
		var expected = new List<int>();

		for (int i = 0; i < initialCapacity / 2; i++)
		{
			list.Insert(i, i);
			expected.Insert(i, i);
		}

		for (int i = initialCapacity / 2; i < initialCapacity + 5; i++)
		{
			list.Insert(5, i);
			expected.Insert(5, i);
		}

		var count = list.Count;
		using (Assert.EnterMultipleScope())
		{
			Assert.That(list, Is.EquivalentTo(expected));
			Assert.That(count, Is.EqualTo(initialCapacity + 5));
			Assert.That(list.Capacity, Is.GreaterThanOrEqualTo(initialCapacity * 2));
		}
	}

	[Test]
	public void CopyTo_FromStart_Copies()
	{
		using var list = new PooledList<int>();
		list.Add(1);
		list.Add(2);
		list.Add(3);
		int[] copyTo = [7, 9, 33, 1337, 8964];

		list.CopyTo(copyTo, 0);

		Assert.That(copyTo, Is.EquivalentTo([1, 2, 3, 1337, 8964]));
	}

	[Test]
	public void CopyTo_FromMiddle_Copies()
	{
		using var list = new PooledList<int>();
		list.Add(1);
		list.Add(2);
		list.Add(3);
		int[] copyTo = [7, 9, 33, 1337, 8964];

		list.CopyTo(copyTo, 1);

		Assert.That(copyTo, Is.EquivalentTo([7, 1, 2, 3, 8964]));
	}

	[Test]
	public void CopyTo_PastEnd_Throws()
	{
		using var list = new PooledList<int>();
		list.Add(1);
		list.Add(2);
		list.Add(3);
		int[] copyTo = [7, 9, 33, 1337, 8964];

		Assert.That(() => list.CopyTo(copyTo, 3), Throws.InstanceOf<ArgumentException>());
	}
}
