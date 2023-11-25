using System;
using System.Linq;
using DataBinding.CollectionExt;
using System.Runtime.CompilerServices;
using Game.Diagnostics.IO;
using Console = Game.Diagnostics.IO.Console;

namespace vm
{
	using boolean = System.Boolean;
	using number = System.Double;

	public class WatchAnnotation
	{
		public CombineType<string, Action> expOrFn;
		public Action<object, object> cb;
	}

	public class Host : IHost, IHostAccessor, IWithDestroyState, IObserved, IObservable, IWithPrototype
	{

		protected Observer ___Sob__;
		public System.Collections.Generic.ICollection<Watcher> _Swatchers;
		public boolean _SisDestroyed;

		public virtual bool _SIsDestroyed { get => _SisDestroyed; set => _SisDestroyed = value; }

		public virtual System.Collections.Generic.ICollection<Watcher> GetWatchers()
		{
			return _Swatchers;
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public event PropertyGetEventHandler PropertyGot;

		public virtual void NotifyPropertyGot(object value, [CallerMemberName] string propertyName = "")
		{
			this.PropertyGot?.Invoke(this, new PropertyGetEventArgs(propertyName, value));
		}
		public virtual void NotifyPropertyChanged(object newValue, object oldValue, [CallerMemberName] string propertyName = "")
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName, newValue, oldValue));
		}

		public Host()
		{
			//防止产生枚举
			// Utils.def(this, "_Swatchers", new List<Watcher>());
			// Utils.def(this, "_SisDestroyed", false);
			this._Swatchers = new System.Collections.Generic.List<Watcher>();
			this._SisDestroyed = false;

			//实现基础方法，用于表达式中方便得调用
			// Utils.implementEnvironment(this);
		}

		public virtual Watcher _Swatch(CombineType<object, string, Func<object, object, object>> expOrFn, Action<object, object, object> cb, CombineType<object, string, number, boolean> loseValue = null, boolean sync = false)
		{
			if (this._SisDestroyed)
			{
				Console.Error("the host is destroyed", this);
				return null;
			}
			if (!Utils.IsObserved(this))
			{
				Utils.Observe(this);
			}
			var watcher = new Watcher(this, expOrFn, cb, new WatchOptions()
			{
				loseValue = loseValue,
				sync = sync,
			});

			this._Swatchers.Add(watcher);
			return watcher;
		}

		public virtual void _Sdestroy()
		{
			var temp = this._Swatchers.ToArray();
			this._Swatchers.Clear();
			foreach (var w in temp)
			{
				w.teardown();
			}

			this._SisDestroyed = true;
		}

		public virtual void _SremoveWatcher(Watcher watcher)
		{
			this._Swatchers.Remove(watcher);
		}

		public virtual void _SaddWatcher(Watcher watcher)
		{
			this._Swatchers.Add(watcher);
		}

		public virtual Observer _SgetOb()
		{
			return ___Sob__;
		}

		public virtual void _SsetOb(Observer value)
		{
			___Sob__ = value;
		}

		#region IWithPrototype
		public object Proto;
		public virtual object _ { get; set; }

		public virtual void SetProto(object dict)
		{
			this.Proto = dict;
			this._ = dict;
		}

		public virtual object GetProto()
		{
			return Proto;
		}
		#endregion

	}

	public partial class Utils
	{

		/**
		 * 向普通对象注入Host相关方法
		 */
		public static IHost implementHost<T>(T obj)
		{
			if (obj is IObservable)
			{
				//实现基础方法，用于表达式中方便得调用
				if (obj is IWithPrototype env)
				{
					InterpreterEnv.ImplementEnvironment(env);
				}

				Observe(obj);
			}
			return obj as IHost;
		}

		/**
		 * 向普通对象注入Host相关方法
		 */
		public static DataBinding.IStdHost implementStdHost<T>(T obj)
		{
			if (obj is IObservable)
			{
				//实现基础方法，用于表达式中方便得调用
				if (obj is IWithPrototype obj1)
				{
					InterpreterEnv.ImplementEnvironment(obj1);
				}

				Observe(obj);
			}
			return obj as DataBinding.IStdHost;
		}

	}

}