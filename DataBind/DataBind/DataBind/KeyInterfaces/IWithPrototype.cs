using System;
using System.Text;

namespace DataBinding.CollectionExt
{
	public interface IWithPrototype
	{
		// protected object ___Sproto__;
		void SetProto(object proto);
		object GetProto();
		object _ { get; set; }
	}
}
