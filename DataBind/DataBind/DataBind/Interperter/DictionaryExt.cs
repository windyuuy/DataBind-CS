using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using vm;

namespace DataBinding.CollectionExt
{
	public partial class Dictionary : System.Collections.IDictionary, IConvableDictionary
	{
		public virtual object RawDict => throw new NotImplementedException();
		private IDictionary InnerDict => ((IDictionary)RawDict);
		[Obsolete]
		public virtual object this[object key] { get => InnerDict[key]; set => throw new NotImplementedException(); }

		public virtual bool IsFixedSize => InnerDict.IsFixedSize;

		public virtual bool IsReadOnly => InnerDict.IsReadOnly;

		public virtual ICollection Keys => InnerDict.Keys;

		public virtual ICollection Values => new DataBinding.CollectionExt.List<object>(InnerDict.Values);

		public virtual int Count => InnerDict.Count;

		public virtual bool IsSynchronized => InnerDict.IsSynchronized;

		public virtual object SyncRoot => InnerDict.SyncRoot;

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
			return InnerDict.GetEnumerator();
		}
	}
}
