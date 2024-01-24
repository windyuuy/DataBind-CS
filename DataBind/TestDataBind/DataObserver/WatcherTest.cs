using NUnit.Framework;
using System;
using DataBinding.CollectionExt;
using System.Text;
using VM;
using DataBinding;
using UnitTestUitls;

namespace TestDataBind.DataObserver
{
	using number = System.Double;

	class TestHost1 : VM.Host
	{
		public string testString = "a";
		public string TestString
		{
			get
			{
				NotifyPropertyGot(testString);
				return testString;
			}
			set
			{
				var v0 = testString;
				testString = value;
				NotifyPropertyChanged(value, v0);
			}
		}
		public number tstNumber = 1;
		public number TstNumber
		{
			get
			{
				NotifyPropertyGot(testString);
				return tstNumber;
			}
			set
			{
				var v0 = tstNumber;
				tstNumber = value;
				NotifyPropertyChanged(value, v0);
			}
		}
	}

	class TestHost2_1 : TestHost1
	{
		public List<number> testArr = new List<number>();

		public List<double> TestArr
		{
			get
			{
				NotifyPropertyGot(testArr);
				return testArr;
			}
			set
			{
				var v0 = testArr;
				testArr = value;
				NotifyPropertyChanged(value, v0);
			}
		}

		public TestHost1 subObj = new TestHost1();
		public TestHost1 SubObj
		{
			get
			{
				NotifyPropertyGot(subObj);
				return subObj;
			}
			set
			{
				var v0 = subObj;
				subObj = value;
				NotifyPropertyChanged(value, v0);
			}
		}

		public SampleOBD3<int> Sub2
		{
			get
			{
				NotifyPropertyGot(sub2);
				return sub2;
			}
			set
			{
				var v0 = sub2;
				sub2 = value;
				NotifyPropertyChanged(value, v0);
			}
		}


		public SampleOBD3<int> sub2 = new SampleOBD3<int>()
		{
			A = 1,
		};

		public List<Dictionary<string, int>> Sub3
		{
			get
			{
				NotifyPropertyGot(sub3);
				return sub3;
			}
			set
			{
				var v0 = sub3;
				sub3 = value;
				NotifyPropertyChanged(value, v0);
			}
		}

		public List<Dictionary<string, int>> sub3 = new List<Dictionary<string, int>>()
		{
			new Dictionary<string, int>(){
				{"e",3},
			},
		};

	}

	class TestHost2<T> : Host
	{
		public T a = default(T);
		public T A
		{
			get
			{
				NotifyPropertyGot(a);
				return a;
			}
			set
			{
				var v0 = a;
				a = value;
				NotifyPropertyChanged(value, v0);
			}
		}
	}

	class View2
	{
		public string testString = "";
		public number tstNumber = 0;
		public string subTestString = "";
		public number subTstNumber = 0;
		public number computValue = 0;
		public SampleOBD3<int> testSub2 = new SampleOBD3<int>()
		{
			A = 1,
		};
		public List<Dictionary<string, int>> testSub3 = new List<Dictionary<string, int>>()
		{
			new Dictionary<string, int>(){
				{"e",3},
			},
		};
	}

	class TBoss : Host
	{
		public bool active = false;
		public bool Active
		{
			get
			{
				NotifyPropertyGot(active);
				return active;
			}
			set
			{
				var v0 = active;
				active = value;
				NotifyPropertyChanged(value, v0);
			}
		}
		public number hpMax = 10;
		public number HpMax
		{
			get
			{
				NotifyPropertyGot(hpMax);
				return hpMax;
			}
			set
			{
				var v0 = hpMax;
				hpMax = value;
				NotifyPropertyChanged(value, v0);
			}
		}
		public number hp = 0;
		public number Hp
		{
			get
			{
				NotifyPropertyGot(hp);
				return hp;
			}
			set
			{
				var v0 = hp;
				hp = value;
				NotifyPropertyChanged(value, v0);
			}
		}
	}

	public class View3
	{
		public number length;
		public string list0 = null;
		public string list1 = null;
		public string list2 = null;
	}

	public class View4
	{
		public string testString = "";
		public number tstNumber = 0;
		public number testObj_num1 = 0;
		public number testArr_1 = 0;
	}

	public class WatcherTest : TestEnv
	{
		[Test]
		public void Test简单数值绑定()
		{
			var view = new TestHost1()
			{
				testString = "",
				tstNumber = 0,
			};

			var host = new TestHost1();
			host._Swatch("TestString", (host, newVal, oldVal) =>
			{
				view.testString = (string)newVal;
			});
			host._Swatch("TstNumber", (host, newVal, oldVal) =>
			{
				view.tstNumber = (number)newVal;
			});

			host.TestString = "哈哈哈";
			VM.Tick.Next();
			expect(view.testString).toEqual("哈哈哈");

			host.TstNumber = 22;
			VM.Tick.Next();
			expect(view.tstNumber).toEqual(22);

		}

		[Test]
		public void Test深层数据绑定()
		{
			var view = new View2();

			view.testSub3.Clear();

			var host = new TestHost2<TestHost2_1>()
			{
				a = new TestHost2_1(),
			};
			host._Swatch("A.TestString", (host, newVal, oldVal) =>
			{
				view.testString = (string)newVal;
			});
			host._Swatch("A.TstNumber", (host, newVal, oldVal) =>
			{
				view.tstNumber = (number)newVal;
			});
			host._Swatch("A.SubObj.TestString", (host, newVal, oldVal) =>
			{
				view.subTestString = (string)newVal;
			});
			host._Swatch("A.SubObj.TstNumber", (host, newVal, oldVal) =>
			{
				view.subTstNumber = (number)newVal;
			});
			host._Swatch("A.Sub2", (host, newVal, oldVal) =>
			{
				view.testSub2 = (SampleOBD3<int>)newVal;
			});
			host._Swatch("A.Sub3", (host, newVal, oldVal) =>
			{
				view.testSub3 = (List<Dictionary<string, int>>)newVal;
			});
			{
				Func<object, object, object> call = (h, h2) =>
				{
					return host.A.TstNumber + host.A.subObj.TstNumber;
				};
				var w = host._Swatch(call, (host, newVal, oldVal) =>
				{
					view.computValue = (number)newVal;
				});
				view.computValue = (double)w?.value;
				expect(view.computValue).toEqual(2);
			}

			host.A.TestString = "哈哈哈";
			VM.Tick.Next();
			expect(view.testString).toEqual("哈哈哈");

			host.A.TstNumber = 13;
			VM.Tick.Next();
			expect(view.tstNumber).toEqual(13);
			expect(view.computValue).toEqual(14);

			host.A.SubObj.TestString = "哈哈哈2";
			VM.Tick.Next();
			expect(view.subTestString).toEqual("哈哈哈2");

			host.A.SubObj.TstNumber = 333;
			VM.Tick.Next();
			expect(view.subTstNumber).toEqual(333);

			host.A.SubObj = new TestHost1()
			{
				testString = "测试对象",
				tstNumber = 666,
			};
			VM.Tick.Next();
			expect(view.subTestString).toEqual("测试对象");
			expect(view.subTstNumber).toEqual(666);


			host.A.Sub2.A = 32;
			VM.Tick.Next();
			// expect(view.testSub2.a).toEqual(32);

			host.A.Sub3.Add(new Dictionary<string, int> { { "e", 5 } });
			VM.Tick.Next();
			expect(view.testSub3?[0]?["e"]).toEqual(3);


			host.A = new TestHost2_1()
			{
				testString = "测试1",
				tstNumber = 3,
				subObj =
				{
					testString= "测试2",
					tstNumber= 4,
				},
				sub2 =
				{
					A= 1,
				},
				sub3 = new List<Dictionary<string, int>>()
				{
					new Dictionary<string, int>(){
						{"e",3},
					},
				},
			};
			VM.Tick.Next();
			expect(view.testString).toEqual("测试1");
			expect(view.tstNumber).toEqual(3);
			expect(view.subTestString).toEqual("测试2");
			expect(view.subTstNumber).toEqual(4);

			{
				var Boos = new Dictionary<string, object>()
				{
					{"boss", new TBoss(){
						active = false,
						hpMax = 10,
						hp = 0,
					}}
				};
				var newHost = Boos;

				number progress = 0;
				{
					var w = newHost._Swatch("boss.Hp/boss.HpMax", (host, newVal, oldVal) =>
					{
						progress = (number)newVal;
					});
					progress = (number)w?.value;
					VM.Tick.Next();
					Boos["boss"] = new TBoss()
					{
						active = false,
						hpMax = 10,
						hp = 1,
					};
					VM.Tick.Next();
					expect(progress).toBe(0.1);
				}

				(Boos["boss"] as TBoss).HpMax = 10;
				(Boos["boss"] as TBoss).Hp = 5;
				VM.Tick.Next();
				expect(progress).toBe(0.5);
			}


			{
				var Boos = new Dictionary<string, object>()
				{
					{"boss", new Dictionary<string, object>(){
						{"active" , false},
						{"HpMax" , (double)10},
						{"Hp" , (double)0},
					}}
				};
				var newHost = Boos;

				number progress = 0;
				{
					var w = newHost._Swatch("boss.Hp/boss.HpMax", (host, newVal, oldVal) =>
					{
						progress = (number)newVal;
					});
					progress = (number)w?.value;
					VM.Tick.Next();
					Boos["boss"] = new Dictionary<string, object>(){
						{"active" , false},
						{"HpMax" , (double)10},
						{"Hp" , (double)1},
					};
					VM.Tick.Next();
					expect(progress).toBe(0.1);
				}

				(Boos["boss"] as Dictionary<string, object>)["HpMax"] = (double)10;
				(Boos["boss"] as Dictionary<string, object>)["Hp"] = (double)5;
				VM.Tick.Next();
				expect(progress).toBe(0.5);
			}

		}

		[Test]
		public void Test深层数组()
		{
			var view = new View3();
			var host = new TestHost2<List<string>>()
			{
				a = new List<string>(),
			};
			host._Swatch("A", (host, newVal0, oldVal) =>
			{
				var newVal = newVal0 as List<string>;
				view.length = newVal.length;
				view.list0 = newVal.TryGet(0);
				view.list1 = newVal.TryGet(1);
				view.list2 = newVal.TryGet(2);
			});

			host.a.push("对象1");
			VM.Tick.Next();
			expect(view.length).toEqual(1);
			expect(view.list0).toEqual("对象1");

			host.a.push("对象2");
			VM.Tick.Next();
			expect(view.length).toEqual(2);
			expect(view.list1).toEqual("对象2");

			host.a.push("对象3");
			VM.Tick.Next();
			expect(view.length).toEqual(3);
			expect(view.list2).toEqual("对象3");

			host.a.push("对象4");
			VM.Tick.Next();
			expect(view.length).toEqual(4);


			host.a[1] = "修改对象2";
			VM.Tick.Next();
			expect(view.list1).toEqual("修改对象2");
			expect(view.length).toEqual(4);

			host.a.RemoveAt(2);
			VM.Tick.Next();
			expect(view.list1).toEqual("修改对象2");
			expect(view.list2).toEqual("对象4");
			expect(view.length).toEqual(3);

		}

		[Test]
		public void Testthis解释()
		{
			var view = new View4();

			var host = new TestHost2_1()
			{
				testString = "a",
				tstNumber = 1,
				subObj = new TestHost1()
				{
					tstNumber = 2,
				},
				testArr = new List<number>()
				{
					1,2,
				},
			};
			host._Swatch("this.TestString", (host, newVal, oldVal) =>
			{
				view.testString = (string)newVal;
			});
			host._Swatch("this.TstNumber", (host, newVal, oldVal) =>
			{
				view.tstNumber = (number)newVal;
			});
			host._Swatch("this.SubObj.TstNumber", (host, newVal, oldVal) =>
			{
				view.testObj_num1 = (number)newVal;
			});
			host._Swatch("this.TestArr[1]", (host, newVal, oldVal) =>
			{
				view.testArr_1 = (number)newVal;
			});

			host.TestString = "哈哈哈";
			host.TstNumber = 22;
			host.SubObj.TstNumber = 33;
			host.TestArr[1] = 44;

			VM.Tick.Next();

			expect(view.testString).toEqual("哈哈哈");
			expect(view.tstNumber).toEqual(22);
			expect(view.testObj_num1).toEqual(33);
			expect(view.testArr_1).toEqual(44);


		}

		[Test]
		public void TestTick()
		{
			var Boos = new Dictionary<string, object>()
			{
				{"boss", new TBoss(){
					active = false,
					hpMax = 10,
					hp = 5,
				}}
			};
			var newHost = Boos;

			number progress = 0;
			{
				var w = newHost._Swatch("boss.Hp/boss.HpMax", (host, newVal, oldVal) =>
				{
					progress = (number)newVal;
				}, -1);
				progress = (number)w?.value;
				VM.Tick.Next();
				expect(progress).toBe(0.5);

				Boos["boss"] = null;
				VM.Tick.Next();
				expect(progress).toBe(-1);

				Boos["boss"] = new TBoss()
				{
					active = false,
					hpMax = 10,
					hp = 1,
				};
				VM.Tick.Next();
				expect(progress).toBe(0.1);
			}

			(Boos["boss"] as TBoss).HpMax = 10;
			(Boos["boss"] as TBoss).Hp = 5;
			VM.Tick.Next();
			expect(progress).toBe(0.5);
		}
	}
}
