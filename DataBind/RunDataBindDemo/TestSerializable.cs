using System;
using DataBinding;

namespace RunDataBindDemo
{
	[StdHost]
	[Serializable]
	public class TestSerializable
	{
		public int Aswjf { get; set; }
	}

	[StdHost]
	[Serializable]
	public class TestSerializable2222 : TestSerializable
	{
		public int Aswjf2 { get; set; }
	}
	
	[StdHost]
	[AutoFieldProperty]
	[Serializable]
	public class TestSerializable3333 : TestSerializable
	{
		public int Cdw3 { get; set; } = 3233;
		public int cdw5 { get; set; } = 3233;
		public int Ddsew1 = 23;
		public int ddsex2 = 23;
	}
}