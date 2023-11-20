using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTestUitls
{
	public class Expect<T>
	{
		public T value;
		public void toEqual(object v)
		{
			Assert.AreEqual(value, v);
		}

		public void toBe(object v)
		{
			Assert.AreEqual(value, v);
		}

		public void toBeInstanceOf(Type v)
        {
			Assert.IsInstanceOf(v,this.value);
		}
		public void toBeInstanceOf<F>()
		{
			Assert.IsInstanceOf<F>(this.value);
		}
	}

	public class TestEnv
	{
		public Expect<T> expect<T>(T value)
		{
			var e = new Expect<T>();
			e.value = value;
			return e;
		}

	}
}
