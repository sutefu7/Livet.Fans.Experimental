using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Livet;
using Livet.Commands;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Reactive.Linq;

namespace SampleWpfApp
{
    class AViewModel : ViewModel
    {
        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                RaisePropertyChanged();

                // Livet.Commands.ViewModelCommand.RaiseCanExecuteChanged で NullReferenceException が発生する
                // http://ts7u.blogspot.com/2014/02/livetcommandsviewmodelcommandraisecanex.html
                ZeroCommand.RaiseCanExecuteChanged();
                OneCommand.RaiseCanExecuteChanged();
            }
        }

        public ViewModelCommand ZeroCommand { get; set; }
        public ListenerCommand<EventArgs> OneCommand { get; set; }

        public ReactiveProperty<string> Name2 { get; set; }
        public ReactiveCommand R1Command { get; set; }
        public ReactiveCommand<EventArgs> R2Command { get; set; }

        public AViewModel()
        {
            ZeroCommand = new ViewModelCommand(
                () => Console.WriteLine($"command1: {Name}"),
                () => !string.IsNullOrWhiteSpace(Name));

            OneCommand = new ListenerCommand<EventArgs>(
                (x) => Console.WriteLine($"command2: {Name}, x: {x}"),
                () => !string.IsNullOrWhiteSpace(Name));

            Name2 = new ReactiveProperty<string>().AddTo(this.CompositeDisposable);

            R1Command = Name2.Select(x => !string.IsNullOrWhiteSpace(Name2.Value)).ToReactiveCommand().AddTo(this.CompositeDisposable);
            R1Command.Subscribe(() => Console.WriteLine($"command3: {Name2.Value}"));

            R2Command = Name2.Select(x => !string.IsNullOrWhiteSpace(Name2.Value)).ToReactiveCommand<EventArgs>().AddTo(this.CompositeDisposable);
            R2Command.Subscribe((x) => Console.WriteLine($"command4: {Name2.Value}, x: {x}"));

        }

        public void Button_Click()
        {
            Console.WriteLine($"method1: {Name}");
        }

        public void Button_Click(EventArgs e)
        {
            Console.WriteLine($"method2: {Name}, x: {e}");
        }

    }
}
