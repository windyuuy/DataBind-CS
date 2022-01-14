using NUnit.Framework;
using System;
using System.ListExt;
using System.Text;
using vm;

namespace TestDataBind
{

	public class SampleOBD3<T> : IObservable, IWithPrototype
	{
		public vm.Observer ___Sob__;
		protected T a1;

		public void Set(T a)
		{
			this.a1 = a;
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public event PropertyGetEventHandler PropertyGot;

		public T A
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

		public vm.Observer _SgetOb()
		{
			return ___Sob__;
		}

		public void _SsetOb(vm.Observer value)
		{
			___Sob__ = value;
		}

		public object Proto;
		public virtual object _ { get; set; }

		public object GetProto()
		{
			return Proto;
		}

		public void SetProto(object dict)
		{
			this.Proto = dict;
			this._ = dict;
		}
	}
	public class SampleOBD4<T> : SampleOBD3<T> where T : class, new()
	{
		public SampleOBD4()
		{
			this.a1 = new T();
		}
		public SampleOBD4(bool fill)
		{
			if (fill)
			{
				this.a1 = new T();
			}
		}

	}

	internal class TestUtils
	{
		[Test]
		public void TestParsePath1()
		{
			var a = new SampleOBD4<SampleOBD4<SampleOBD3<int>>>();
			a.A.A.A = 100;
			var func = vm.Utils.parsePath("A.A.A");
			Assert.IsNotNull(func);
			Assert.AreEqual(func(null, a), 100);
		}

		[Test]
		public void TestParsePath2()
		{
			var a = new SampleOBD4<Dictionary<string, SampleOBD3<int>>>();
			a.A["a"] = new SampleOBD3<int>();
			a.A["a"].A = 100;
			var func = vm.Utils.parsePath("A.a.A");
			Assert.IsNotNull(func);
			Assert.AreEqual(func(null, a), 100);
		}
	}
}
