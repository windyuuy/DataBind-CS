using System;

namespace UIBind.Meta
{
	[AttributeUsage(AttributeTargets.Field, Inherited = false)]
	public class UIBindKeyAttribute: Attribute
	{
		public Type DataType = typeof(object);

		public UIBindKeyAttribute()
		{
			
		}
		public UIBindKeyAttribute(Type dataType)
		{
			DataType = dataType;
		}
	}
}