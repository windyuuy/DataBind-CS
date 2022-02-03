using DataBinding;
using System;
using System.Runtime.CompilerServices;
using vm;

namespace RunDataBindDemo
{
    public interface ITest
    {
        //void Print() => console.log("RunDataBindDemo::Print()!");
        public double CCC { get; set; }
        public string SS { get; set; }
        public event PropertyGetEventHandler PropertyGet2;
    }
    public class T2 : vm.IObservable
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

    public class TSampleHost2 : IHost, IStdHost
    {
        public void _SaddWatcher(Watcher watcher)
        {
            this.AddWatcher(watcher);
        }

        public void _Sdestroy()
        {
            this.Destroy();
        }

        public void _SremoveWatcher(Watcher watcher)
        {
            this.RemoveWatcher(watcher);
        }

        public Watcher _Swatch(CombineType<object, string, Func<object, object, object>> expOrFn, Action<object, object, object> cb, CombineType<object, string, double, bool> loseValue, bool sync)
        {
            return this.Watch(expOrFn, cb, loseValue, sync);
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

        public int kkk = 235;
    }

    [StdHost]
    public class TestStdHostAttr
    {
        public int aaa = 234;
        public int bbb { get; set; } = 23;
    }

    [StdHost][AutoFieldProperty]
    public class TestStdHostAttr2
    {
        public int aaa = 234;
        public int bbb { get; set; } = 23;
    }
}
