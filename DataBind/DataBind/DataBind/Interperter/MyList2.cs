using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DataBind.VM;

namespace DataBind.CollectionExt
{
	public partial class List<T> : IConvertible
	{
		public TypeCode GetTypeCode()
		{
			return TypeCode.Object;
		}

		public bool ToBoolean(IFormatProvider provider)
		{
			return Convert.ToBoolean(provider);
		}

		public byte ToByte(IFormatProvider provider)
		{
			return Convert.ToByte(provider);
		}

		public char ToChar(IFormatProvider provider)
		{
			return Convert.ToChar(provider);
		}

		public DateTime ToDateTime(IFormatProvider provider)
		{
			return Convert.ToDateTime(provider);
		}

		public decimal ToDecimal(IFormatProvider provider)
		{
			return Convert.ToDecimal(provider);
		}

		public double ToDouble(IFormatProvider provider)
		{
			return Convert.ToDouble(provider);
		}

		public short ToInt16(IFormatProvider provider)
		{
			return Convert.ToInt16(provider);
		}

		public int ToInt32(IFormatProvider provider)
		{
			return Convert.ToInt32(provider);
		}

		public long ToInt64(IFormatProvider provider)
		{
			return Convert.ToInt64(provider);
		}

		public sbyte ToSByte(IFormatProvider provider)
		{
			return Convert.ToSByte(provider);
		}

		public float ToSingle(IFormatProvider provider)
		{
			return Convert.ToSingle(provider);
		}

		public string ToString(IFormatProvider provider)
		{
			return Convert.ToString(provider);
		}

		public object ToType(Type conversionType, IFormatProvider provider)
		{
			if (conversionType.IsAssignableFrom(this.GetType()))
			{
				return this;
			}
			else if (typeof(List).IsAssignableFrom(conversionType))
			{
				var con = conversionType.GetConstructor(new Type[] { typeof(IConvableList) });
				var obj = con.Invoke(new object[] { this });//.CreateInstance(conversionType.Name, false, Reflection.BindingFlags.CreateInstance, null, new object[] { this }, null, null);
				return obj;
			}
			else
			{
				return Convert.ChangeType(this, conversionType);
			}
		}

		public ushort ToUInt16(IFormatProvider provider)
		{
			return Convert.ToUInt16(provider);
		}

		public uint ToUInt32(IFormatProvider provider)
		{
			return Convert.ToUInt32(provider);
		}

		public ulong ToUInt64(IFormatProvider provider)
		{
			return Convert.ToUInt64(provider);
		}
	}
}
