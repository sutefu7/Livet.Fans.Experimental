using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Livet;
using Livet.Commands;
using Livet.Fans.Experimental;

namespace SampleWpfApp
{
    class EViewModel : ViewModel
    {

        private string name1;
        public string Name1
        {
            get { return name1; }
            set { this.SetProperty(ref name1, value); }
        }

        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set { this.SetProperty(ref isChecked, value); }
        }

        public ViewModelCommand Check1Command { get; set; }
        public ViewModelCommand Check2Command { get; set; }
        public ListenerCommand<string> Check3Command { get; set; }
        public ListenerCommand<string> Check4Command { get; set; }
        public ViewModelCommand Check5Command { get; set; }
        public ListenerCommand<bool> Check6Command { get; set; }

        public EViewModel()
        {
            Check1Command = new ViewModelCommand(
                () => Console.WriteLine($"Name1: {Name1}"),
                () => !string.IsNullOrWhiteSpace(Name1));

            Check2Command = new ViewModelCommand(
                () => Console.WriteLine($"Name1: {Name1}"),
                () => !string.IsNullOrWhiteSpace(Name1)).ObservesProperty(() => this.Name1);

            Check3Command = new ListenerCommand<string>(
                (x) => Console.WriteLine($"Name1: {Name1}"),
                () => !string.IsNullOrWhiteSpace(Name1));

            Check4Command = new ListenerCommand<string>(
                (x) => Console.WriteLine($"Name1: {Name1}"),
                () => !string.IsNullOrWhiteSpace(Name1)).ObservesProperty(() => this.Name1);

            Check5Command = new ViewModelCommand(
                () => Console.WriteLine($"IsChecked: {IsChecked}")).ObservesCanExecute(() => this.IsChecked);

            Check6Command = new ListenerCommand<bool>(
                (x) => Console.WriteLine($"IsChecked: {IsChecked}")).ObservesCanExecute(() => this.IsChecked);

        }

    }
}
