using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Livet.Fans.Experimental
{
    /// <summary>
    /// BindingExtension クラス向けの作業用データクラスです。
    /// </summary>
    class BindingControlObject : DependencyObject
    {
        // バインド先情報
        public PropertyPath Path { get; set; }

        // WPF コントロール。FrameworkElement, FrameworkContentElement に分かれている。
        // IsEnabled, DataContext, DataContextChanged を内部で使用する。
        // WPF コントロールが
        //     FrameworkElement の場合、0
        //     FrameworkContentElement の場合、1
        public Tuple<int, FrameworkElement, FrameworkContentElement> WpfControl { get; private set; }

        // WPF コントロールのメンバー（プロパティ、イベント等）
        public object WpfMember { get; set; }

        // WPF コントロールにバインドされている（はずの）DataContext(=ViewModel)
        public object WpfDataContext { get; set; }

        // ViewModel
        public object ViewModel
        {
            get => WpfDataContext;
            set => WpfDataContext = value;
        }

        // ViewModel のメンバー（プロパティ、コマンド、メソッド等）
        public object ViewModelMember
        {
            get { return GetValue(ViewModelMemberProperty); }
            set { SetValue(ViewModelMemberProperty, value); }
        }

        public static readonly DependencyProperty ViewModelMemberProperty =
            DependencyProperty.Register(nameof(ViewModelMember), typeof(object), typeof(BindingControlObject), new PropertyMetadata(null));

        /// <summary>
        /// WPF コントロールを元に、DataContext(=ViewModel) とバインド先メンバー (=ViewModel のメンバー) を取得します。 
        /// </summary>
        /// <param name="obj">WPF コントロール</param>
        public void SetWpfControl(object obj)
        {
            switch (obj)
            {
                case FrameworkElement fe:
                    WpfControl = Tuple.Create<int, FrameworkElement, FrameworkContentElement>(0, fe, null);
                    WpfDataContext = fe.DataContext;
                    GetViewModelMember();
                    break;

                case FrameworkContentElement ce:
                    WpfControl = Tuple.Create<int, FrameworkElement, FrameworkContentElement>(1, null, ce);
                    WpfDataContext = ce.DataContext;
                    GetViewModelMember();
                    break;

                default:
                    throw new InvalidOperationException("未対応のコントロールをバインドしています。");
            }
        }

        /// <summary>
        /// 管理しているコントロールのうち、取得できた方の型の WPF コントロールを返却します。
        /// </summary>
        /// <returns></returns>
        public object GetWpfControl()
        {
            var selector = WpfControl.Item1;
            switch (selector)
            {
                case 0: return WpfControl.Item2;
                case 1: return WpfControl.Item3;
                default: throw new NotImplementedException("未対応のコントロールを指定しました。");
            }
        }

        /// <summary>
        /// Path 情報を元に、実際のバインド先メンバーを取得します。
        /// </summary>
        public void GetViewModelMember()
        {
            var binding = new Binding { Source = this.ViewModel, Path = this.Path };
            BindingOperations.SetBinding(this, BindingControlObject.ViewModelMemberProperty, binding);
        }

    }
}
