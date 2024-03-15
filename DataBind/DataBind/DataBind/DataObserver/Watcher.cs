
using System;
using DataBind.CollectionExt;
using DataBind;
using EngineAdapter.Diagnostics;
using Console = EngineAdapter.Diagnostics.Console;

namespace DataBind.VM
{
	using number = System.Double;
	using boolean = System.Boolean;

	public class WatchOptions
	{
		public boolean sync;
		public CombineType<object, string, number, boolean> loseValue;
	}

	public class Watcher
	{
		public static int uid = 0;

		/**
         * 宿主
         */
		public IHostAccessor host;


		public number id;

		/**
         * update的时候的回调函数
         */
		public Action<object, object, object> cb;

		/**
         * 立即执行
         */
		public boolean sync;

		/**
         * 控制watch的开关
         */
		public boolean active;

		/**
         * 当前收集的依赖，用于与新的依赖差异对比
         */
		public List<Dep> deps;

		public IIdMap depIds;

		/**
         * 本轮收集的依赖，在作为当前依赖前，需要用于差异对比
         */
		public List<Dep> newDeps;

		public IIdMap newDepIds;

		/**
         * 最终要执行的get函数
         */
		public Func<object, object, object> getter;

		/**
         * 执行后的结果值
         */
		public object value;

		/**
         * 当执行失败时所要表达值
         */
		public CombineType<object, string, number, boolean> loseValue;

		public Watcher(
			IHostAccessor host,
			CombineType<object, string, Func<object, object, object>> expOrFn,
			Action<object, object, object> cb,
			WatchOptions options
		)
		{
			this.host = host;
			// options
			if (options != null)
			{
				this.sync = !!options.sync;
				this.loseValue = options.loseValue;
			}
			else
			{
				this.sync = false;

				this.loseValue = null;
			}

			this.cb = cb;

			this.id = ++uid;

			this.active = true;

			this.deps = new List<Dep>();

			this.newDeps = new List<Dep>();

			this.depIds = new IdMap();
			this.newDepIds = new IdMap();

			if (expOrFn.Is<Func<object, object, object>>())
			{
				this.getter = expOrFn;
			}
			else
			{
				this.getter = Utils.parsePath(expOrFn.As<string>());

				if (this.getter == null)
				{
					this.getter = (a, b) => { return null; };
					Console.Warn(
						$"expOrFn 路径异常: \"{expOrFn}\" "
					);
				}
			}

			this.value = this.get();

		}

		/**
		 * 获取值，并重新收集依赖
		 */
		public object get()
		{
			/*开始收集依赖*/
			Dep.pushCollectTarget(this);

			object value;

			try
			{
				value = this.getter(this.host, this.host);
			}
			catch (Exception e)
			{
				Console.Error(e);
				value = null;
			}

			//当get失败，则使用loseValue的值
			if (this.loseValue != null && value == null)
			{
				value = this.loseValue?.RawObject;
			}

			/*结束收集*/
			Dep.popCollectTarget();
			this.cleanupDeps();
			return value;
		}

		/**
		 * 添加依赖
		 * 在收集依赖的时候，触发 Dependency.collectTarget.addDep
		 */
		public void addDep(Dep dep)
		{
			var id = dep.id;

			if (!this.newDepIds.Has(id))
			{
				this.newDepIds.Add(id);

				this.newDeps.push(dep);

				//向dep添加自己，实现双向访问，depIds用作重复添加的缓存
				if (!this.depIds.Has(id))
				{
					dep.add(this);

				}
			}
		}

		/**
		 * 清理依赖收集
		 */
		public void cleanupDeps()
		{
			//移除本次收集后，不需要的依赖（通过差异对比）
			var i = this.deps.length;


			while (i-- != 0)
			{
				var dep = this.deps[i];

				if (!this.newDepIds.Has(dep.id))
				{
					dep.remove(this);

				}
			}

			//让new作为当前记录的依赖，并清空旧的
			var tmp1 = this.depIds;
			this.depIds = this.newDepIds;
			this.newDepIds = tmp1;
			this.newDepIds.Clear();

			var tmp2 = this.deps;
			this.deps = this.newDeps;
			this.newDeps = tmp2;
			this.newDeps.Clear();
		}

		/**
		 * 当依赖发生变化就会被执行
		 */
		public void update()
		{
			if (this.sync)
			{
				//立即渲染
				this.run();
			}
			else
			{
				//下一帧渲染，可以降低重复渲染的概率
				Tick.Add(this);
			}
		}

		/**
		 * 执行watch
		 */
		public void run()
		{
			if (this.active)
			{
				var value = this.get();
				//如果数值不想等，或者是复杂对象就需要更新视图
				if (value != this.value || Utils.IsObservable(value))
				{
					var oldValue = this.value;

					this.value = value;
					/*触发回调渲染视图*/
					this.cb(this.host, value, oldValue);

				}
			}
		}

		/**
		 * 收集该watcher的所有deps依赖
		 */
		public void depend()
		{
			var i = this.deps.length;


			while (i-- != 0)
			{
				this.deps[i].asCurTargetDepend();

			}
		}

		/**
		 * 将自身从所有依赖收集订阅列表删除
		 */
		public void teardown()
		{
			if (this.active)
			{
				this.host._SremoveWatcher(this);
				var i = this.deps.length;


				while (i-- != 0)
				{
					this.deps[i].remove(this);
				}
				this.active = false;
			}
		}
	}

}