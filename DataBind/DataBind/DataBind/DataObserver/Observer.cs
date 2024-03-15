using System;
using DataBind.CollectionExt;
using System.Linq;
using EngineAdapter.Diagnostics;
using Console = EngineAdapter.Diagnostics.Console;


namespace DataBind.VM
{
	using TDictGotFuncs = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<PropertyGetEventHandler>>;
	using TGotFuncs = System.Collections.Generic.List<PropertyGetEventHandler>;
	using TDictChangedFuncs = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<PropertyChangedEventHandler>>;
	using TChangedFuncs = System.Collections.Generic.List<PropertyChangedEventHandler>;

	// TODO: 支持下对象销毁时, 把相关的表达式一起禁用掉(消除错误报告), 对象重新出现时, 相关表达式一起自动启用
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
			if (obj is IObservable obj1)
			{
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
		public static Observer Observe(object value)
		{
			if (value is Observer value1)
			{
				return value1;
			}
			else if (IsObservable(value))
			{
				if (IsObserved(value))
				{
					return ((IObservable)value)._SgetOb();
				}
				else
				{
					//只有普通的对象才可以进行观察
					return new Observer(AsObservable(value));
				}
			}
			else if(value != null && (
				value.GetType().IsPrimitive
				|| (value is string)
				|| (value is Delegate)
				|| (value.GetType().GetProperties().Length==0 && value.GetType().GetFields().Length==0)
				|| value.GetType().Name == "RuntimeMethodInfo"
				) ==false)
			{
				Console.Error($"该对象类型不可监测: type={value.GetType().Name}");
			}
			return null;
		}

		public static IObservable AsObservable(object value)
		{
			return (IObservable)value;
		}
		public static IObservableCollection AsObservableCollection(object value)
		{
			return (IObservableCollection)value;
		}
		public static bool IsObservableCollection(object value)
		{
			return value is IObservableCollection;
		}
		public static System.Collections.IEnumerable AsCollection(object value)
		{
			return (System.Collections.IEnumerable)value;
		}

		/**
		 * 拦截对象所有的key和value
		 */
		public static void DefineReactive(
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
			var childOb = Observe(val);
			var obj = Utils.AsObservable(obj0);
			var ob = obj._SgetOb();
			System.Diagnostics.Debug.Assert(ob != null);

			{
				PropertyGetEventHandler handle = (sender, e) =>
				 {
					 System.Diagnostics.Debug.Assert(e.PropertyName == key);
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
							 var value = e.Value;
							 if (Utils.IsCollection(value))
							 {
								 Dep.dependCollection(Utils.AsCollection(value));
							 }
						 }
					 }
				 };
				//var weakRef=new WeakReference<PropertyGetEventHandler>(handle);
				var DictGotFuncs = ob.DictGotFuncs;
				TGotFuncs funcs;
				lock (DictGotFuncs)
				{
					DictGotFuncs.TryGetValue(key, out funcs);
					if (funcs == null)
					{
						funcs = new TGotFuncs();
						DictGotFuncs.Add(key, funcs);
					}
				}
				if (funcs != null)
				{
					lock (funcs)
					{
						funcs.Add(handle);
					}
				}
			}
			{
				PropertyChangedEventHandler handle = (sender, e) =>
				{
					System.Diagnostics.Debug.Assert(e.PropertyName == key);
					if (e.PropertyName != key)
					{
						return;
					}
					var newVal = e.NewValue;
					childOb = Observe(newVal);//如果是普通对象需要处理成可观察的
					dep.notifyWatchers();//触发刷新
				};
				//var weakRef = new WeakReference<PropertyChangedEventHandler>(handle);
				var DictChangedFuncs = ob.DictChangedFuncs;
				TChangedFuncs funcs;
				lock (DictChangedFuncs)
				{
					DictChangedFuncs.TryGetValue(key, out funcs);
					if (funcs == null)
					{
						funcs = new TChangedFuncs();
						DictChangedFuncs.Add(key, funcs);
					}
				}
				if (funcs != null)
				{
					lock (funcs)
					{
						funcs.Add(handle);
					}
				}
			}
		}
	}

	public class Observer
	{
		public IObservable value;
		public Dep dep;
		public TDictGotFuncs DictGotFuncs = new TDictGotFuncs();
		public TDictChangedFuncs DictChangedFuncs = new TDictChangedFuncs();

		public Observer(
			IObservable value
		)
		{
			this.value = value;
			this.dep = new Dep();

			//实现双向绑定
			value._SsetOb(this);
			value.PropertyGot += OnPropertyGot;
			value.PropertyChanged += OnPropertyChanged;

			//if (value is System.Collections.IEnumerable)
			//{
			//    this.observeCollection((System.Collections.IEnumerable)value);
			//}

			{
				this.Walk(value);
			}
		}

		protected virtual void OnPropertyGot(object sender, PropertyGetEventArgs e)
		{
			TGotFuncs funcs;
			lock (DictGotFuncs)
			{
				DictGotFuncs.TryGetValue(e.PropertyName, out funcs);
			}
			if (funcs != null)
			{
				PropertyGetEventHandler[] funcsArr;
				lock (funcs)
				{
					funcsArr = funcs.ToArray();
				}
				foreach (var f in funcsArr)
				{
					f(sender, e);
				}
			}
		}

		protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			TChangedFuncs funcs;
			lock (DictChangedFuncs)
			{
				DictChangedFuncs.TryGetValue(e.PropertyName, out funcs);
			}
			if (funcs != null)
			{
				PropertyChangedEventHandler[] funcsArr;
				lock (funcs)
				{
					funcsArr = funcs.ToArray();
				}
				foreach (var f in funcsArr)
				{
					f(sender, e);
				}
			}
		}

		protected virtual void ObserveNormalObject(object obj,System.Reflection.PropertyInfo[] keys)
        {
			var type = obj.GetType();
			for (var i = 0; i < keys.Length; i++)
			{
				var key = keys[i];
				if (key.GetIndexParameters().Length == 0)
				{
					var value = key.GetValue(obj);
					Utils.DefineReactive(obj, key.Name, value);
				}
				else
				{
					// 过滤 Item 属性
				}
			}
		}
		/**
		 * 遍历所有属性，拦截get set
		 */
		public void Walk(object obj)
		{
			var type = obj.GetType();
			var keys = type.GetProperties(
				System.Reflection.BindingFlags.Instance
				| System.Reflection.BindingFlags.Public
				);

			// 监听集合事件
			if (Utils.IsObservableCollection(obj))
			{
				if (obj is System.Collections.IDictionary dict)
				{
					//var IDictKeysDict = new Dictionary<string, bool>();
					//typeof(System.Collections.IDictionary).GetProperties(
					//	System.Reflection.BindingFlags.Instance
					//	| System.Reflection.BindingFlags.Public).ForEach(k=>IDictKeysDict[k.Name] = true);

					//var keys2 = keys.Where(k => IDictKeysDict.ContainsKey(k.Name)==false);
					//ObserveNormalObject(obj, keys);
					foreach (var key in dict.Keys)
					{
						var value = dict[key];
						Utils.DefineReactive(obj, Utils.ToIndexKey(key), value);
					}
				}
				else if (obj is System.Collections.IList list)
				{
					//ObserveNormalObject(obj, keys);
					for (int index = 0; index < list.Count; index++)
					{
						var value = list[index];
						Utils.DefineReactive(obj, index.ToString(), value);
					}
				}
				else if (obj is System.Collections.IEnumerable items)
				{
					//ObserveNormalObject(obj, keys);
					foreach (var item in items)
					{
						Utils.Observe(item);
					}
				}

				var objColl = Utils.AsObservableCollection(obj);
				objColl.RelationChanged += (sender, e) =>
				{
					var ob = e.Host._SgetOb();
					if (e.NewItems != null)
					{
						ob.ObserveCollection(e.NewItems);
					}
					ob.dep.notifyWatchers();
				};
            }
            else
            {
				ObserveNormalObject(obj, keys);
			}
		}

		/**
		 * 所以成员都替换成observe
		 */
		public void ObserveCollection(System.Collections.IEnumerable items)
		{
			if (items is System.Collections.IDictionary dict)
			{
				foreach (var item in dict.Values)
				{
					Utils.Observe(item);
				}
			}
			else
			{
				foreach (var item in items)
				{
					Utils.Observe(item);
				}
			}
		}

	}
}