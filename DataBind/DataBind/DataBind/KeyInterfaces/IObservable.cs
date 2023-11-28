using System;
using DataBinding.CollectionExt;
using System.Linq;


namespace VM
{
	[System.Diagnostics.DebuggerStepThrough]
	public class PropertyChangedEventArgs : EventArgs
	{
		[System.Diagnostics.DebuggerStepThrough]
		public PropertyChangedEventArgs(string propertyName, object newValue, object oldValue)
		{
			this.PropertyName = propertyName;
			this.NewValue = newValue;
			this.OldValue = oldValue;
		}

		public virtual string PropertyName { get; }
		public object OldValue;
		public object NewValue;

	}
	[System.Diagnostics.DebuggerStepThrough]
	public class PropertyGetEventArgs : EventArgs
	{
		[System.Diagnostics.DebuggerStepThrough]
		public PropertyGetEventArgs(string propertyName, object value)
		{
			this.PropertyName = propertyName;
			this.Value = value;
		}

		public virtual string PropertyName { get; }
		public object Value;
	}
	[System.Diagnostics.DebuggerStepThrough]
	public class RelationChangedEventArgs : EventArgs
	{
		public RelationChangedEventArgs(string propertyName, IObservable host, System.Collections.IEnumerable newItems)
		{
			this.PropertyName = propertyName;
			this.NewItems = newItems;
			this.Host = host;
		}

		public IObservable Host;
		public virtual string PropertyName { get; }
		public System.Collections.IEnumerable NewItems;
	}
	public delegate void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e);
	public delegate void PropertyGetEventHandler(object sender, PropertyGetEventArgs e);
	public delegate void RelationChangedEventHandler(object sender, RelationChangedEventArgs e);

	public interface IObserved
	{
		// Observer ___Sob__ { get; set; }
		Observer _SgetOb();
		void _SsetOb(Observer value);
	}

	public interface IObservableEvents
	{
		event PropertyChangedEventHandler PropertyChanged;
		event PropertyGetEventHandler PropertyGot;
	}

	/// <summary>
	/// 标记可观察对象
	/// </summary>
	public interface IObservable : IObserved, IObservableEvents
	{
	}
	/// <summary>
	/// 标记可观察容器对象
	/// </summary>
	public interface IObservableCollection : IObservable
	{
		event RelationChangedEventHandler RelationChanged;
	}

	public interface IObservableEventDelegate
	{
		void NotifyPropertyGot(object value, string propertyName);
		void NotifyPropertyChanged(object newValue, object oldValue, string propertyName);
	}
}

namespace DataBinding
{

	/// <summary>
	/// 标记可观察对象
	/// </summary>
	[System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
	public sealed class ObservableAttribute : System.Attribute
	{
		/// <summary>
		/// 状态：0：可观察，1：已观察，2：无需观察
		/// </summary>
		public int ObserveState = 0;
		public ObservableAttribute(int ObserveState = 0)
		{
			this.ObserveState = ObserveState;
		}
	}

	/// <summary>
	/// 标记可观察对象
	/// - 所有属性对应的类型也会被标记为可观察
	/// </summary>
	[System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
	public sealed class ObservableRecursiveAttribute : System.Attribute
	{
		public ObservableRecursiveAttribute()
		{
		}
	}

}
