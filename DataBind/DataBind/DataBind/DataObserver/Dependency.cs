
using System;
using DataBinding.CollectionExt;

namespace vm
{
	using number = System.Double;

	public class Dep
	{

		public static int uid = 0;

		/**
		 * 递归遍历数组，进行ob对象的依赖记录。
		 */
		public static void dependCollection(System.Collections.IEnumerable value)
		{
			if (value is System.Collections.IDictionary)
			{
				var dict = value as System.Collections.IDictionary;
				foreach (var obj0 in dict.Values)
				{
					if (Utils.IsObservable(obj0))
					{
						IObservable obj = Utils.AsObservable(obj0);
						if (obj != null && obj._SgetOb() != null)
						{
							obj._SgetOb().dep.asCurTargetDepend();
						}
						if (Utils.IsCollection(obj))
						{
							dependCollection(Utils.AsCollection(obj));
						}
					}
				}
			}
			else
			{
				foreach (var obj0 in value)
				{
					if (Utils.IsObservable(obj0))
					{
						IObservable obj = Utils.AsObservable(obj0);
						if (obj != null && obj._SgetOb() != null)
						{
							obj._SgetOb().dep.asCurTargetDepend();
						}
						if (Utils.IsCollection(obj))
						{
							dependCollection(Utils.AsCollection(obj));
						}
					}
				}
			}
		}

		/**
         * 当前正在收集依赖的对象
         */
		public static Watcher target = null;

		/**
         * 当前正在收集以来的列队
         */
		static List<Watcher> collectTargetStack = new List<Watcher>();

		public static void pushCollectTarget(Watcher target)
		{
			collectTargetStack.push(target);
			Dep.target = target;
		}

		public static void popCollectTarget()
		{
			collectTargetStack.pop();
			Dep.target = collectTargetStack.TryGet(collectTargetStack.length - 1);
		}

		/**
         * 唯一id，方便hashmap判断是否存在
         */
		public number id = uid++;

		/**
         * 侦听者
         */
		List<Watcher> watchers = new List<Watcher>();

		public void add(Watcher sub)
		{
			this.watchers.Add(sub);

		}

		/*移除一个观察者对象*/
		public void remove(Watcher sub)
		{
			this.watchers.Remove(sub);
		}

		/**
         * 向当前 watcher 依赖项目中添加自身
         */
		public void asCurTargetDepend()
		{
			if (Dep.target != null)
			{
				// Dep.target指向的是一个watcher
				Dep.target.addDep(this);

			}
		}

		/**
         * 通知所有侦听者
         */
		public void notifyWatchers()
		{
			var ws = this.watchers.slice();

			for (int i = 0, l = ws.length; i < l; i++)
			{
				ws[i].update();
			}
		}
	}
}