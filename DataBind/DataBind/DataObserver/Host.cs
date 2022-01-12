using System;
using System.Collections.Generic;
using System.Linq;
using System.ListExt;
using System.Runtime.CompilerServices;

namespace vm
{
	using boolean = System.Boolean;
	using number = System.Double;

	public class WatchAnnotation
	{
		public CombineType<string, Action> expOrFn;
		public Action<object, object> cb;
	}

	public interface IDestroyState
	{
		bool IsDestroyed { get; set; }
	}
	public interface IHostAccessor
	{
		System.Collections.Generic.ICollection<Watcher> GetWatchers();
	}

	public static class HostExt
	{
		/**
         * 侦听一个数据发生的变化
         * @param expOrFn 访问的数据路径，或数据值的计算函数，当路径中的变量或计算函数所访问的值发生变化时，将会被重新执行
         * @param cb 重新执行后，发生变化则会出发回调函数
         */
		public static Watcher _Swatch(this IHostAccessor self, CombineType<object, string, Func<object, object, object>> expOrFn, Action<object, object, object> cb, CombineType<object, string, number, boolean> loseValue = null, boolean sync = false)
		{
			if (self is IDestroyState)
			{
				if ((self as IDestroyState).IsDestroyed)
				{
					console.error("the host is destroyed", self);
					return null;
				}
			}

			if (!Utils.IsObserved(self))
			{
				Utils.observe(self);
			}
			var watcher = new Watcher(self, expOrFn, cb, new WatchOptions()
			{
				loseValue = loseValue,
				sync = sync,
			});

			self.GetWatchers().Add(watcher);
			return watcher;
		}

		/**
         * 释放host，包括所有watch
         */
		public static void _Sdestroy(this IHostAccessor self)
		{

			var temp = self.GetWatchers().ToArray();
			self.GetWatchers().Clear();
			foreach (var w in temp)
			{
				w.teardown();
			}

			if (self is IDestroyState)
			{
				(self as IDestroyState).IsDestroyed = true;
			}
		}

		public static void _SremoveWatcher(this IHostAccessor self, Watcher watcher)
		{
			self.GetWatchers().Remove(watcher);
		}

		public static void _SaddWatcher(this IHostAccessor self, Watcher watcher)
		{
			self.GetWatchers().Add(watcher);
		}

	}

	public interface IHost
	{

		/**
         * 侦听一个数据发生的变化
         * @param expOrFn 访问的数据路径，或数据值的计算函数，当路径中的变量或计算函数所访问的值发生变化时，将会被重新执行
         * @param cb 重新执行后，发生变化则会出发回调函数
         */
		Watcher _Swatch(CombineType<object, string, Func<object, object, object>> expOrFn, Action<object, object, object> cb, CombineType<object, string, number, boolean> loseValue, boolean sync);

		/**
         * 释放host，包括所有watch
         */
		void _Sdestroy();

		void _SremoveWatcher(Watcher watcher);
		void _SaddWatcher(Watcher watcher);
	}

	public class Host : IHost, IHostAccessor, IDestroyState, IObserved, IObservable, IWithPrototype
	{

		protected Observer ___Sob__;
		public System.Collections.Generic.ICollection<Watcher> _Swatchers;
		public boolean _SisDestroyed;

		public bool IsDestroyed { get => _SisDestroyed; set => _SisDestroyed = value; }

		public virtual System.Collections.Generic.ICollection<Watcher> GetWatchers()
		{
			return _Swatchers;
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public event PropertyGetEventHandler PropertyGot;

		public void NotifyPropertyGot(object value, [CallerMemberName] string propertyName = "")
		{
			this.PropertyGot?.Invoke(this, new PropertyGetEventArgs(propertyName, value));
		}
		public void NotifyPropertyChanged(object newValue, object oldValue, [CallerMemberName] string propertyName = "")
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

		public Watcher _Swatch(CombineType<object, string, Func<object, object, object>> expOrFn, Action<object, object, object> cb, CombineType<object, string, number, boolean> loseValue = null, boolean sync = false)
		{
			if (this._SisDestroyed)
			{
				console.error("the host is destroyed", this);
				return null;
			}
			if (!Utils.IsObserved(this))
			{
				Utils.observe(this);
			}
			var watcher = new Watcher(this, expOrFn, cb, new WatchOptions()
			{
				loseValue = loseValue,
				sync = sync,
			});

			this._Swatchers.Add(watcher);
			return watcher;
		}

		public void _Sdestroy()
		{
			var temp = this._Swatchers;
			this._Swatchers = new System.Collections.Generic.List<Watcher>();
			foreach (var w in temp)
			{
				w.teardown();
			}

			this._SisDestroyed = true;
		}

		public void _SremoveWatcher(Watcher watcher)
		{
			this._Swatchers.Remove(watcher);
		}

		public void _SaddWatcher(Watcher watcher)
		{
			this._Swatchers.Add(watcher);
		}

		public Observer _SgetOb()
		{
			return ___Sob__;
		}

		public Observer _SsetOb(Observer value)
		{
			___Sob__ = value;
			return value;
		}

		#region IWithPrototype
		public object Proto;
		public virtual object _ { get; set; }

		public void SetProto(object dict)
		{
			this.Proto = dict;
			this._ = dict;
		}

        public object GetProto()
        {
			return Proto;
        }
        #endregion

    }

	[System.AttributeUsage(System.AttributeTargets.All, Inherited = false, AllowMultiple = true)]
	sealed class ImplementHostAttribute : System.Attribute
	{
		// This is a positional argument
		public ImplementHostAttribute(string positionalString)
		{
		}
	}
}