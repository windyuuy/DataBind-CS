using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace TestDataBind
{
	internal class TestIdMap
	{
		[Test]
		public void test()
		{
			var map = new vm.IdMap();
			Assert.AreEqual(map.has(10),false);
			map.add(10);
			Assert.AreEqual(map.has(10),true);
			map.add(10);
			Assert.AreEqual(map.has(10),true);
			map.clear();
			Assert.AreEqual(map.has(10), false);

		}
	}
}
