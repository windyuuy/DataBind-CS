using System;
using System.Collections;
using vm;

namespace System.ListExt
{
	public partial class List<T> : IWithPrototype
	{
		public object Proto;
		public virtual object _ { get; set; }

        public virtual object GetProto()
        {
            return Proto;
        }

        public virtual void SetProto(object dict)
		{
			this.Proto = dict;
			this._ = dict;
		}

	}
}
