using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBind
{
	/// <summary>
	/// 使field自动转为property
	/// </summary>
	[System.AttributeUsage(AttributeTargets.Class| AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
	public sealed class AutoFieldPropertyAttribute : System.Attribute
	{
		public AutoFieldPropertyAttribute()
		{
		}
	}

	/// <summary>
	/// 使field自动转为property
	/// </summary>
	[System.AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
	public sealed class KeepFieldPropertyAttribute : System.Attribute
	{
		public KeepFieldPropertyAttribute()
		{
		}
	}
}
