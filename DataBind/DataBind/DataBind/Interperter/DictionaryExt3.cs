using System;
using DataBind.VM;

namespace DataBind.CollectionExt
{
	public partial class Dictionary<K, V> : System.Collections.Generic.IDictionary<K, V>, IWithPrototype, IObservableCollection, IHostAccessor
	{
		public object Proto;
		public virtual object _self { get; set; }

		public virtual object GetProto()
		{
			return Proto;
		}

		public virtual void SetProto(object dict)
		{
			this.Proto = dict;
			this._self = dict;
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
		public virtual void NotifyAddRelations(System.Collections.IEnumerable values, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
		{
			this.RelationChanged?.Invoke(this, new RelationChangedEventArgs(propertyName, this, values));
		}
		public virtual void NotifyAddRelation(object value, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
		{
			this.RelationChanged?.Invoke(this, new RelationChangedEventArgs(propertyName, this, new object[] { value }));
		}
		public virtual void NotifyChangedRelation([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
		{
			this.RelationChanged?.Invoke(this, new RelationChangedEventArgs(propertyName, this, null));
		}

		protected Observer ___Sob__;
		public virtual Observer _SgetOb()
		{
			return ___Sob__;
		}

		public virtual void _SsetOb(Observer value)
		{
			___Sob__ = value;
		}

		public System.Collections.Generic.ICollection<Watcher> _Swatchers = new System.Collections.Generic.List<Watcher>();
		public virtual System.Collections.Generic.ICollection<Watcher> GetWatchers()
		{
			return _Swatchers;
		}

	}
}
