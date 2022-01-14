using System;
using System.Text;

namespace System.ListExt
{
	public interface IWithPrototype
	{
		// protected object ___Sproto__;
        void SetProto(object proto);
		object GetProto();
		object _ { get; set; }
	}
}
