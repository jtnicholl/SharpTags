namespace SharpTags.UnitTests;

[TestFixture]
internal sealed class PooledListTests
{
	private PooledList<int> list = null!;

	[SetUp]
	public void SetUp()
	{
		this.list = new();
	}

	[TearDown]
	public void TearDown()
	{
		this.list.Dispose();
	}

	[Test]
	public void Add_EmptyList_Adds()
	{
		list.Add(1);

		Assert.That(list, Is.EquivalentTo([1]));
	}

	[Test]
	public void Add_NonEmptyList_Adds()
	{
		list.Add(1);
		list.Add(2);
		list.Add(3);

		Assert.That(list, Is.EquivalentTo([1, 2, 3]));
	}

	[Test]
	public void Insert_WithinRange_Inserts()
	{
		list.Add(1);
		list.Add(3);

		list.Insert(1, 2);

		Assert.That(list, Is.EquivalentTo([1, 2, 3]));
	}

	[Test]
	public void Insert_AtEnd_Adds()
	{
		list.Add(1);
		list.Add(2);

		list.Insert(2, 3);

		Assert.That(list, Is.EquivalentTo([1, 2, 3]));
	}

	[Test]
	public void Insert_NegativePosition_Throws()
	{
		list.Add(1);
		list.Add(3);

		Assert.That(() => list.Insert(-1, 0), Throws.InstanceOf<ArgumentOutOfRangeException>());
	}

	[Test]
	public void Insert_AfterEnd_Throws()
	{
		list.Add(1);
		list.Add(3);

		Assert.That(() => list.Insert(3, 0), Throws.InstanceOf<ArgumentOutOfRangeException>());
	}

	[Test]
	public void Remove_WhenExists_Removes()
	{
		list.Add(1);
		list.Add(2);
		list.Add(3);

		var returnVal = list.Remove(2);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(returnVal, Is.True);
			Assert.That(list, Is.EquivalentTo([1, 3]));
		}
	}

	[Test]
	public void Remove_WhenDoesNotExists_DoesNothing()
	{
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
		list.Add(1);
		list.Add(2);
		list.Add(3);

		var actual = list.Contains(4);

		Assert.That(actual, Is.False);
	}

	[Test]
	public void Contains_WhenContains_ReturnsTrue()
	{
		list.Add(1);
		list.Add(2);
		list.Add(3);

		var actual = list.Contains(2);

		Assert.That(actual, Is.True);
	}

	[Test]
	public void IndexOf_WhenExists_ReturnsIndex()
	{
		list.Add(1);
		list.Add(2);
		list.Add(3);

		var actual = list.IndexOf(2);

		Assert.That(actual, Is.EqualTo(1));
	}

	[Test]
	public void IndexOf_WhenDoesNotExist_ReturnsNegativeOne()
	{
		list.Add(1);
		list.Add(2);
		list.Add(3);

		var actual = list.IndexOf(4);

		Assert.That(actual, Is.EqualTo(-1));
	}

	[Test]
	public void RemoveAt_InsideRange_Removes()
	{
		list.Add(1);
		list.Add(2);
		list.Add(3);

		list.RemoveAt(1);

		Assert.That(list, Is.EquivalentTo([1, 3]));
	}

	[Test]
	public void Add_BeyondInitialCapacity_DoublesCapacityAndAdds()
	{
		var initialCapacity = this.list.Capacity;

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
		var initialCapacity = this.list.Capacity;

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
		var initialCapacity = this.list.Capacity;

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
		var initialCapacity = this.list.Capacity;
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
		list.Add(1);
		list.Add(2);
		list.Add(3);
		int[] copyTo = [7, 9, 33, 1337, 8964];

		Assert.That(() => list.CopyTo(copyTo, 3), Throws.InstanceOf<ArgumentException>());
	}
}
