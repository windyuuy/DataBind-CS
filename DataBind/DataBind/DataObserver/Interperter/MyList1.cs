using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using vm;

namespace System.ListExt
{
	public partial class List<T> : List, IConvableList, ICollection<T>, IEnumerable<T>, IEnumerable, IList<T>, IReadOnlyCollection<T>, IReadOnlyList<T>, ICollection
	{
		public List()
		{
			this.list = new Collections.Generic.List<object>();
		}
		public List(System.Collections.Generic.List<object> ls)
		{
			this.list = ls;
		}
		public List(IEnumerable<T> ls)
		{
			this.list = new Collections.Generic.List<object>();
			ls.ForEach(e => list.Add(e));
		}
		public List(IEnumerable<object> ls)
		{
			this.list = new Collections.Generic.List<object>();
			ls.ForEach(e => list.Add(e));
		}
		public List(int capacity)
		{
			this.list = new Collections.Generic.List<object>(capacity);
		}
		public List(IConvableList ls)
		{
			this.list = ls.RawList;
		}
		public new virtual T this[int index] { get => (T)list[index]; set => list[index] = value; }

		public override int Count => list.Count;

		public override bool IsReadOnly => false;

		public override bool IsSynchronized => false;

		public override object SyncRoot => false;

		public override bool IsFixedSize => false;

		public virtual void Add(T item)
		{
			list.Add(item);
		}

		public override void Clear()
		{
			list.Clear();
		}

		public virtual bool Contains(T item)
		{
			return list.Contains(item);
		}

		public override bool Contains(object value)
		{
			return list.Contains((T)value);
		}

		public virtual void CopyTo(T[] array, int arrayIndex)
		{
			// list.CopyTo(array, arrayIndex);
			for (int i = arrayIndex; i < arrayIndex + list.Count; i++)
			{
				array[i] = (T)list[i - arrayIndex];
			}
		}

		public override void CopyTo(Array array, int index)
		{
			// list.CopyTo((T[])array, index);
			throw new Exception("not implemented");
		}

		public new virtual IEnumerator<T> GetEnumerator()
		{
			// TODO: 优化性能
			// return list.GetEnumerator();
			var cp = new System.Collections.Generic.List<T>();
			list.ForEach(e => cp.Add(ConvItem(e)));
			return cp.GetEnumerator();
		}

		public virtual int IndexOf(T item)
		{
			return list.IndexOf(item);
		}

		public override int IndexOf(object value)
		{
			var v = (T)value;
			return list.IndexOf(v);
		}

		public virtual void Insert(int index, T item)
		{
			list.Insert(index, item);
		}

		public override void Insert(int index, object value)
		{
			list.Insert(index, (T)value);
		}

		public virtual bool Remove(T item)
		{
			return list.Remove(item);
		}

		public override void Remove(object value)
		{
			list.Remove((T)value);
		}

		public override void RemoveAt(int index)
		{
			list.RemoveAt(index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return list.GetEnumerator();
		}

		public virtual int length => this.Count;

		public virtual T pop()
		{
			if (list.Count <= 0)
			{
				return default(T);
			}

			var index = list.Count - 1;
			var p = list[index];
			this.list.RemoveAt(index);
			var p2 = (T)p;
			return p2;
		}

		public virtual int push(T e)
		{
			this.list.Add(e);
			return list.Count;
		}

		public virtual List<T> reverse()
		{
			this.list.Reverse();
			return this;
		}

		public virtual List<T> splice(int index, int count)
		{
			var sp = new List<T>(count);
			for (int i = 0; i < count; i++)
			{
				sp.Add((T)list[index]);
				this.RemoveAt(index);
			}
			return sp;
		}

		public virtual List<T> slice(int? start = null, int? end = null)
		{
			var start0 = start != null ? start.Value : 0;
			var end0 = end != null ? end.Value : list.Count;
			var size = end0 - start0;
			if (size < 0)
			{
				size = 0;
			}
			var cp = new List<T>(size);
			for (int i = start0; i < end0; i++)
			{
				cp.Add((T)list[i]);
			}
			return cp;
		}

		public virtual List<F> AsList<F>()
		{
			return new List<F>(list);
		}

		public virtual Array ToArray(Type t)
		{
			var arr = Array.CreateInstance(t, list.Count);
			for (var i = 0; i < list.Count; i++)
			{
				var f = list[i];
				var v2 = System.Convert.ChangeType(f, t);
				arr.SetValue(v2, i);
			}
			return arr;
		}

		public virtual T ConvItem(object value)
		{
			return Utils.ConvItem<T>(value);
		}

		public virtual T TryGet(int index)
		{
			if (list.Count > index && index >= 0)
			{
				return ConvItem(list[index]);
			}
			else
			{
				return default(T);
			}
		}

		public static implicit operator List<object>(List<T> self)
		{
			return new List<object>(self.list);
		}
		public static implicit operator List<T>(List<object> self)
		{
			return new List<T>(self.list);
		}
		public static implicit operator T[](List<T> self)
		{
			var arr = new T[self.Count];
			self.CopyTo(arr, 0);
			return arr;
		}
		public static implicit operator System.Object[](List<T> self)
		{
			var arr = new System.Object[self.Count];
			self.CopyTo(arr, 0);
			return arr;
		}

		public virtual List<T> FindAll(Func<T, bool> p)
		{
			var cp = new List<T>();
			foreach (var item in list)
			{
				if (p((T)item))
				{
					cp.Add((T)item);
				}
			}
			return cp;
		}

		public virtual string Join(string v)
		{
			return string.Join(v, list);
		}

		internal List<T> Clone()
		{
			var ls = new System.Collections.Generic.List<object>();
			ls.AddRange(list);
			return new List<T>(ls);
		}
	}
}
