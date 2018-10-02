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
using Livet.Fans.Experimental;
using Livet.Messaging;

namespace SampleWpfApp
{
    /// <summary>
    /// Window6.xaml の相互作用ロジック
    /// </summary>
    public partial class Window6 : Window
    {
        public Window6()
        {
            InitializeComponent();
        }
    }

    public class Window6ViewModel : ViewModel
    {
        public async void Button_Click()
        {
            // メッセージボックス（情報）
            await Messenger.RaiseAsync(new InformationMessage("this is a test", "情報", MessageBoxImage.Information, "Info"));

            // メッセージボックス（警告）
            await Messenger.RaiseAsync(new InformationMessage("this is a test", "警告", MessageBoxImage.Warning, "Warn"));

            // メッセージボックス（エラー）
            await Messenger.RaiseAsync(new InformationMessage("this is a test", "エラー", MessageBoxImage.Error, "Err"));

            // メッセージボックス（確認、Yes/No/Cancel）
            var mes = new ConfirmationMessage("this is a test", "確認", MessageBoxImage.Question, MessageBoxButton.YesNoCancel, MessageBoxResult.Cancel, "Confirm");
            mes = await Messenger.GetResponseAsync(mes);

            //if (mes.Response == null)
            //    Console.WriteLine("Cancel 押した");

            //if (mes.Response.HasValue && mes.Response.Value)
            //    Console.WriteLine("Yes/OK 押した");

            //if (mes.Response.HasValue && !mes.Response.Value)
            //    Console.WriteLine("No 押した");

            // 上記と同等
            switch (mes.ClickedButton())
            {
                case MessageBoxResult.OK: Console.WriteLine("OK ボタンを押した"); break;
                case MessageBoxResult.Cancel: Console.WriteLine("Cancel ボタンを押した"); break;
                case MessageBoxResult.Yes: Console.WriteLine("Yes ボタンを押した"); break;
                case MessageBoxResult.No: Console.WriteLine("No ボタンを押した"); break;
            }

            // メッセージボックス（確認、OK/Cancel）
            mes = new ConfirmationMessage("this is a test", "確認", MessageBoxImage.Question, MessageBoxButton.OKCancel, MessageBoxResult.Cancel, "Confirm");
            mes = await Messenger.GetResponseAsync(mes);

            // TODO: 列挙体で判定したい
            switch (mes.ClickedButton())
            {
                case MessageBoxResult.OK: Console.WriteLine("OK ボタンを押した"); break;
                case MessageBoxResult.Cancel: Console.WriteLine("Cancel ボタンを押した"); break;
                case MessageBoxResult.Yes: Console.WriteLine("Yes ボタンを押した"); break;
                case MessageBoxResult.No: Console.WriteLine("No ボタンを押した"); break;
            }

        }
        
    }

}
