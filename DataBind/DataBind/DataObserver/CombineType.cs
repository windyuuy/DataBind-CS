
using System.ListExt;

namespace vm
{
	public class CombineType
	{
		public CombineType()
		{
		}
		public CombineType(object value)
		{
			throw new System.Exception("not implemented.");
		}

		public virtual object RawObject
		{
			get => throw new System.Exception("invalid value to conv");
		}
		public virtual CombineType Set(object value)
		{
			throw new System.Exception($"invalid value type to conv {value.GetType().Name}");
		}
		public virtual T As<T>()
		{
			return (T)this.RawObject;
		}
		//public virtual bool Is<T>()
		//{
		//	return this.RawObject is T;
		//}
		//public virtual bool islist()
		//{
		//	return this.rawobject is system.collections.ilist;
		//}
		public virtual List<T> AsList<T>()
        {
			var v = this.RawObject;
			if(v is List<T>)
            {
				return (List<T>)v;
            }
            else
            {
				return new List<T>(((List)v).RawList);
            }
		}
	}
	public class CombineType<C0> : CombineType
	{
		public CombineType()
		{
		}
		public CombineType(object value)
		{
			this.Set(value);
		}
		public C0 Common
		{
			get
			{
				return (C0)this.RawObject;
			}
		}
		public static implicit operator CombineType<C0>(C0 self)
		{
			return new CombineType<C0>(self);
		}

	}

	// public class CombineCommonType<Base, T> where T : CombineType, new()
	// {
	// 	public CombineCommonType()
	// 	{

	// 	}
	// 	public CombineCommonType(T value)
	// 	{
	// 		this.value = value;
	// 	}
	// 	public T value;
	// 	public Base Common => value.As<Base>();
	// 	public static implicit operator T(CombineCommonType<Base, T> self)
	// 	{
	// 		return self.value;
	// 	}
	// 	public static implicit operator CombineCommonType<Base, T>(T self)
	// 	{
	// 		return new CombineCommonType<Base, T>(self);
	// 	}
	// 	public static implicit operator CombineCommonType<Base, T>(Base self)
	// 	{
	// 		return new CombineCommonType<Base, T>(new T().Set(self) as T);
	// 	}
	// }

	public class CombineType<C0, T1> : CombineType<C0>
	{
		public CombineType()
		{
		}
		public override CombineType Set(object value)
		{
			if (value is T1)
			{
				this.value1 = (T1)value;
				return this;
			}
			else
			{
				return base.Set(value);
			}
		}
		public CombineType(T1 value) : base(value)
		{
			this.value1 = value;
		}
		public CombineType(object value) : base(value)
		{
		}

		public T1 value1;

		public override object RawObject
		{
			get
			{
				if (this.value1 != null)
				{
					return this.value1;
				}
				return null;
			}
		}

		public static implicit operator CombineType<C0, T1>(C0 self)
		{
			return new CombineType<C0, T1>(self);
		}
		public static implicit operator T1(CombineType<C0, T1> self)
		{
			return self.value1;
		}
		public static implicit operator CombineType<C0, T1>(T1 self)
		{
			return new CombineType<C0, T1>(self);
		}
	}

	public class CombineType<C0, T1, T2> : CombineType<C0, T1>
	{
		public CombineType()
		{
		}
		public override CombineType Set(object value)
		{
			if (value is T2)
			{
				this.value2 = (T2)value;
				return this;
			}
			else
			{
				return base.Set(value);
			}
		}
		public CombineType(T2 value) : base((T1)(object)value)
		{
			this.value2 = value;
		}
		public CombineType(object value) : base(value)
		{
		}

		public T2 value2;

		public override object RawObject
		{
			get
			{
				if (base.RawObject != null)
				{
					return base.RawObject;
				}
				if (this.value2 != null)
				{
					return this.value2;
				}
				return null;
			}
		}

		public static implicit operator CombineType<C0, T1, T2>(C0 self)
		{
			return new CombineType<C0, T1, T2>(self);
		}
		public static implicit operator T2(CombineType<C0, T1, T2> self)
		{
			return self.value2;
		}
	}

	public class CombineType<C0, T1, T2, T3> : CombineType<C0, T1, T2>
	{
		public CombineType()
		{
		}
		public override CombineType Set(object value)
		{
			if (value is T3)
			{
				this.value3 = (T3)value;
				return this;
			}
			else
			{
				return base.Set(value);
			}
		}
		public CombineType(T3 value) :base()
		{
			this.value3 = value;
		}
		public CombineType(object value) : base(value)
		{
		}
		public T3 value3;

		public override object RawObject
		{
			get
			{
				if (base.RawObject != null)
				{
					return base.RawObject;
				}
				if (this.value3 != null)
				{
					return this.value3;
				}
				return null;
			}
		}

        public static implicit operator CombineType<C0, T1, T2, T3>(T1 self)
        {
            return new CombineType<C0, T1, T2, T3>(self);
        }
        public static implicit operator CombineType<C0, T1, T2, T3>(T2 self)
        {
            return new CombineType<C0, T1, T2, T3>(self);
        }
        public static implicit operator CombineType<C0, T1, T2, T3>(T3 self)
        {
            return new CombineType<C0, T1, T2, T3>(self);
        }
        public static implicit operator CombineType<C0, T1, T2, T3>(C0 self)
		{
			return new CombineType<C0, T1, T2, T3>(self);
		}
		public static implicit operator T3(CombineType<C0, T1, T2, T3> self)
		{
			return self.value3;
		}
	}

	public class CombineType<C0, T1, T2, T3, T4> : CombineType<C0, T1, T2, T3>
	{
		public CombineType()
		{
		}
		public override CombineType Set(object value)
		{
			if (value is T4)
			{
				this.value4 = (T4)value;
				return this;
			}
			else
			{
				return base.Set(value);
			}
		}
		public CombineType(T4 value) : base((T3)(object)value)
		{
			this.value4 = value;
		}
		public CombineType(object value) : base(value)
		{
		}
		public T4 value4;

		public override object RawObject
		{
			get
			{
				if (base.RawObject != null)
				{
					return base.RawObject;
				}
				if (this.value4 != null)
				{
					return this.value4;
				}
				return null;
			}
		}

		public static explicit operator CombineType<C0, T1, T2, T3,T4>(C0 self)
		{
			return new CombineType<C0, T1, T2, T3, T4>(self);
		}
		public static implicit operator T4(CombineType<C0, T1, T2, T3, T4> self)
		{
			return self.value4;
		}
	}

	public class CombineType<C0, T1, T2, T3, T4, T5> : CombineType<C0, T1, T2, T3, T4>
	{
		public CombineType()
		{
		}
		public override CombineType Set(object value)
		{
			if (value is T5)
			{
				this.value5 = (T5)value;
				return this;
			}
			else
			{
				return base.Set(value);
			}
		}
		public CombineType(T5 value) : base((T4)(object)value)
		{
			this.value5 = value;
		}
		public CombineType(object value) : base(value)
		{
		}
		public T5 value5;

		public override object RawObject
		{
			get
			{
				if (base.RawObject != null)
				{
					return base.RawObject;
				}
				if (this.value5 != null)
				{
					return this.value5;
				}
				return null;
			}
		}

		public static implicit operator CombineType<C0, T1, T2, T3, T4, T5>(C0 self)
		{
			return new CombineType<C0, T1, T2, T3, T4, T5>(self);
		}
		public static implicit operator T5(CombineType<C0, T1, T2, T3, T4, T5> self)
		{
			return self.value5;
		}
	}

	public class CombineType<C0, T1, T2, T3, T4, T5, T6> : CombineType<C0, T1, T2, T3, T4, T5>
	{
		public CombineType()
		{
		}
		public override CombineType Set(object value)
		{
			if (value is T6)
			{
				this.value6 = (T6)value;
				return this;
			}
			else
			{
				return base.Set(value);
			}
		}
		public CombineType(T6 value) : base((T5)(object)value)
		{
			this.value6 = value;
		}
		public CombineType(object value) : base(value)
		{
		}
		public T6 value6;

		public override object RawObject
		{
			get
			{
				if (base.RawObject != null)
				{
					return base.RawObject;
				}
				if (this.value6 != null)
				{
					return this.value6;
				}
				return null;
			}
		}

		public static implicit operator CombineType<C0, T1, T2, T3, T4, T5, T6>(C0 self)
		{
			return new CombineType<C0, T1, T2, T3, T4, T5, T6>(self);
		}
		public static implicit operator T6(CombineType<C0, T1, T2, T3, T4, T5, T6> self)
		{
			return self.value6;
		}
	}

	public class CombineType<C0, T1, T2, T3, T4, T5, T6, T7> : CombineType<C0, T1, T2, T3, T4, T5, T6>
	{
		public CombineType()
		{
		}
		public override CombineType Set(object value)
		{
			if (value is T7)
			{
				this.value7 = (T7)value;
				return this;
			}
			else
			{
				return base.Set(value);
			}
		}
		public CombineType(T7 value) : base((T6)(object)value)
		{
			this.value7 = value;
		}
		public CombineType(object value) : base(value)
		{
		}

		public T7 value7;

		public override object RawObject
		{
			get
			{
				if (base.RawObject != null)
				{
					return base.RawObject;
				}
				if (this.value7 != null)
				{
					return this.value7;
				}
				return null;
			}
		}

		public static implicit operator CombineType<C0, T1, T2, T3, T4, T5, T6, T7>(C0 self)
		{
			return new CombineType<C0, T1, T2, T3, T4, T5, T6, T7>(self);
		}
		public static implicit operator T7(CombineType<C0, T1, T2, T3, T4, T5, T6, T7> self)
		{
			return self.value7;
		}
		public static implicit operator CombineType<C0, T1, T2, T3, T4, T5, T6, T7>(T7 self)
		{
			return new CombineType<C0, T1, T2, T3, T4, T5, T6, T7>(self);
		}
	}

	public class MixType<T1, T2>
	{
		public T1 value1;
		public T2 value2;

		public MixType(T1 raw)
		{

		}
	}

}
