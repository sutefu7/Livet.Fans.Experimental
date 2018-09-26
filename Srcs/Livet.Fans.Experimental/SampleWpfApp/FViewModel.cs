using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Livet;
using System.Reactive.Disposables;
using Reactive.Bindings;
using System.Reactive.Linq;
using Reactive.Bindings.Extensions;

namespace SampleWpfApp
{
    class FViewModel : ViewModel
    {
        public ReactiveProperty<string> Name1 { get; set; }
        public ReactiveCommand Check1Command { get; set; }

        // コーディング中の AddTo() の名前解決テストです。そのため本 ViewModel は、どの View にバインドしていません。
        public FViewModel()
        {
            Name1 = new ReactiveProperty<string>()
                .AddTo(this.CompositeDisposable);

            Check1Command = Name1
                .Select(x => !string.IsNullOrWhiteSpace(Name1.Value))
                .ToReactiveCommand()
                .AddTo(this);
            
        }


    }
}
