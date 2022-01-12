using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using vm;

namespace System.ListExt
{
	using TRawList = System.Collections.Generic.List<object>;
	public partial class List<T> : List, IConvableList,IHostAccessor, ICollection<T>, IEnumerable<T>, IEnumerable, IList<T>, IReadOnlyCollection<T>, IReadOnlyList<T>, ICollection, IObservableCollection
	{
		public List()
		{
			this.list = new TRawList();
		}
		public List(TRawList ls)
		{
			this.list = ls;
		}
		public List(IEnumerable<T> ls)
		{
			this.list = new TRawList();
			ls.ForEach(e => list.Add(e));
		}
		public List(IEnumerable<object> ls)
		{
			this.list = new TRawList();
			ls.ForEach(e => list.Add(e));
		}
		public List(IEnumerable ls)
		{
			this.list = new TRawList();
			foreach (var e in ls)
			{
				list.Add(e);
			}
		}
		public List(int capacity)
		{
			this.list = new TRawList(capacity);
		}
		public List(IConvableList ls)
		{
			this.list = ls.RawList;
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public event PropertyGetEventHandler PropertyGot;
		public event RelationChangedEventHandler RelationChanged;

		public virtual void NotifyPropertyGot(object value, [Runtime.CompilerServices.CallerMemberName] string propertyName = "")
		{
			this.PropertyGot?.Invoke(this, new PropertyGetEventArgs(propertyName, value));
		}
		public virtual void NotifyPropertyChanged(object newValue, object oldValue, [Runtime.CompilerServices.CallerMemberName] string propertyName = "")
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName, newValue, oldValue));
		}
		public virtual void NotifyAddRelations(System.Collections.IEnumerable values, [Runtime.CompilerServices.CallerMemberName] string propertyName = "")
		{
			this.RelationChanged?.Invoke(this, new RelationChangedEventArgs(propertyName, this, values));
		}
		public virtual void NotifyAddRelation(object value, [Runtime.CompilerServices.CallerMemberName] string propertyName = "")
		{
			this.RelationChanged?.Invoke(this, new RelationChangedEventArgs(propertyName, this, new object[] { value }));
		}
		public virtual void NotifyChangedRelation([Runtime.CompilerServices.CallerMemberName] string propertyName = "")
		{
			this.RelationChanged?.Invoke(this, new RelationChangedEventArgs(propertyName, this, null));
		}

		protected Observer ___Sob__;
		public virtual Observer _SgetOb()
		{
			return ___Sob__;
		}

		public virtual Observer _SsetOb(Observer value)
		{
			___Sob__ = value;
			return value;
		}

		public System.Collections.Generic.ICollection<Watcher> _Swatchers = new System.Collections.Generic.List<Watcher>();
		public virtual System.Collections.Generic.ICollection<Watcher> GetWatchers()
		{
			return _Swatchers;
		}

		protected virtual T GetValue(int index)
		{
			return Utils.ConvItem<T>(list[index]);
		}
		public new virtual T this[int index]
		{
			get
			{
				var value=GetValue(index);
				//this.NotifyPropertyGot(value, index.ToString());
				return value;
			}
			set
			{
				var v0 = list[index];
				list[index] = value;
                this.NotifyAddRelation(value);
				//this.NotifyPropertyChanged(value,v0,index.ToString());
            }
		}
		public virtual T this[double index]
		{
			get=>this[(int)index];
			set=>this[(int)index] = value;
		}

		public override int Count => list.Count;

		public override bool IsReadOnly => false;

		public override bool IsSynchronized => false;

		public override object SyncRoot => false;

		public override bool IsFixedSize => false;

		public virtual void Add(T item)
		{
			list.Add(item);
			this.NotifyAddRelation(item);
		}

		public override void Clear()
		{
			list.Clear();
			this.NotifyChangedRelation();
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
			this.NotifyAddRelation(item);
		}

		public override void Insert(int index, object value)
		{
			list.Insert(index, (T)value);
			this.NotifyAddRelation(value);
		}

		public virtual bool Remove(T item)
		{
			var ret = list.Remove(item);
			this.NotifyChangedRelation();
			return ret;
		}

		public override void Remove(object value)
		{
			list.Remove((T)value);
			this.NotifyChangedRelation();
		}

		public override void RemoveAt(int index)
		{
			list.RemoveAt(index);
			this.NotifyChangedRelation();
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
			this.NotifyChangedRelation();
			return p2;
		}

		public virtual int push(T e)
		{
			this.Add(e);
			return list.Count;
		}

		public virtual List<T> reverse()
		{
			this.list.Reverse();
			this.NotifyChangedRelation();
			return this;
		}

		public virtual List<T> splice(int index, int? count0 = null, params T[] inserts)
		{
			int count;
			if (count0 == null)
			{
				count = list.Count-index;
			}
			else
			{
				count = count0.Value;
			}
			var sp = list.GetRange(index, count);
			var spl = new List<T>(sp);
			list.RemoveRange(index, count);
			if (inserts != null)
			{
				for (var i = 0; i < inserts.Length; i++)
				{
					list.Insert(index + i, inserts[i]);
				}
			}
			this.NotifyAddRelations(inserts);
			return spl;
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
			var cp = list.GetRange(start0, end0 - start0);
			var cpl = new List<T>(cp);
			return cpl;
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
				return GetValue(index);
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
			var ls = new TRawList();
			ls.AddRange(list);
			return new List<T>(ls);
		}

	}
}
