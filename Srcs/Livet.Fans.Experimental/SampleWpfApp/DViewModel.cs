using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Livet;
using Livet.Commands;
using Livet.Fans.Experimental;
using Livet.Fans.Experimental.EventListeners.WeakEvents;

namespace SampleWpfApp
{
    class DViewModel : ViewModel
    {
        private string name3;
        public string Name3
        {
            get { return name3; }
            set { this.SetProperty(ref name3, value); }
        }

        private string name4;
        public string Name4
        {
            get { return name4; }
            set { this.SetProperty(ref name4, value); }
        }

        private LivetWeakEventListener<PropertyChangedEventHandler, PropertyChangedEventArgs> _Listener;

        public DViewModel()
        {
            var listener = new LivetWeakEventListener<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                h => new PropertyChangedEventHandler(h),
                h => this.PropertyChanged += h,
                h => this.PropertyChanged -= h,
                (s, e) => 
                {
                    switch (e.PropertyName)
                    {
                        case nameof(Name3):
                            Console.WriteLine($"Name3: {Name3}");
                            break;

                        case nameof(Name4):
                            Console.WriteLine($"Name4: {Name4}");
                            break;
                    }
                });

            _Listener = listener;

        }

    }
}
