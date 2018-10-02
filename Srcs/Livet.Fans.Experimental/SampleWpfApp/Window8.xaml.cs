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
using Livet.Messaging;
using Livet.Fans.Experimental;

namespace SampleWpfApp
{
    /// <summary>
    /// Window8.xaml の相互作用ロジック
    /// </summary>
    public partial class Window8 : Window
    {
        public Window8()
        {
            InitializeComponent();
        }
    }

    public class Window8ViewModel : ViewModel
    {
        public async void Button_Click()
        {
            this.ShowInformationMessage("this is a test");
            await this.ShowInformationMessageAsync("this is a test");

            var mes = await this.ShowConfirmationYesNoCancelMessageAsync("this is a test", defaultResult: MessageBoxResult.Cancel);
            switch (mes.ClickedButton())
            {
                case MessageBoxResult.Yes: Console.WriteLine("Yes"); break;
                case MessageBoxResult.No: Console.WriteLine("No"); break;
                case MessageBoxResult.Cancel: Console.WriteLine("Cancel"); break;
            }

            //this.Close();
            //await this.CloseAsync();

            //this.Minimize();
            //await this.MinimizeAsync();

            //this.Maximize();
            //await this.MaximizeAsync();

            // 3秒間の間に、手動で画面を非アクティブにしておく、という確認
            //await Task.Delay(3000);
            ////this.Active();
            //await this.ActiveAsync();

        }
    }

}
