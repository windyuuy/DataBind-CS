
using DataBind.CollectionExt;
using EngineAdapter.Diagnostics;
using NUnit.Framework;

namespace TestDataBind.DataObserver.Interperter
{

	// 需要实现 IStdHost 观察者接口
	public class SampleHost : DataBind.VM.Host
	{
		private SampleOB hello1 = new SampleOB();
		private double qQ = 234;

		// 可嵌套引用可观察对象类型
		// 注意：必须使用属性字段
		public SampleOB hello
		{
			get
			{
				NotifyPropertyGot(hello1);
				return hello1;
			}
			set
			{
				var v0 = hello1;
				NotifyPropertyChanged(value, v0);
				hello1 = value;
			}
		}
		public double QQ
		{
			get
			{
				NotifyPropertyGot(qQ);
				return qQ;
			}
			set
			{
				var v0 = qQ;
				NotifyPropertyChanged(value, v0);
				qQ = value;
			}
		}
	}


	// 需要添加Observable特性，使目标成为可观察对象
	public class SampleOB : DataBind.VM.Host
	{
		private double kKK = 234;
		private List<int> intList = new List<int> { 1, 2, 3, 4 };
		private Dictionary<int, string> numDictionary = new Dictionary<int, string>();

		// 注意：必须使用属性字段
		public double KKK
		{
			get
			{
				NotifyPropertyGot(kKK);
				return kKK;
			}
			set
			{
				var v0 = kKK;
				NotifyPropertyChanged(value, v0);
				kKK = value;
			}
		}
		// DataBind.CollectionExt.List 容器数据
		// 注意：必须使用属性字段
		public List<int> IntList
		{
			get
			{
				NotifyPropertyGot(intList);
				return intList;
			}
			set
			{
				var v0 = intList;
				NotifyPropertyChanged(value, v0);
				intList = value;
			}
		}
		// DataBind.CollectionExt.Dictionary 容器数据
		// 注意：必须使用属性字段
		public Dictionary<int, string> NumDictionary
		{
			get
			{
				NotifyPropertyGot(numDictionary);
				return numDictionary;
			}
			set
			{
				var v0 = numDictionary;
				NotifyPropertyChanged(value, v0);
				numDictionary = value;
			}
		}
	}

	public class TestNumCalc
	{
		[Test]
		public void TestNumCalc1()
		{
			var sampleHost = new SampleHost();

			// 监听表达式
			sampleHost._Swatch("QQ+hello.KKK", (host, value, oldValue) =>
			{
				Console.Log("value changed:");
				Console.Log(value);
			});
			sampleHost.QQ = 2134;
			sampleHost.hello.KKK = 3242;
			// 通知表达式值变化
			DataBind.VM.Tick.Next();

			// 监听表达式
			sampleHost._Swatch("hello.IntList[2]", (host, value, oldValue) =>
			{
				Console.Log("value changed:");
				Console.Log(value);
			});
			sampleHost.hello.IntList[2] = 44;
			// 通知表达式值变化
			DataBind.VM.Tick.Next();

			// 监听表达式
			sampleHost._Swatch("hello.NumDictionary[123]", (host, value, oldValue) =>
			{
				Console.Log("value changed:");
				Console.Log(value);
			});
			sampleHost.hello.NumDictionary[123] = "你变了";
			// 通知表达式值变化
			DataBind.VM.Tick.Next();

		}
	}
}
