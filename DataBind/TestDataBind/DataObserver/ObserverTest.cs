using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using vm;

namespace TestDataBind
{
	public class SampleOBD : IObservable
	{
		public vm.Observer ___Sob__;

		public event PropertyChangedEventHandler PropertyChanged;
		public event PropertyGetEventHandler PropertyGot;

		public vm.Observer _SgetOb()
		{
			return ___Sob__;
		}

		public void _SsetOb(vm.Observer value)
		{
			___Sob__ = value;
		}
	}


	public class SampleOBD2 : IObservable
	{
		public vm.Observer ___Sob__;
		private double a1;
		private double b1;
		private double c1;

		public void Set(double a, double b, double c)
		{
			a1 = a;
			b1 = b;
			c1 = c;
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public event PropertyGetEventHandler PropertyGot;

		public double a
		{
			get
			{
				PropertyGot?.Invoke(this, new PropertyGetEventArgs("a", a1));
				return a1;
			}
			set
			{
				var oldValue = a1;
				a1 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("a", value, oldValue));
			}
		}
		public double b
		{
			get
			{
				PropertyGot?.Invoke(this, new PropertyGetEventArgs("a", a1));
				return b1;
			}
			set
			{
				var oldValue = b1;
				b1 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("a", value, oldValue));
			}
		}
		public double c
		{
			get
			{
				PropertyGot?.Invoke(this, new PropertyGetEventArgs("a", a1));
				return c1;
			}
			set
			{
				var oldValue = b1;
				c1 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("a", value, oldValue));
			}
		}

		public vm.Observer _SgetOb()
		{
			return ___Sob__;
		}

		public void _SsetOb(vm.Observer value)
		{
			___Sob__ = value;
		}
	}

	internal class Observer
	{
		[Test]
		public void testObserve()
		{
			var obj = new SampleOBD();
			var obj2 = Utils.observe(obj);

			Assert.IsTrue(obj2 is vm.Observer);
			Assert.AreSame(obj._SgetOb(), obj2);

			var obj3 = vm.Utils.observe(obj);
			Assert.IsNull(vm.Utils.observe(123));
		}

		[Test]
		public void testDefineReactive()
		{
			var o = new SampleOBD2() { a = 1, b = 2, c = 3, };
			vm.Utils.defineReactive(o, "a", 1);
			Assert.AreEqual(1, o.a);
			o.a = 123;
			Assert.AreEqual(o.a, 123);
		}

		[Test]
		public void TestObserver()
		{
			var obj = new SampleOBD();
			var ob = vm.Utils.observe(obj);

			Assert.AreEqual(ob?.value, obj);
		}

		[Test]
		public void TestDefineCompute()
		{
			var obj = new SampleOBD2()
			{
				a = 1,
				b = 2,
			};
			var ob = vm.Utils.observe(obj);


			//vm.defineCompute(obj, 'a', () => {
			//    return 10;
			//})
			//expect(obj.a).toBe(10);

		}

	}
}
