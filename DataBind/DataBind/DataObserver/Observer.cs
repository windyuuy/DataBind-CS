using System;
using System.ListExt;
using System.Linq;


namespace vm
{
	public class PropertyChangedEventArgs : EventArgs
	{
		public PropertyChangedEventArgs(string propertyName, object newValue, object oldValue)
		{
			this.PropertyName = propertyName;
			this.newValue = newValue;
			this.oldValue = oldValue;
		}

		public virtual string PropertyName { get; }
		public object oldValue;
		public object newValue;

	}
	public class PropertyGetEventArgs : EventArgs
	{
		public PropertyGetEventArgs(string propertyName, object value)
		{
			this.PropertyName = propertyName;
			this.value = value;
		}

		public virtual string PropertyName { get; }
		public object value;
	}
	public delegate void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e);
	public delegate void PropertyGetEventHandler(object sender, PropertyGetEventArgs e);
	public interface IObserved
	{
		// Observer __ob__ { get; set; }
		Observer _SgetOb();
		Observer _SsetOb(Observer value);
	}
	public interface IObservable : IObserved
	{
		event PropertyChangedEventHandler PropertyChanged;
		event PropertyGetEventHandler PropertyGot;
	}
	public partial class Utils
	{
		/// <summary>
		/// 是否可观察对象
		/// </summary>
		/// <returns></returns>
		public static bool IsObservable(object obj)
		{
			return obj is IObservable;
		}

		public static bool IsCollection(object obj)
		{
			return obj is System.Collections.IEnumerable;
		}

		public static bool IsObserved(object obj)
		{
			if (obj is IObservable)
			{
				var obj1 = (IObservable)obj;
				if (obj1._SgetOb() is Observer)
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsObserved(IObservable obj)
		{
			if (obj != null)
			{
				if (obj._SgetOb() is Observer)
				{
					return true;
				}
			}
			return false;
		}

		/**
		 * 将对象处理为可观察对象
		 */
		public static Observer observe(object value)
		{
			if (value is Observer)
			{
				return (Observer)value;
			}
			else if (IsObservable(value))
			{
				//只有普通的对象才可以进行观察
				return new Observer(AsObservable(value));
			}
			return null;
		}

		public static IObservable AsObservable(object value)
		{
			return (IObservable)value;
		}
		public static List<object> AsCollection(object value)
		{
			return (List<object>)value;
		}

		/**
		 * 拦截对象所有的key和value
		 */
		public static void defineReactive(
				object obj0,
				string key,
				/**
				 * 对象的默认值，也就是 obj[key]
				 */
				object val
			)
		{
			if (!IsObservable(obj0))
			{
				return;
			}

			//必包的中依赖，相当于是每一个属性的附加对象，用于记录属性的所有以来侦听。
			var dep = new Dep();
			var childOb = observe(val);
			var obj = Utils.AsObservable(obj0);
			obj.PropertyGot += (sender, e) =>
			{
				if (e.PropertyName != key)
				{
					return;
				}
				//进行依赖收集，依赖收集前 Dependency.collectTarget 会被赋值，收集完成后会置空。
				if (Dep.target != null)
				{
					dep.asCurTargetDepend();//将自身加入到Dependency.collectTarget中

					if (childOb != null)
					{
						childOb.dep.asCurTargetDepend();
						var value = e.value;
						if (Utils.IsCollection(value))
						{
							Dep.dependArray(Utils.AsCollection(value));
						}
					}
				}
			};
			obj.PropertyChanged += (sender, e) =>
			{
				if (e.PropertyName != key)
				{
					return;
				}
				var newVal = e.newValue;
				childOb = observe(newVal);//如果是普通对象需要处理成可观察的
				dep.notifyWatchers();//触发刷新
			};
		}
	}

	public class Observer
	{
		public IObservable value;
		public Dep dep;
		public Observer(
			IObservable value
		)
		{
			this.value = value;
			this.dep = new Dep();

			//实现双向绑定
			value._SsetOb(this);

			{
				/*如果是对象则直接walk进行绑定*/
				this.walk(value);
			}
		}

		/**
		 * 遍历所有属性，拦截get set
		 */
		public void walk(object obj)
		{
			var type = obj.GetType();
			var keys = type.GetProperties(
				System.Reflection.BindingFlags.Instance
				| System.Reflection.BindingFlags.Public
				);
			for (var i = 0; i < keys.Length; i++)
			{
				var key = keys[i];
				var value = key.GetValue(obj);
				Utils.defineReactive(obj, key.Name, value);
			}
		}

		/**
         * 所以成员都替换成observe
         */
		public void observeArray(List<object> items)
		{
			for (int i = 0, l = items.length; i < l; i++)
			{
				Utils.observe(items[i]);
			}
		}

	}
}