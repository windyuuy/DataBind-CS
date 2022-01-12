using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using vm;

namespace TestDataBind.DataObserver
{
    using number = System.Double;

    class Host : vm.Host
    {
        public string testString = "a";
        public string TestString
        {
            get {
                NotifyPropertyGot(testString);
                return testString; }
            set {
                var v0 = testString;
                testString = value; 
                NotifyPropertyChanged(value, v0);
            }
        }
        public number tstNumber = 1;
        public number TstNumber
        {
            get {
                NotifyPropertyGot(testString);
                return tstNumber; }
            set {
                var v0 = tstNumber;
                tstNumber = value;
                NotifyPropertyChanged(value, v0);
            }
        }
    }

    public class WatcherTest:TestEnv
    {
        [Test]
        public void test简单数值绑定()
        {
            var view = new Host() {
                testString = "",
                tstNumber = 0,
            };

            var host = new Host();
            host._Swatch("TestString", (host, newVal, oldVal) => {
                view.testString = (string)newVal;
            });
            host._Swatch("TstNumber", (host, newVal, oldVal) => {
                view.tstNumber = (number)newVal;
            });

            host.TestString = "哈哈哈";
            vm.Tick.next();
            expect(view.testString).toEqual("哈哈哈");

            host.TstNumber = 22;
            vm.Tick.next();
            expect(view.tstNumber).toEqual(22);

        }
    }
}
