using System;
using System.Linq;
using vm;

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
		public static Watcher Watch(this IStdHost self, CombineType<object, string, Func<object, object, object>> expOrFn, Action<object, object, object> cb, CombineType<object, string, number, bool> loseValue = null, bool sync = false)
		{
			return HostExt._Swatch((IHostAccessor)self, expOrFn, cb, loseValue, sync);
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
	}

}
