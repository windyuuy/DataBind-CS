using DataBinding;
using System;
using System.Runtime.CompilerServices;
using VM;

namespace RunDataBindDemo
{
    public interface ITest
    {
        //void Print() => console.log("RunDataBindDemo::Print()!");
        public double CCC { get; set; }
        public string SS { get; set; }
        public event PropertyGetEventHandler PropertyGet2;
    }
    public class T2 : VM.IObservable
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyGetEventHandler PropertyGot;

        public Observer _SgetOb()
        {
            throw new System.NotImplementedException();
        }

        public void _SsetOb(Observer value)
        {
            throw new System.NotImplementedException();
        }
    }
    public class Demo
    {
        //public event RelationChangedEventHandler RelationChanged;
        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyGetEventHandler PropertyGot;

        public virtual void NotifyPropertyGot(object value, string propertyName = "")
        {
            this.PropertyGot?.Invoke(this, new PropertyGetEventArgs(propertyName, value));
        }
        public virtual void NotifyPropertyChanged(object newValue, object oldValue, string propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName, newValue, oldValue));
        }

        protected System.Collections.Generic.ICollection<Watcher> _Swatchers;
        System.Collections.Generic.ICollection<Watcher> GetWatchers()
        {
            if (this._Swatchers != null)
            {
                return this._Swatchers;
            }
            else
            {
                this._Swatchers = new System.Collections.Generic.List<Watcher>();
                return this._Swatchers;
            }
        }
    }

    public class TSampleTarget
    {
        public double DoubleFV {
            get;
            set;
        }
        //public virtual double CCC { get; set; }
        //public event PropertyGetEventHandler PropertyGet2;
        //public void emit()
        //{
        //    var evt=this.GetType().GetField("PropertyGet2",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance);
        //    var wef = evt.GetValue(this);
        //    var callInst=(PropertyGetEventHandler)wef;
        //    callInst(this,new PropertyGetEventArgs("wf",234));
        //}
    }

    public struct Hello
    {
        public int wfe;

        //public static bool operator +(Hello lhs, Hello rhs){return true;}
        //public static bool operator -(Hello lhs, Hello rhs){return true;}
        //public static bool operator *(Hello lhs, Hello rhs){return true;}
        //public static bool operator /(Hello lhs, Hello rhs){return true;}
        //public static bool operator %(Hello lhs, Hello rhs){return true;}
        //public static bool operator ^(Hello lhs, Hello rhs){return true;}
        //public static bool operator !=(Hello lhs, Hello rhs) { return true;}
        //public static bool operator ==(Hello lhs, Hello rhs) { return true;}
        //public static bool operator >=(Hello lhs, Hello rhs) { return true;}
        //public static bool operator <=(Hello lhs, Hello rhs) { return true;}
        public static bool operator ^(Hello lhs, Hello rhs) { return true; }


    }

    public class TSampleHost:IStdHost
    {
        public virtual string KKK { get; set; }
        public virtual int KKK2 { get; set; }
    }

    public struct Hello2
    {
        public int wfe;
        public int fefge;
    }

    public class TSampleHost2 : IHost, IStdHost
    {
        (int, int,int,int,int,int,int,int,int,int,int,int,int) ff = (32, 23,1,2,3,4,5,6,7,8,2,3,4);
        (int wef, float ff) ff1 = (32, 23);
        Hello2 klwk = new Hello2();

        System.Collections.Generic.Dictionary<string, string> wfe = new System.Collections.Generic.Dictionary<string, string>()
        {
            ["lwkjfe"] = "lkwefj",
        };

        public virtual void _SaddWatcher(Watcher watcher)
        {
            var wfd = ff1.wef;
            var vwed=new Tuple<int,int>(0,0);
            object wfw = new TSampleTarget();
            if (wfw is IStdHost)
            {
                var wef=wfw as IStdHost;
                Console.Write(wef);
            }

            if (wfw is IStdHost wef2)
            {
                Console.Write(wef2);
            }
            (int, float) ff = (32, 23);
            this.AddWatcher(watcher);

            var xzsxd = new Hello2
            {
                fefge = 234,
                wfe = 243234,
            };
            var we = xzsxd;

            var wef1 = new int[234];
            var wef21 = new Hello2[234];

            for(var i = 0; i < 23; i++)
            {
                wef1[i] = 234;
            }

            Console.Write(xzsxd.wfe);
            Console.Write(we.wfe);

            try
            {
                Console.Write(we.wfe);
                throw new Exception("wekf");
            }catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public virtual void _Sdestroy()
        {
            this.Destroy();
        }

        public virtual void _SremoveWatcher(Watcher watcher)
        {
            this.RemoveWatcher(watcher);
        }

        public virtual Watcher _Swatch(CombineType<object, string, Func<object, object, object>> expOrFn, Action<object, object, object> cb, CombineType<object, string, double, bool> loseValue, bool sync)
        {
            return HostExt2.Watch(this,expOrFn, cb, loseValue, sync);
        }
    }

    public class TSubSampleObserver
    {
        public string CCC { get; set; } = "23";
    }

    [ObservableRecursive]
    public class TSampleObserver
    {

        //public event PropertyChangedEventHandler PropertyChanged;
        //public event PropertyGetEventHandler PropertyGot;

        //public virtual void NotifyPropertyGot222(object value, [CallerMemberName] string propertyName = "")
        //{
        //    PropertyGot?.Invoke(this, new PropertyGetEventArgs(propertyName, value));
        //}
        //public virtual void NotifyPropertyChanged222(object newValue, object oldValue, [CallerMemberName] string propertyName = "")
        //{
        //    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName, newValue, oldValue));
        //}

        public int ffff;
        public int FFFF
        {
            get
            {
                return ffff;
            }

            set
            {
                //if (ffff == value)
                //{
                //    return;
                //}
                ffff = value;
            }
        }

        public string wf { get; set; }
        public string wf2 { get; }
        public string wf3 { set { ffff = int.Parse(value); } }

        public TSubSampleObserver sub { get;set;} =new TSubSampleObserver();
    }

    public class AAA
    {
        protected int kk2=3;
        public int kk=3;
    }

    [AutoFieldProperty]
    public class BBB : AAA
    {
        public int ccc = 234;
        public int eee = 234;
    }

    public class CCC : AAA
    {
        [AutoFieldProperty]
        public int ddd = 235;
        [AutoFieldProperty]
        public static int ddd2 = 235;

        public int kkk = 235;
    }

    [StdHost]
    public class TestStdHostAttr
    {
        public int aaa = 234;
        public int bbb { get; set; } = 23;
    }

    [StdHost]
    [AutoFieldProperty]
    public class TestStdHostAttr2
    {
        public int aaa = 234;
        public int bbb { get; set; } = 23;
    }

    public class TSetFieldSample
    {
        public int aaa = 23;
        public int bbb { get; set; } = 23;
        public static int uuu = 3;
        public static int xxx { get; set; } = 3;

        public int fff
        {
            get
            {
                var ccc = new CCC();
                return ccc.ddd;
            }
            set
            {
                var ccc = new CCC();
                ccc.ddd = value;
            }
        }

        public void setAA()
        {
            aaa = 24;
            bbb = 24;
            ref var qqq = ref aaa;
            var kkk = uuu;
            var zzz = xxx;
        }

        public void setBB()
        {
            var ddd = aaa;

            var ccc= bbb;
        }

        public void setCC()
        {
            var ccc = new CCC();
            var eee = ccc.ddd+2342;
            ccc.ddd = 234;

            var eee2 = CCC.ddd2 + 2342;
            CCC.ddd2 = 234;
        }
    }
}
