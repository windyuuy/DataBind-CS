using System;
using System.Reflection;
using Game.Diagnostics;
using VM;

namespace DataBinding
{
	using number = System.Double;

	/// <summary>
	/// IStdHost 代理
	/// </summary>
	public static class HostExt2
	{
		/**
         * 侦听一个数据发生的变化
         * @param expOrFn 访问的数据路径，或数据值的计算函数，当路径中的变量或计算函数所访问的值发生变化时，将会被重新执行
         * @param cb 重新执行后，发生变化则会出发回调函数
         */
		public static Watcher Watch(IStdHost self, CombineType<object, string, Func<object, object, object>> expOrFn, Action<object, object, object> cb, CombineType<object, string, number, bool> loseValue = null, bool sync = false)
		{
			return HostExt._Swatch((IHostAccessor)self, expOrFn, cb, loseValue, sync);
		}

		/**
         * 侦听一个数据发生的变化
         * @param expOrFn 访问的数据路径，或数据值的计算函数，当路径中的变量或计算函数所访问的值发生变化时，将会被重新执行
         * @param cb 重新执行后，发生变化则会出发回调函数
         */
		public static Watcher _Watch0(IStdHost self, CombineType<object, string, Func<object, object, object>> expOrFn, Action<object, object, object> cb, CombineType<object, string, number, bool> loseValue = null, bool sync = false)
		{
			return Watch(self, expOrFn, cb, loseValue, sync);
		}

		/**
         * 侦听一个数据发生的变化
         * @param expOrFn 访问的数据路径，或数据值的计算函数，当路径中的变量或计算函数所访问的值发生变化时，将会被重新执行
         * @param cb 重新执行后，发生变化则会出发回调函数
         */
		public static Watcher Watch(this IStdHost self, string expOrFn, Action<object, object, object> cb, CombineType<object, string, number, bool> loseValue = null, bool sync = false)
		{
			return HostExt._Swatch((IHostAccessor)self, expOrFn, cb, loseValue, sync);
		}

		/**
         * 侦听一个数据发生的变化
         * @param expOrFn 访问的数据路径，或数据值的计算函数，当路径中的变量或计算函数所访问的值发生变化时，将会被重新执行
         * @param cb 重新执行后，发生变化则会出发回调函数
         */
		public static Watcher Watch(this IStdHost self, Func<object, object, object> expOrFn, Action<object, object, object> cb, CombineType<object, string, number, bool> loseValue = null, bool sync = false)
		{
			return HostExt._Swatch((IHostAccessor)self, expOrFn, cb, loseValue, sync);
		}

		/**
         * 释放host，包括所有watch
         */
		public static void Destroy(this IStdHost self)
		{
			HostExt._Sdestroy((IHostAccessor)self);
		}

		public static void RemoveWatcher(this IStdHost self, Watcher watcher)
		{
			HostExt._SremoveWatcher((IHostAccessor)self, watcher);
		}

		public static void AddWatcher(this IStdHost self, Watcher watcher)
		{
			HostExt._SaddWatcher((IHostAccessor)self, watcher);
		}

		public static void ClearWatchers(this IStdHost self)
		{
			HostExt._SclearWatchers((IHostAccessor)self);
		}

		public static bool IsHostAlive(this IStdHost self)
		{
			return !((IWithDestroyState)self)._SIsDestroyed;
		}
		public static bool IsHostDestroyed(this IStdHost self)
		{
			return ((IWithDestroyState)self)._SIsDestroyed;
		}

		public static void SetProto(this IStdHost self, object proto)
		{
			((DataBinding.CollectionExt.IWithPrototype)self).SetProto(proto);
		}

		public static object GetProto(this IStdHost self)
		{
			return ((DataBinding.CollectionExt.IWithPrototype)self).GetProto();
		}

		/// <summary>
		/// 通知设置值并强制通知值已更新
		/// </summary>
		/// <param name="self"></param>
		/// <param name="key"></param>
		/// <param name="newValue"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T NotifyChangeValue<T>(this IStdHost self, string key, T newValue)
		{
			var p = self.GetType().GetProperty(key, BindingFlags.Instance | BindingFlags.Public);
			if (p != null)
			{
				var oldValue=p.GetValue(self);
				p.SetValue(self, newValue);
				if (self is VM.IObservableEventDelegate observable)
				{
					observable.NotifyPropertyChanged(newValue, oldValue, key);
				}
				else
				{
					Debug.LogError($"Invalid type {self?.GetType()?.FullName} not implement interface {nameof(VM.IObservableEventDelegate)}");
				}
			}
			else
			{
				Debug.LogError($"Invalid type {self?.GetType()?.FullName} without property {key}");
			}
			return newValue;
		}
		
		/// <summary>
		/// 强制通知值已更新
		/// </summary>
		/// <param name="self"></param>
		/// <param name="key"></param>
		/// <param name="oldValue"></param>
		/// <typeparam name="T"></typeparam>
		public static void MarkDirty<T>(this IStdHost self, string key, T oldValue)
		{
			var p = self.GetType().GetProperty(key, BindingFlags.Instance | BindingFlags.Public);
			if (p != null)
			{
				var value=p.GetValue(self);
				if (self is VM.IObservableEventDelegate observable)
				{
					observable.NotifyPropertyChanged(value, oldValue, key);
				}
				else
				{
					Debug.LogError($"Invalid type {self?.GetType()?.FullName} not implement interface {nameof(VM.IObservableEventDelegate)}");
				}
			}
			else
			{
				Debug.LogError($"Invalid type {self?.GetType()?.FullName} without property {key}");
			}
		}
		/// <summary>
		/// 强制通知值已更新
		/// </summary>
		/// <param name="self"></param>
		/// <param name="key"></param>
		/// <typeparam name="T"></typeparam>
		public static void MarkDirty<T>(this IStdHost self, string key)
		{
			var p = self.GetType().GetProperty(key, BindingFlags.Instance | BindingFlags.Public);
			if (p != null)
			{
				var value=p.GetValue(self);
				if (self is VM.IObservableEventDelegate observable)
				{
					observable.NotifyPropertyChanged(value, value, key);
				}
				else
				{
					Debug.LogError($"Invalid type {self?.GetType()?.FullName} not implement interface {nameof(VM.IObservableEventDelegate)}");
				}
			}
			else
			{
				Debug.LogError($"Invalid type {self?.GetType()?.FullName} without property {key}");
			}
		}
	}

}
