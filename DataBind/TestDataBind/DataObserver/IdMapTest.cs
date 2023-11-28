﻿using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace TestDataBind
{
	internal class TestIdMap
	{
		[Test]
		public void Test()
		{
			var map = new VM.IdMap();
			Assert.AreEqual(map.Has(10),false);
			map.Add(10);
			Assert.AreEqual(map.Has(10),true);
			map.Add(10);
			Assert.AreEqual(map.Has(10),true);
			map.Clear();
			Assert.AreEqual(map.Has(10), false);

		}
	}
}
