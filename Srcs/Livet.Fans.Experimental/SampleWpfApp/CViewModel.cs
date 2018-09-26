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
    class CViewModel : ViewModel
    {
        private string name1;
        public string Name1
        {
            get { return name1; }
            set { this.SetProperty(ref name1, value); }
        }

        private string name2;
        public string Name2
        {
            get { return name2; }
            set { this.SetProperty(ref name2, value); }
        }

        public ViewModelCommand Check1Command { get; set; }

        private LivetWeakEventListener<PropertyChangedEventHandler, PropertyChangedEventArgs> _Listener;

        public CViewModel()
        {
            Check1Command = new ViewModelCommand(() =>
            {
                Console.WriteLine($"Name1: {Name1}");
                Console.WriteLine($"Name2: {Name2}");
            });

            var listener = new LivetWeakEventListener<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                h => new PropertyChangedEventHandler(h),
                h => this.PropertyChanged += h,
                h => this.PropertyChanged -= h,
                (s, e) =>
                {
                    switch (e.PropertyName)
                    {
                        case nameof(Name1):
                            Console.WriteLine($"Name1: {Name1}");
                            break;

                        case nameof(Name2):
                            Console.WriteLine($"Name2: {Name2}");
                            break;
                    }
                });

            _Listener = listener;

        }

    }
}
