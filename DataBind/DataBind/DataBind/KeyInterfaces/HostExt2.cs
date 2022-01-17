using System;
using System.Linq;
using vm;

namespace DataBinding
{
	using number = System.Double;

	public static class HostExt2
	{
		/**
         * 侦听一个数据发生的变化
         * @param expOrFn 访问的数据路径，或数据值的计算函数，当路径中的变量或计算函数所访问的值发生变化时，将会被重新执行
         * @param cb 重新执行后，发生变化则会出发回调函数
         */
		public static Watcher Watch(this IHostStand self, CombineType<object, string, Func<object, object, object>> expOrFn, Action<object, object, object> cb, CombineType<object, string, number, bool> loseValue = null, bool sync = false)
		{
			return HostExt._Swatch((IHostAccessor)self, expOrFn, cb, loseValue, sync);
		}

		/**
         * 释放host，包括所有watch
         */
		public static void Destroy(this IHostStand self)
		{
			HostExt._Sdestroy((IHostAccessor)self);
		}

		public static void RemoveWatcher(this IHostStand self, Watcher watcher)
		{
			HostExt._SremoveWatcher((IHostAccessor)self, watcher);
		}

		public static void AddWatcher(this IHostStand self, Watcher watcher)
		{
			HostExt._SaddWatcher((IHostAccessor)self, watcher);
		}

		public static bool IsHostAlive(this IHostStand self)
        {
			return !((IWithDestroyState)self)._SIsDestroyed;
		}
		public static bool IsHostDestroyed(this IHostStand self)
		{
			return ((IWithDestroyState)self)._SIsDestroyed;
		}
	}

}
