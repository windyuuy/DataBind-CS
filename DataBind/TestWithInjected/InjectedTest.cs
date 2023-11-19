using NUnit.Framework;
using RunDataBindDemo;
using DataBinding;
using Game.Diagnostics.IO;

namespace TestWithInjected
{
	public class Tests
	{
		[Test]
		public void TestField2Prop()
		{
			var dv = new TestStdHostAttr2();
			var host = new TestStdHostAttr2();
			var nextValue = 0;
			var watcher = host.Watch("aaa", (h, newValue, oldValue) =>
			{
				nextValue = (int)newValue;
			});
			Assert.AreEqual(watcher.value, dv.aaa);
			host.aaa = nextValue;
			vm.Tick.Next();
			Assert.AreEqual(watcher.value, nextValue);
		}

		[Test]
		public void TestInjected()
		{
			var target = new TSampleTarget();
			target.DoubleFV = 234;
			var wef = target.GetType().GetInterface("ITest") != null;
			if (wef)
			{
				var qq = ((ITest)(object)target);
				qq.PropertyGet2 += (a, b) =>
				{
					Console.Log("event called");
				};
				//target.emit();
				//qq.Print();
				Console.Log("yes");
				qq.CCC = 2323;
				qq.SS = "lwkje";
				Console.Log("CCC:", qq.CCC);
				Console.Log("SS:", qq.SS);
			}
			else
			{
				Console.Log("not");
			}
		}

		[Test]
		public void TestObserver()
		{
			var sampleObs = new TSampleObserver();
			if (sampleObs is vm.IObservable sampleObs1)
			{
				var demos = new System.Collections.Generic.List<string>()
				{
					"hello22",
					"hello33",
					"hello22",
				};
				var rets = new System.Collections.Generic.List<string>();
				sampleObs1.PropertyChanged += (host, e) =>
				{
					rets.Add(e.NewValue as string);
					Console.Log("value changed:", e.NewValue);
				};
				sampleObs.wf = demos[0];
				sampleObs.wf = demos[0];
				sampleObs.wf = demos[1];
				sampleObs.wf = demos[1];
				sampleObs.wf = demos[2];
				sampleObs.wf = demos[2];

				Assert.AreEqual(rets.Count, demos.Count);
				Assert.AreEqual(rets, demos);

				rets.Clear();
				sampleObs.sub.PropertyChanged += (host, e) =>
				  {
					  rets.Add(e.NewValue as string);
					  Console.Log("sub value changed:", e.NewValue);
				  };
				sampleObs.sub.CCC = demos[0];
				sampleObs.sub.CCC = demos[0];
				sampleObs.sub.CCC = demos[1];
				sampleObs.sub.CCC = demos[1];
				sampleObs.sub.CCC = demos[2];
				sampleObs.sub.CCC = demos[2];

				Assert.AreEqual(rets.Count, demos.Count);
				Assert.AreEqual(rets, demos);

			}
		}

		[Test]
		public void TestHost()
		{
			var demos = new System.Collections.Generic.List<string>()
			{
				"hello22",
				"hello33",
				"hello22",
			};
			var rets = new System.Collections.Generic.List<string>();

			var sampleHost = new TSampleHost();
			if (sampleHost is IStdHost sampleHost1)
			{
				Console.Log(sampleHost1.IsHostDestroyed());
				sampleHost1.ClearWatchers();
				sampleHost1.Watch("KKK", (host, newValue, oldValue) =>
				{
					rets.Add(newValue as string);
					Console.Log("value changed:", newValue);
				});
				sampleHost.KKK = demos[0];
				vm.Tick.Next();
				sampleHost.KKK = demos[0];
				vm.Tick.Next();
				sampleHost.KKK = demos[1];
				vm.Tick.Next();
				sampleHost.KKK = demos[1];
				vm.Tick.Next();
				sampleHost.KKK = demos[2];
				vm.Tick.Next();
				sampleHost.KKK = demos[2];
				vm.Tick.Next();

				Assert.AreEqual(rets.Count, demos.Count);
				Assert.AreEqual(rets, demos);

				var sampleObs = new TSampleObserver();
				sampleHost1.SetProto(sampleObs);
				sampleHost1.Watch("FFFF", (host, newValue, oldValue) =>
				{
					rets.Add(newValue as string);
					Console.Log("value changed:", newValue);
				});
				sampleObs.FFFF = 234;
				sampleHost.KKK2 = 2;
				vm.Tick.Next();

				var value = vm.Utils.IndexValueRecursive(sampleHost, "FFFF");
				Console.Log("value:", value);

				var exp = new vm.Interpreter("KKK2+FFFF");
				var ret = exp.Run(sampleHost);

				Console.Log("true");
			}
			else
			{
				Console.Log("false");
			}
		}
	}
}