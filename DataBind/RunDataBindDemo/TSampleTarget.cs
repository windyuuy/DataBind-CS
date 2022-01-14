using DataBinding;
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
    }

    public class TTest1 : IHostStand
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyGetEventHandler PropertyGot;

        public virtual void NotifyPropertyGot(object value, [CallerMemberName] string propertyName = "")
        {
            PropertyGot?.Invoke(this, new PropertyGetEventArgs(propertyName, value));
        }
        public virtual void NotifyPropertyChanged(object newValue, object oldValue, [CallerMemberName] string propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName, newValue, oldValue));
        }

        public Hello ffff;
        public Hello FFF
        {
            get
            {
                var v0 = ffff;
                NotifyPropertyGot(v0);
                return v0;
            }

            set
            {
                ffff = value;
                var xx = 234;
                NotifyPropertyChanged(value, value);
                if (xx > 0)
                {
                    return;
                }
            }
        }
        public TSampleTarget ffff2;
        public TSampleTarget FFF2
        {
            get
            {
                return ffff2;
            }

            set
            {
                ffff2 = value;
            }
        }

		protected Observer ___Sob__;
        public virtual Observer _SgetOb()
        {
            return ___Sob__;
        }

        public virtual void _SsetOb(Observer value)
        {
            ___Sob__ = value;
        }

    }
    public class TSampleHost:IHostStand
    {

        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyGetEventHandler PropertyGot;

        protected System.Collections.Generic.ICollection<Watcher> _Swatchers;
        System.Collections.Generic.ICollection<Watcher> GetWatchers22()
        {
            //if (this._Swatchers != null)
            //{
            //    return this._Swatchers;
            //}
            //else
            //{
            //    this._Swatchers = new System.Collections.Generic.List<Watcher>();
            //    return this._Swatchers;
            //}
            return this._Swatchers!=null?this._Swatchers:this._Swatchers = new System.Collections.Generic.List<Watcher>();
        }
    }

    [Observable]
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
        public string wf2 { get;  }
        public string wf3 { set { ffff = int.Parse(value); } }
    }
}
