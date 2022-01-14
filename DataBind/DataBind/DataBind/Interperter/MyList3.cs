using System;
using System.Collections;
using vm;

namespace System.ListExt
{
	public partial class List<T> : IWithPrototype
	{
		public object Proto;
		public virtual object _ { get; set; }

        public object GetProto()
        {
            return Proto;
        }

        public void SetProto(object dict)
		{
			this.Proto = dict;
			this._ = dict;
		}

	}
}
