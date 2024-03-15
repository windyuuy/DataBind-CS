using NUnit.Framework;
using System;
using DataBinding.CollectionExt;
using System.Text;
using VM;

namespace TestDataBind
{

	public class SampleOBD3<T> : IObservable, IWithPrototype
	{
		public VM.Observer ___Sob__;
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

		public VM.Observer _SgetOb()
		{
			return ___Sob__;
		}

		public void _SsetOb(VM.Observer value)
		{
			___Sob__ = value;
		}

		public object Proto;
		public virtual object _self { get; set; }

		public virtual object GetProto()
		{
			return Proto;
		}

		public virtual void SetProto(object dict)
		{
			this.Proto = dict;
			this._self = dict;
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
			var func = VM.Utils.parsePath("A.A.A");
			Assert.IsNotNull(func);
			Assert.AreEqual(func(null, a), 100);
		}

		[Test]
		public void TestParsePath2()
		{
			var a = new SampleOBD4<Dictionary<string, SampleOBD3<int>>>();
			a.A["a"] = new SampleOBD3<int>();
			a.A["a"].A = 100;
			var func = VM.Utils.parsePath("A.a.A");
			Assert.IsNotNull(func);
			Assert.AreEqual(func(null, a), 100);
		}
	}
}
