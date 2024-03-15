using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using DataBind.VM;

namespace TestDataBind
{
	public class SampleOBD : IObservable
	{
		public DataBind.VM.Observer ___Sob__;

		public event PropertyChangedEventHandler PropertyChanged;
		public event PropertyGetEventHandler PropertyGot;

		public DataBind.VM.Observer _SgetOb()
		{
			return ___Sob__;
		}

		public void _SsetOb(DataBind.VM.Observer value)
		{
			___Sob__ = value;
		}
	}


	public class SampleOBD2 : IObservable
	{
		public DataBind.VM.Observer ___Sob__;
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

		public DataBind.VM.Observer _SgetOb()
		{
			return ___Sob__;
		}

		public void _SsetOb(DataBind.VM.Observer value)
		{
			___Sob__ = value;
		}
	}

	internal class Observer
	{
		[Test]
		public void TestObserve()
		{
			var obj = new SampleOBD();
			var obj2 = Utils.Observe(obj);

			Assert.IsTrue(obj2 is DataBind.VM.Observer);
			Assert.AreSame(obj._SgetOb(), obj2);

			var obj3 = DataBind.VM.Utils.Observe(obj);
			Assert.IsNull(DataBind.VM.Utils.Observe(123));
		}

		[Test]
		public void TestDefineReactive()
		{
			var o = new SampleOBD2() { a = 1, b = 2, c = 3, };
			DataBind.VM.Utils.Observe(o);
			DataBind.VM.Utils.DefineReactive(o, "a", 1);
			Assert.AreEqual(1, o.a);
			o.a = 123;
			Assert.AreEqual(o.a, 123);
		}

		[Test]
		public void TestObserver()
		{
			var obj = new SampleOBD();
			var ob = DataBind.VM.Utils.Observe(obj);

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
			var ob = DataBind.VM.Utils.Observe(obj);


			//vm.defineCompute(obj, 'a', () => {
			//    return 10;
			//})
			//expect(obj.a).toBe(10);

		}

	}
}
