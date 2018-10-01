using Livet;
using Livet.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SampleWpfApp
{
    class HViewModel : ViewModel
    {

        private string name = "a";
        public string Name
        {
            get { return name; }
            set { this.SetProperty(ref name, value); }
        }
        
        private ViewModelCommand clickCommand;
        public ViewModelCommand ClickCommand
        {
            get
            {
                return this.SetCommand(ref clickCommand, () => { }, null, () => Name);
            }
        }

        public void MouseUp(MouseButtonEventArgs e)
        {
            Console.WriteLine($"MouseUp: {e.ButtonState}");
        }

    }
}
