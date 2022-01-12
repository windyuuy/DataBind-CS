using System;
using System.Collections;
using System.Text;

namespace System.ListExt
{
	public interface IConvableList
	{
		System.Collections.Generic.List<object> RawList { get; }
	}

	public class List : IList, ICollection, IConvableList
	{
		protected System.Collections.Generic.List<object> list;
		public virtual System.Collections.Generic.List<object> RawList => list;

		public virtual object this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public virtual bool IsFixedSize => throw new NotImplementedException();

		public virtual bool IsReadOnly => throw new NotImplementedException();

		public virtual int Count => throw new NotImplementedException();

		public virtual bool IsSynchronized => throw new NotImplementedException();

		public virtual object SyncRoot => throw new NotImplementedException();

		public static bool isList(object obj)
		{
			return obj is List;
		}

		public virtual int Add(object value)
		{
			throw new NotImplementedException();
		}

		public virtual void Clear()
		{
			throw new NotImplementedException();
		}

		public virtual bool Contains(object value)
		{
			throw new NotImplementedException();
		}

		public virtual void CopyTo(Array array, int index)
		{
			throw new NotImplementedException();
		}

		public virtual IEnumerator GetEnumerator()
		{
			throw new NotImplementedException();
		}

		public virtual int IndexOf(object value)
		{
			throw new NotImplementedException();
		}

		public virtual void Insert(int index, object value)
		{
			throw new NotImplementedException();
		}

		public virtual void Remove(object value)
		{
			throw new NotImplementedException();
		}

		public virtual void RemoveAt(int index)
		{
			throw new NotImplementedException();
		}
	}
}
