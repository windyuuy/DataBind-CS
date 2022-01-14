using System;
using vm;

namespace System.ListExt
{
	public partial class Dictionary<K, V> : System.Collections.Generic.IDictionary<K, V>, IWithPrototype, IObservableCollection, IHostAccessor
	{
		public object Proto;
		public virtual object _ { get; set; }

        public object GetProto()
        {
            return Proto;
        }

        public void SetProto(object dict)
		{
			this.Proto = dict;
			this._ = dict;
		}

		public event RelationChangedEventHandler RelationChanged;
		public event PropertyChangedEventHandler PropertyChanged;
		public event PropertyGetEventHandler PropertyGot;

		public virtual void NotifyPropertyGot(object value, string propertyName = "")
		{
			this.PropertyGot?.Invoke(this, new PropertyGetEventArgs(propertyName, value));
		}
		public virtual void NotifyPropertyChanged(object newValue, object oldValue, string propertyName = "")
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

	}
}
