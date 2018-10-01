using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Livet;
using Livet.Commands;

/*
 * 以下の移植です。
 * Prism
 * BindableBase.cs
 * https://github.com/PrismLibrary/Prism/blob/master/Source/Prism/Mvvm/BindableBase.cs
 * 
 * 
 * 処理内部で、CommandExtensions.cs にあるメソッド (ObservesProperty() ) を呼び出している（依存している）事に注意
 * 
 * 
 */


namespace Livet
{
    /// <summary>
    /// NotificationObject に対する拡張クラスです。
    /// </summary>
    public static class NotificationObjectExtensions
    {
        // RaisePropertyChanged() （というか PropertyChanged イベント）の影響範囲はその ViewModel, Model クラス内だけなので、
        // 登録しようとしているプロパティが属する各 ViewModel, Model クラス数分の RaisePropertyChanged() を用意して使う
        private static Dictionary<string, Action<string>> _Cache = null;

        /// <summary>
        /// 変更通知プロパティを実現するためのヘルパーメソッドです。
        /// 現在値と新しい値が違う場合、PropertyChanged イベントを発行します。
        /// </summary>
        /// <typeparam name="T">プロパティの型</typeparam>
        /// <param name="self">NotificationObject</param>
        /// <param name="storage">現在値</param>
        /// <param name="value">新しい値</param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static bool SetProperty<T>(this NotificationObject self, ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;

            storage = value;

            if (_Cache == null)
                _Cache = new Dictionary<string, Action<string>>();

            var selfType = self.GetType();
            if (!_Cache.ContainsKey(selfType.FullName))
            {
                // protected virtual void RaisePropertyChanged<T>(Expression<Func<T>>) ではなく、
                // protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName="") を取得して使う
                var mi = selfType
                    .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                    .FirstOrDefault(x => x.Name == "RaisePropertyChanged" && x.GetParameters().Any(y => y.ParameterType.Equals(typeof(string))));

                var method = (Action<string>)Delegate.CreateDelegate(typeof(Action<string>), self, mi);
                _Cache.Add(selfType.FullName, method);
            }

            var RaisePropertyChanged = _Cache[selfType.FullName];
            RaisePropertyChanged(propertyName);

            return true;
        }

        /// <summary>
        /// コマンドのインスタンス生成のためのヘルパーメソッドです。
        /// </summary>
        /// <param name="self">NotificationObject</param>
        /// <param name="command">ViewModelCommand</param>
        /// <param name="execute">実行処理</param>
        /// <param name="canExecute">実行処理の実行有無</param>
        /// <returns>ViewModelCommand</returns>
        public static ViewModelCommand SetCommand(this NotificationObject self, ref ViewModelCommand command, Action execute, Func<bool> canExecute = null)
        {
            return SetCommand<object>(self, ref command, execute, canExecute, null);
        }

        /// <summary>
        /// コマンドのインスタンス生成のためのヘルパーメソッドです。
        /// </summary>
        /// <typeparam name="TProperty">監視対象プロパティの型</typeparam>
        /// <param name="self">NotificationObject</param>
        /// <param name="command">ViewModelCommand</param>
        /// <param name="execute">実行処理</param>
        /// <param name="canExecute">実行処理の実行有無</param>
        /// <param name="observesProperty">監視したいプロパティを返却するデリゲート</param>
        /// <returns>ViewModelCommand</returns>
        public static ViewModelCommand SetCommand<TProperty>(this NotificationObject self, ref ViewModelCommand command, Action execute, Func<bool> canExecute = null, Expression<Func<TProperty>> observesProperty = null)
        {
            if (command == null)
            {
                if (canExecute == null)
                    canExecute = () => true;

                command = new ViewModelCommand(execute, canExecute);

                if (observesProperty != null)
                    command.ObservesProperty(observesProperty);
            }

            return command;
        }

        /// <summary>
        /// コマンドのインスタンス生成のためのヘルパーメソッドです。
        /// </summary>
        /// <typeparam name="T">任意の型</typeparam>
        /// <param name="self">NotificationObject</param>
        /// <param name="command">ListenerCommand<T></param>
        /// <param name="execute">実行処理</param>
        /// <param name="canExecute">実行処理の実行有無</param>
        /// <returns>ListenerCommand<T></returns>
        public static ListenerCommand<T> SetCommand<T>(this NotificationObject self, ref ListenerCommand<T> command, Action<T> execute, Func<bool> canExecute = null)
        {
            return SetCommand<T, object>(self, ref command, execute, canExecute, null);
        }

        /// <summary>
        /// コマンドのインスタンス生成のためのヘルパーメソッドです。
        /// </summary>
        /// <typeparam name="T">任意の型</typeparam>
        /// <typeparam name="TProperty">監視対象プロパティの型</typeparam>
        /// <param name="self">NotificationObject</param>
        /// <param name="command">ListenerCommand<T></param>
        /// <param name="execute">実行処理</param>
        /// <param name="canExecute">実行処理の実行有無</param>
        /// <param name="observesProperty">監視したいプロパティを返却するデリゲート</param>
        /// <returns>ListenerCommand<T></returns>
        public static ListenerCommand<T> SetCommand<T, TProperty>(this NotificationObject self, ref ListenerCommand<T> command, Action<T> execute, Func<bool> canExecute = null, Expression<Func<TProperty>> observesProperty = null)
        {
            if (command == null)
            {
                if (canExecute == null)
                    canExecute = () => true;
                
                command = new ListenerCommand<T>(execute, canExecute);

                if (observesProperty != null)
                    command.ObservesProperty(observesProperty);
            }

            return command;
        }

    }
}
