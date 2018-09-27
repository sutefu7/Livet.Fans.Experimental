using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Livet;
using Livet.Commands;
using Livet.EventListeners.WeakEvents;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace SampleWpfApp
{
    class GViewModel : ViewModel
    {
        private string name1;
        public string Name1
        {
            get { return name1; }
            set { this.SetProperty(ref name1, value); }
        }

        //private GrandParent grandParent;
        //public GrandParent GrandParent
        //{
        //    get { return grandParent; }
        //    set { this.SetProperty(ref grandParent, value); }
        //}
        public GrandParent GrandParent { get; set; }

        public ReactiveProperty<string> Name3 { get; set; }
        public ViewModelCommand Check1Command { get; set; }
        public ViewModelCommand Check2Command { get; set; }
        public ViewModelCommand Check3Command { get; set; }

        public GViewModel()
        {
            GrandParent = new GrandParent();
            Name3 = new ReactiveProperty<string>();

            Check1Command = new ViewModelCommand(() => { }, () => !string.IsNullOrEmpty(this.Name1))
                .ObservesProperty(() => this.Name1);

            Check2Command = new ViewModelCommand(() => { }, () => !string.IsNullOrEmpty(this.GrandParent.Parent.Child.Name))
                .ObservesProperty(() => this.GrandParent.Parent.Child.Name);

            Check3Command = new ViewModelCommand(() => { }, () => !string.IsNullOrEmpty(Name3.Value))
                .ObservesProperty(() => this.Name3.Value);
        }

    }

    class Child : NotificationObject
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { this.SetProperty(ref name, value); }
        }
    }

    class Parent
    {
        public Child Child { get; set; }

        public Parent()
        {
            Child = new Child();
        }
    }

    class GrandParent
    {
        public Parent Parent { get; set; }
        
        public GrandParent()
        {
            Parent = new Parent();
        }
    }
    
}
