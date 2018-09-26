using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Livet.EventListeners.WeakEvents;

/*
 * 本処理は、以下のプログラムをベースに作成しました。
 * 
 * WPF4.5の新機能～「イベントのマークアップ拡張」で、イベント発生時のコマンド呼び出しをスッキリ記述する～
 * http://sourcechord.hatenablog.com/entry/2014/12/08/030947
 * 
 * ※最新ソース
 * https://github.com/sourcechord/MarkupExtensionsForEvents
 * 
 */

namespace Livet.Fans.Experimental
{
    /// <summary>
    /// バインド先メンバーが ICommand の場合の、実行処理の生成と、戻り値を返却するクラスです。
    /// </summary>
    class BindingICommandResolver : IBindingResolver
    {
        private ICommand _Command;
        private LivetWeakEventListener<EventHandler, EventArgs> _Listener; // 突然、CanExecuteChanged イベントが発行されなくなるバグの修正。クラス変数にセットして生存させておくように対応

        /// <summary>
        /// WPF コントロールの活性状態の切り替え、実行処理の生成と、戻り値を返却します。
        /// </summary>
        /// <param name="controlInfo">View と ViewModel の関連情報</param>
        /// <param name="option">View のオプション指定情報</param>
        /// <returns>戻り値</returns>
        /// <remarks>ここでは option は未使用</remarks>
        public object GetReturnValue(BindingControlObject controlInfo, BindingOptionObject option)
        {
            var command = controlInfo.ViewModelMember as ICommand;
            
            // 初回バインド時の活性制御
            var selector = controlInfo.WpfControl.Item1;
            if (selector == 0)
            {
                var element = controlInfo.WpfControl.Item2;
                if (command.CanExecute(EventArgs.Empty))
                    element.IsEnabled = true;
                else
                    element.IsEnabled = false;
            }
            else
            {
                var element = controlInfo.WpfControl.Item3;
                if (command.CanExecute(EventArgs.Empty))
                    element.IsEnabled = true;
                else
                    element.IsEnabled = false;
            }

            // CanExecuteChanged を監視して、バインド先の WPF コントロールの活性状態を自動切り替えする
            var listener = new LivetWeakEventListener<EventHandler, EventArgs>(
                h => new EventHandler(h),
                h => command.CanExecuteChanged += h,
                h => command.CanExecuteChanged -= h,
                (s, e) =>
                {
                    var selector2 = controlInfo.WpfControl.Item1;
                    if (selector2 == 0)
                    {
                        var element = controlInfo.WpfControl.Item2;
                        if (command.CanExecute(e))
                            element.IsEnabled = true;
                        else
                            element.IsEnabled = false;
                    }
                    else
                    {
                        var element = controlInfo.WpfControl.Item3;
                        if (command.CanExecute(e))
                            element.IsEnabled = true;
                        else
                            element.IsEnabled = false;
                    }
                });

            _Command = command;
            _Listener = listener;
            
            // 戻り値の準備
            var t = default(Type);
            switch (controlInfo.WpfMember)
            {
                case EventInfo ei:
                    t = ei.EventHandlerType;
                    break;

                case MethodInfo mi:
                    t = mi.GetParameters()[1].ParameterType;
                    break;

                default:
                    throw new NotImplementedException($"BindingICommandResolver: {controlInfo.WpfMember.GetType().FullName} は未対応です。");
            }
            
            // ここで、イベントハンドラを作成し、マークアップ拡張の結果として返す
            var nonGenericMethod = GetType().GetMethod("GenericEventHandler", BindingFlags.NonPublic | BindingFlags.Instance);
            var argType = t.GetMethod("Invoke").GetParameters()[1].ParameterType;
            var genericMethod = nonGenericMethod.MakeGenericMethod(argType);

            return Delegate.CreateDelegate(t, this, genericMethod);

        }

        /// <summary>
        /// 戻り値となる汎用イベントハンドラのひな形です。
        /// リフレクション経由で、ジェネリックなイベント引数型が決定されて、呼び出されます。
        /// </summary>
        /// <typeparam name="T">EventArgs の型</typeparam>
        /// <param name="sender">イベント発生したコントロール</param>
        /// <param name="e">イベント引数</param>
        private void GenericEventHandler<T>(object sender, T e)
        {
            if (_Command.CanExecute(e))
                _Command.Execute(e);

        }

    }
}
