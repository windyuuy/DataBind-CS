using DataBinding;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RunDataBindDemo
{
    [Watchable]
    public class TSampleTarget : INotifyPropertyChanged
    {
        private static void NotifyPropertyChanged(INotifyPropertyChanged self, PropertyChangedEventHandler PropertyChanged, [CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(self, new PropertyChangedEventArgs(propertyName));
        }

        public double doubleFV2;
        public double DoubleFV2
        {
            get => doubleFV2;
            set
            {
                doubleFV2 = value;
                NotifyPropertyChanged(this,this.PropertyChanged, "DoubleFV2");
                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DoubleFV2"));
            }
        }
        public double DoubleFV {
            get;
            set;
        }
        public bool boolFV { get; set; }
        public string stringFV { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
