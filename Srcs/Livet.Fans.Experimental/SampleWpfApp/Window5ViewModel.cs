using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Livet;

namespace SampleWpfApp
{
    class Window5ViewModel : ViewModel
    {
        private string name = "Auto ViewModel Binding Test";
        public string Name
        {
            get { return name; }
            set { this.SetProperty(ref name, value); }
        }
    }
}
