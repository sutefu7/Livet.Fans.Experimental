using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Livet;
using Livet.Commands;
//using Livet.Fans.Experimental;
using Livet.Messaging;

namespace SampleWpfApp
{
    /// <summary>
    /// Window7.xaml の相互作用ロジック
    /// </summary>
    public partial class Window7 : Window
    {
        public Window7()
        {
            InitializeComponent();
        }
    }

    public class Window7ViewModel : ViewModel
    {
        public async void Button_Click()
        {
            this.Messenger.Raise(new InformationMessage("情報だよ", "情報", MessageBoxImage.Information, "Info"));
            await this.Messenger.RaiseAsync(new InformationMessage("情報だよ", "情報", MessageBoxImage.Information, "Info"));

            // 上記と同等
            this.ShowInformationMessage("情報だよ");
            this.ShowWarningMessage("警告だよ");
            this.ShowErrorMessage("エラーだよ");

            await this.ShowInformationMessageAsync("情報だよ");
            await this.ShowWarningMessageAsync("警告だよ");
            await this.ShowErrorMessageAsync("エラーだよ");

            ConfirmationMessage mes = this.ShowConfirmationOKCancelMessage("確認だよ");
            mes = this.ShowConfirmationYesNoMessage("確認だよ");
            mes = this.ShowConfirmationYesNoCancelMessage("確認だよ");

            mes = await this.ShowConfirmationOKCancelMessageAsync("確認だよ");
            mes = await this.ShowConfirmationYesNoMessageAsync("確認だよ");
            mes = await this.ShowConfirmationYesNoCancelMessageAsync("確認だよ");
        }
        
    }

}
