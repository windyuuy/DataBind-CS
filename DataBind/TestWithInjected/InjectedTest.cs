using NUnit.Framework;
using RunDataBindDemo;
using vm;

namespace TestWithInjected
{
    public class Tests
    {
        [Test]
        public void TestInjected()
        {
            var target = new TSampleTarget();
            target.DoubleFV = 234;
            var wef = target.GetType().GetInterface("ITest")!=null;
            if (wef)
            {
                var qq = ((ITest)(object)target);
                qq.PropertyGet2 += (a, b) =>
                {
                    console.log("event called");
                };
                //target.emit();
                //qq.Print();
                console.log("yes");
                qq.CCC = 2323;
                qq.SS = "lwkje";
                console.log("CCC:",qq.CCC);
                console.log("SS:",qq.SS);
            }
            else
            {
                console.log("not");
            }
        }

        [Test]
        public void TestObserver()
        {
            var sampleObs = new TSampleObserver();
            if(sampleObs is vm.IObservable)
            {
                var sampleObs1=sampleObs as vm.IObservable;
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
                    console.log("value changed:",e.NewValue);
                };
                sampleObs.wf = demos[0];
                sampleObs.wf = demos[0];
                sampleObs.wf = demos[1];
                sampleObs.wf = demos[1];
                sampleObs.wf = demos[2];
                sampleObs.wf = demos[2];

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
            if (sampleHost is vm.IFullHost)
            {
                var sampleHost1 = sampleHost as vm.IFullHost;
                console.log(sampleHost1._SIsDestroyed);
                sampleHost1.GetWatchers().Clear();
                sampleHost1._Swatch("KKK", (host, newValue, oldValue) =>
                {
                    rets.Add(newValue as string);
                    console.log("value changed:", newValue);
                });
                sampleHost.KKK = demos[0];
                vm.Tick.next();
                sampleHost.KKK = demos[0];
                vm.Tick.next();
                sampleHost.KKK = demos[1];
                vm.Tick.next();
                sampleHost.KKK = demos[1];
                vm.Tick.next();
                sampleHost.KKK = demos[2];
                vm.Tick.next();
                sampleHost.KKK = demos[2];
                vm.Tick.next();

                Assert.AreEqual(rets.Count, demos.Count);
                Assert.AreEqual(rets, demos);
                console.log("true");
            }
            else
            {
                console.log("false");
            }
        }
    }
}