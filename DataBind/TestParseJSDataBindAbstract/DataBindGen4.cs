﻿using System.Text.RegularExpressions;

using number = System.Double;

namespace TestingCode
{
	public class TestWriteCodeCase4: vm.Host
	{
		/// <note>
		/// env::a
		/// </note>
		public TA a {get;set;} = new TA();
		public class TA
		{
			public TB b {get;set;} = new TB();
			public class TB
			{
				/// <usecase>
				/// a.b.c<br/>
				/// </usecase>
				public void c(Regex btn)
				{
				}
			}
		}
	}
}