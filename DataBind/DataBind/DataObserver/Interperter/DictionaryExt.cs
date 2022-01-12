using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using vm;

namespace System.ListExt
{
	public partial class Dictionary : System.Collections.IDictionary, IConvableDictionary
	{
		public virtual object RawDict => throw new NotImplementedException();
		public virtual object this[object key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public virtual bool IsFixedSize => throw new NotImplementedException();

		public virtual bool IsReadOnly => throw new NotImplementedException();

		public virtual ICollection Keys => throw new NotImplementedException();

		public virtual ICollection Values => throw new NotImplementedException();

		public virtual int Count => throw new NotImplementedException();

		public virtual bool IsSynchronized => throw new NotImplementedException();

		public virtual object SyncRoot => throw new NotImplementedException();

		public virtual void Add(object key, object value)
		{
			throw new NotImplementedException();
		}

		public virtual void Clear()
		{
			throw new NotImplementedException();
		}

		public virtual bool Contains(object key)
		{
			throw new NotImplementedException();
		}

		public virtual void CopyTo(Array array, int index)
		{
			throw new NotImplementedException();
		}

		public virtual IDictionaryEnumerator GetEnumerator()
		{
			throw new NotImplementedException();
		}

		public virtual void Remove(object key)
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}
	}
}
