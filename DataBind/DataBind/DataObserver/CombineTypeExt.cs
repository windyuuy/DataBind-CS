using System;
using System.Collections.Generic;
using System.Text;

namespace vm
{
	public static class CombineTypeExt
	{
		public static bool Is<T>(this CombineType v)
		{
			if (v == null)
			{
				return false;
			}
			else
			{
				return v.RawObject is T;
			}
		}

		public static bool IsList(this CombineType v)
		{
			if (v == null)
			{
				return false;
			}
			else
			{
				return v.RawObject is System.Collections.IList;
			}
		}

		public static bool IsDict(this object v)
		{
			return v is System.Collections.IDictionary;
		}
		public static bool IsProtoDict(this object v)
		{
			return (v is System.Collections.IDictionary) && (v is System.ListExt.IWithPrototype);
		}
		public static bool IsWithProto(this object v)
		{
			return (v is System.ListExt.IWithPrototype);
		}
		public static bool IsConvableDict(this object v)
		{
			return v is System.ListExt.Dictionary;
		}
	}
}
