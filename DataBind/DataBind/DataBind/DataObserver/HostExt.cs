using System;
using System.Linq;
using vm;
using Game.Diagnostics.IO;

namespace vm
{

	using boolean = System.Boolean;
	using number = System.Double;

	public static class HostExt
	{
		/**
         * 侦听一个数据发生的变化
         * @param expOrFn 访问的数据路径，或数据值的计算函数，当路径中的变量或计算函数所访问的值发生变化时，将会被重新执行
         * @param cb 重新执行后，发生变化则会出发回调函数
         */
		public static Watcher _Swatch(this IHostAccessor self, CombineType<object, string, Func<object, object, object>> expOrFn, Action<object, object, object> cb, CombineType<object, string, number, boolean> loseValue = null, boolean sync = false)
		{
			if (self is IWithDestroyState self1)
			{
				if (self1._SIsDestroyed)
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

			if (self is IWithDestroyState self1)
			{
				self1._SIsDestroyed = true;
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

		public static void _SclearWatchers(this IHostAccessor self)
		{
			self.GetWatchers().Clear();
		}
	}

}
