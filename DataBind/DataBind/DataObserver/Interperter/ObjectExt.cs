﻿using System;
using System.Text;

namespace System.ListExt
{
	public interface IWithPrototype
	{
		void SetProto(object proto);
		object GetProto();
		object _ { get; set; }
	}
}
