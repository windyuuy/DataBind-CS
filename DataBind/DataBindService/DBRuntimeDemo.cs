using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vm;

namespace DataBindService
{
    public class DBRuntimeDemo
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
            if(this._Swatchers != null)
            {
                return this._Swatchers;
            }
            else
            {
                this._Swatchers=new System.Collections.Generic.List<Watcher>();
                return this._Swatchers;
            }
        }
    }
}
