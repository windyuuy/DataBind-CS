using System;
using System.Collections.Generic;
using System.Linq;
using System.ListExt;
using System.Runtime.CompilerServices;

namespace vm
{
	using boolean = System.Boolean;
	using number = System.Double;

	public interface IWithDestroyState
	{
		bool _SIsDestroyed { get; set; }
	}

	public interface IHostAccessor
	{
		// protected System.Collections.Generic.ICollection<Watcher> _Swatchers;
		System.Collections.Generic.ICollection<Watcher> GetWatchers();
	}

	/// <summary>
	/// 实现观察者
	/// </summary>
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

	public interface IFullHost:IHostAccessor,IWithDestroyState
    {

    }

	/// <summary>
	/// 用于标记需要作为观察者
	/// </summary>
	public interface IHostStand
    {

    }

}

namespace DataBinding
{

	/// <summary>
	/// 手动添加，标记这个类需要作为观察者
	/// </summary>
	[System.AttributeUsage(System.AttributeTargets.All, Inherited = false, AllowMultiple = true)]
	sealed class HostAttribute : System.Attribute
	{
		public HostAttribute()
		{
		}
	}

}