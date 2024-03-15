using NUnit.Framework;
using System;
using DataBind.CollectionExt;

namespace TestDataBind.DataObserver.Interperter
{
	[TestFixture]
	public class ListTests
	{
		[Test]
		public void Add_StateUnderTest_ExpectedBehavior()
		{
			// Arrange
			var list = new List<int>();
			Assert.AreEqual(list.Count, 0);

			var item = 100;
			// Act
			list.Add(
				item);

			// Assert
			Assert.AreEqual(list.Count, 1);
			Assert.AreEqual(list[0], 100);
		}

		[Test]
		public void Clear_StateUnderTest_ExpectedBehavior()
		{
			// Arrange
			var list = new List<int>()
			{
				100,
				200,
				100,
				100,
				200,
			};
			Assert.AreEqual(list.Count, 5);

			// Act
			list.Clear();

			// Assert
			Assert.AreEqual(list.Count, 0);
		}

		[Test]
		public void Contains_StateUnderTest_ExpectedBehavior()
		{
			// Arrange
			var list = new List<int>();

			var item = 100;

			var result = list.Contains(item);
			Assert.IsFalse(result);

			list.Add(item);
			// Act
			result = list.Contains(item);

			// Assert
			Assert.IsTrue(result);
		}

		[Test]
		public void Contains_StateUnderTest_ExpectedBehavior1()
		{
			// Arrange
			var list = new List<object>();

			object value = null;
			// Act
			var result = list.Contains(
				value);

			// Assert
			Assert.IsFalse(result);
		}

		[Test]
		public void CopyTo_StateUnderTest_ExpectedBehavior()
		{
			// Arrange
			var list = new List<int>() {
				2,3,4,5,
			};
			int[] array = new int[list.Count];
			int arrayIndex = 0;

			// Act
			list.CopyTo(
				array,
				arrayIndex);

			// Assert
			Assert.AreEqual(list.Count, array.Length);
			Assert.AreEqual(list[0], array[0]);
			Assert.AreEqual(list[1], array[1]);
			Assert.AreEqual(list[2], array[2]);
			Assert.AreEqual(list[3], array[3]);
		}

		[Test]
		public void CopyTo_StateUnderTest_ExpectedBehavior1()
		{
			// Arrange
			var list = new List<int>() {
				2,3,4,5,
			};
			int arrayIndex = 1;
			int[] array = new int[list.Count + arrayIndex];

			// Act
			list.CopyTo(
				array,
				arrayIndex);

			// Assert
			Assert.AreEqual(list.Count, array.Length - arrayIndex);
			Assert.AreEqual(list[0], array[arrayIndex + 0]);
			Assert.AreEqual(list[1], array[arrayIndex + 1]);
			Assert.AreEqual(list[2], array[arrayIndex + 2]);
			Assert.AreEqual(list[3], array[arrayIndex + 3]);
		}

		[Test]
		public void GetEnumerator_StateUnderTest_ExpectedBehavior()
		{
			// Arrange
			var list = new List<int>()
			{
				2,3,4,
			};

			// Act
			var result = list.GetEnumerator();
			Assert.IsTrue(result.MoveNext());
			Assert.IsTrue(result.MoveNext());
			Assert.IsTrue(result.MoveNext());
			Assert.IsFalse(result.MoveNext());

			var list2 = new System.Collections.Generic.List<int>();
			foreach (var e in list)
			{
				list2.Add(e);
			}
			Assert.AreEqual(list.Count, list2.Count);
			Assert.AreEqual(list[0], list2[0]);
			Assert.AreEqual(list[1], list2[1]);
			Assert.AreEqual(list[2], list2[2]);
		}

		[Test]
		public void IndexOf_StateUnderTest_ExpectedBehavior()
		{
			// Arrange
			var list = new List<int>()
			{
				2,3,4,5,
			};
			var item = 4;

			// Act
			var result = list.IndexOf(
				item);

			// Assert
			Assert.AreEqual(2, result);
		}

		[Test]
		public void IndexOf_StateUnderTest_ExpectedBehavior1()
		{
			// Arrange
			var list = new List<int>()
			{
				2,3,4,5,
			};
			int value = 23;

			// Act
			var result = list.IndexOf(
				value);

			// Assert
			Assert.AreEqual(-1, result);
		}

		[Test]
		public void Insert_StateUnderTest_ExpectedBehavior()
		{
			// Arrange
			var list = new List<int>()
			{
				2,4,5,
			};
			int index = 1;
			var item = 3;

			// Act
			list.Insert(
				index,
				item);

			// Assert
			Assert.AreEqual(list.Count, 4);
			Assert.AreEqual(list[index], item);
		}

		[Test]
		public void Remove_StateUnderTest_ExpectedBehavior()
		{
			// Arrange
			var list = new List<int>()
			{
				2,3,3,4,5,
			};
			var item = 3;

			// Act
			var result = list.Remove(
				item);

			// Assert
			Assert.AreEqual(list.Count, 4);
		}

		[Test]
		public void Remove_StateUnderTest_ExpectedBehavior1()
		{
			// Arrange
			var list = new List<int>()
			{
				2,3,4,5,6,
			};
			var value = 22;

			// Act
			list.Remove(
				value);

			// Assert
			Assert.AreEqual(list.Count, 5);
		}

		[Test]
		public void RemoveAt_StateUnderTest_ExpectedBehavior()
		{
			// Arrange
			var list = new List<int>()
			{
				2,3,3,4,5,
			};
			int index = 2;

			// Act
			list.RemoveAt(
				index);

			// Assert
			Assert.AreEqual(list.Count, 4);
			Assert.AreEqual(list[2], 4);
		}

		[Test]
		public void pop_StateUnderTest_ExpectedBehavior()
		{
			// Arrange
			var list = new List<int>()
			{
				2,3,4,5,
			};

			// Act
			var result = list.pop();

			// Assert
			Assert.AreEqual(result, 5);
			Assert.AreEqual(list.Count, 3);
			Assert.AreEqual(list[2], 4);
		}

		[Test]
		public void push_StateUnderTest_ExpectedBehavior()
		{
			// Arrange
			var list = new List<int>()
			{
				2,4,5,
			};
			var e = 3;

			// Act
			var len = list.push(
				e);

			// Assert
			Assert.AreEqual(len, 4);
			Assert.AreEqual(list.length, 4);
			Assert.AreEqual(list[3], 3);
		}

		[Test]
		public void reverse_StateUnderTest_ExpectedBehavior()
		{
			// Arrange
			var list = new List<int>()
			{
				2,3,4,5,
			};

			// Act
			var result = list.reverse();

			// Assert
			Assert.AreSame(result, list);
			Assert.AreEqual(list.Count, 4);
			Assert.AreEqual(list[0], 5);
			Assert.AreEqual(list[3], 2);
		}

		[Test]
		public void splice_StateUnderTest_ExpectedBehavior()
		{
			// Arrange
			var list = new List<int>()
			{
				2,6,7,3,4,5,
			};
			int index = 1;
			int count = 2;

			// Act
			var result = list.splice(
				index,
				count);

			// Assert
			Assert.AreEqual(list.Count, 4);
			Assert.AreEqual(list[0], 2);
			Assert.AreEqual(list[1], 3);
			Assert.AreEqual(list[2], 4);
			Assert.AreEqual(list[3], 5);

			Assert.AreEqual(result[0], 6);
			Assert.AreEqual(result[1], 7);
		}

		[Test]
		public void splice2_StateUnderTest_ExpectedBehavior()
		{
			// Arrange
			var list = new List<int>()
			{
				2,3,4,5,6,7,
			};
			int index = 2;

			// Act
			var result = list.splice(
				index);

			// Assert
			Assert.AreEqual(list.Count, 2);
			Assert.AreEqual(list[0], 2);
			Assert.AreEqual(list[1], 3);

			Assert.AreEqual(result[0], 4);
			Assert.AreEqual(result[1], 5);
			Assert.AreEqual(result[2], 6);
			Assert.AreEqual(result[3], 7);
		}

		[Test]
		public void slice_StateUnderTest_ExpectedBehavior()
		{
			// Arrange
			var list = new List<int>()
			{
				1,2,3,4,5,
			};
			int start = 1;
			int end = 4;

			// Act
			var result = list.slice(
				start,
				end);

			// Assert
			Assert.AreEqual(result.Count, end - start);
			Assert.AreEqual(result[0], 2);
			Assert.AreEqual(result[2], 4);
		}

		[Test]
		public void AsList_StateUnderTest_ExpectedBehavior()
		{
			//// Arrange
			//var list = new List();

			//// Act
			//var result = list.AsList();

			//// Assert
			//Assert.Fail();
		}

		[Test]
		public void TryGet_StateUnderTest_ExpectedBehavior()
		{
			// Arrange
			//var list = new List();
			//int index = 0;

			//// Act
			//var result = list.TryGet(
			//    index);

			//// Assert
			//Assert.Fail();
		}

		[Test]
		public void FindAll_StateUnderTest_ExpectedBehavior()
		{
			// Arrange
			//var list = new List();
			//Func p = null;

			//// Act
			//var result = list.FindAll(
			//    p);

			//// Assert
			//Assert.Fail();
		}

		[Test]
		public void Join_StateUnderTest_ExpectedBehavior()
		{
			// Arrange
			//var list = new List();
			//string v = null;

			//// Act
			//var result = list.Join(
			//    v);

			//// Assert
			//Assert.Fail();
		}
	}
}
