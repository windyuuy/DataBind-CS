using System;
using System.Text;

namespace DataBind.CollectionExt
{
	public interface IWithPrototype
	{
		// protected object ___Sproto__;
		void SetProto(object proto);
		object GetProto();
		object _self { get; set; }
	}
}
