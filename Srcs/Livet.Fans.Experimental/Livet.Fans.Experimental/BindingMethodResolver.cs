using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Livet.Behaviors;

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
    /// バインド先メンバーがメソッドの場合の、実行処理の生成と、戻り値を返却するクラスです。
    /// </summary>
    class BindingMethodResolver : IBindingResolver
    {
        private Action<object> _Action;

        /// <summary>
        /// 実行処理の生成と、戻り値を返却します。
        /// </summary>
        /// <param name="controlInfo">View と ViewModel の関連情報</param>
        /// <param name="option">View のオプション指定情報</param>
        /// <returns>戻り値</returns>
        public object GetReturnValue(BindingControlObject controlInfo, BindingOptionObject option)
        {
            // バインド先の ViewModel のメソッドを取得
            if (option.UseEventArgs)
            {
                var method = new MethodBinderWithArgument();
                _Action = x => method.Invoke(controlInfo.ViewModel, option.Path.Path, x);
            }
            else
            {
                var method = new MethodBinder();
                _Action = x => method.Invoke(controlInfo.ViewModel, option.Path.Path);
            }
            
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
                    throw new NotImplementedException($"BindingMethodResolver: {controlInfo.WpfMember.GetType().FullName} は未対応です。");
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
            _Action.Invoke(e);
        }

    }
}
