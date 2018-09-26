using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Livet;
using Livet.EventListeners.WeakEvents;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Reactive.Disposables;

namespace SampleWpfApp
{
    class BViewModel : ViewModel
    {
        private string name1;
        public string Name1
        {
            get { return name1; }
            set { name1 = value; RaisePropertyChanged(); }
        }
        
        private string name2;
        public string Name2
        {
            get { return name2; }
            set { name2 = value; RaisePropertyChanged(); }
        }

        private string name3;
        public string Name3
        {
            get { return name3; }
            set { name3 = value; RaisePropertyChanged(); }
        }

        private string name4;
        public string Name4
        {
            get { return name4; }
            set { name4 = value; RaisePropertyChanged(); }
        }

        private string name5;
        public string Name5
        {
            get { return name5; }
            set { name5 = value; RaisePropertyChanged(); }
        }

        public ReactiveProperty<string> Name6 { get; set; }
        public ReactiveProperty<string> Name7 { get; set; }
        public ReactiveCommand ConfirmCommand { get; set; } // 全ての Name* の値を確認

        // Mode の設定具合の確認。値が変更されたら表示する
        public LivetWeakEventListener<PropertyChangedEventHandler, PropertyChangedEventArgs> Listener1;
        public LivetWeakEventListener<PropertyChangedEventHandler, PropertyChangedEventArgs> Listener2;
        public LivetWeakEventListener<PropertyChangedEventHandler, PropertyChangedEventArgs> Listener3;

        public BViewModel()
        {
            Name6 = new ReactiveProperty<string>().AddTo(this.CompositeDisposable);
            Name7 = new ReactiveProperty<string>().AddTo(this.CompositeDisposable);
            ConfirmCommand = new ReactiveCommand().AddTo(this.CompositeDisposable);
            ConfirmCommand.Subscribe(() =>
            {
                Console.WriteLine($"Name1: {Name1}");
                Console.WriteLine($"Name2: {Name2}");
                Console.WriteLine($"Name3: {Name3}");
                Console.WriteLine($"Name4: {Name4}");
                Console.WriteLine($"Name5: {Name5}");
                Console.WriteLine($"Name6: {Name6.Value}");
                Console.WriteLine($"Name7: {Name7.Value}");
            });

            Listener1 = new LivetWeakEventListener<PropertyChangedEventHandler, PropertyChangedEventArgs>(
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

                        case nameof(Name3):
                            Console.WriteLine($"Name3: {Name3}");
                            break;

                        case nameof(Name4):
                            Console.WriteLine($"Name4: {Name4}");
                            break;

                        case nameof(Name5):
                            Console.WriteLine($"Name5: {Name5}");
                            break;

                    }
                }).AddTo(this.CompositeDisposable);

            Listener2 = new LivetWeakEventListener<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                h => new PropertyChangedEventHandler(h),
                h => Name6.PropertyChanged += h,
                h => Name6.PropertyChanged -= h,
                (s, e) => Console.WriteLine($"Name6: {Name6.Value}")).AddTo(this.CompositeDisposable);

            Listener3 = new LivetWeakEventListener<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                h => new PropertyChangedEventHandler(h),
                h => Name7.PropertyChanged += h,
                h => Name7.PropertyChanged -= h,
                (s, e) => Console.WriteLine($"Name7: {Name7.Value}")).AddTo(this.CompositeDisposable);

        }
        
    }
}
