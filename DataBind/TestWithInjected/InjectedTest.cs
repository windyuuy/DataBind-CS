using NUnit.Framework;
using RunDataBindDemo;

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
                sampleObs1.PropertyChanged += (host, e) =>
                {
                    console.log("value changed:",e.NewValue);
                };
                sampleObs.wf = "hello22";
            }
        }
    }
}