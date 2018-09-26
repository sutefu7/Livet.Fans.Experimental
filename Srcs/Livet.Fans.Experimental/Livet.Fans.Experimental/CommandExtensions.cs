using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Livet;
using Livet.Commands;
using Livet.EventListeners.WeakEvents;

/*
 * 以下の移植です。
 * Prism
 * DelegateCommand.cs
 * https://github.com/PrismLibrary/Prism/blob/master/Source/Prism/Commands/DelegateCommand.cs
 * 
 * [C#][ラムダ式][式木] Expression を使ってラムダ式のメンバー名を取得する
 * http://blog.shos.info/archives/2012/12/cexpression_expression_2.html
 * 
 */


namespace Livet
{
    /// <summary>
    /// ViewModelCommand, ListenerCommand に対する拡張クラスです。
    /// </summary>
    public static class CommandExtensions
    {
        private static Dictionary<string, LivetWeakEventListener<PropertyChangedEventHandler, PropertyChangedEventArgs>> _Cache = null;

        #region ViewModelCommand

        /// <summary>
        /// 指定したプロパティを監視します。状態変更時に追従して、コマンドの活性状態を切り替えます。
        /// </summary>
        /// <typeparam name="TProperty">プロパティの型</typeparam>
        /// <param name="self">ViewModelCommand</param>
        /// <param name="propertyExpression">指定したプロパティ</param>
        /// <returns>ViewModelCommand</returns>
        public static ViewModelCommand ObservesProperty<TProperty>(this ViewModelCommand self, Expression<Func<TProperty>> propertyExpression)
        {
            // Expression を追って、プロパティ名と親クラス（INotifyPropertyChanged を持つもの）を取得する
            var propertyName = ((MemberExpression)propertyExpression.Body).Member.Name;
            var notifier = (INotifyPropertyChanged)((ConstantExpression)((MemberExpression)propertyExpression.Body).Expression).Value;
            return ObservesPropertyInternal(self, notifier, propertyName);
        }

        /// <summary>
        /// 指定したプロパティを状態変更処理（CanExecute）として取得、さらに監視します。状態変更時に追従して、コマンドの活性状態を切り替えます。
        /// </summary>
        /// <param name="self">ViewModelCommand</param>
        /// <param name="propertyExpression">bool 型のプロパティ</param>
        /// <returns>ViewModelCommand</returns>
        public static ViewModelCommand ObservesCanExecute(this ViewModelCommand self, Expression<Func<bool>> propertyExpression)
        {
            // （注意）ViewModelCommand の private フィールド: _canExecute は内部変数のため、仕様変更の可能性あり
            var canExecute = propertyExpression.Compile();
            var _canExecuteType= self.GetType().GetField("_canExecute", BindingFlags.NonPublic | BindingFlags.Instance);
            _canExecuteType.SetValue(self, canExecute);

            var propertyName = ((MemberExpression)propertyExpression.Body).Member.Name;
            var notifier = (INotifyPropertyChanged)((ConstantExpression)((MemberExpression)propertyExpression.Body).Expression).Value;
            return ObservesPropertyInternal(self, notifier, propertyName);
        }

        /// <summary>
        /// 指定されたプロパティの PropertyChanged イベント発生を受けて、コマンドの RaiseCanExecuteChanged() を実行します。
        /// </summary>
        /// <param name="self">ViewModelCommand</param>
        /// <param name="notifier">INotifyPropertyChanged 型の ViewModel or Model</param>
        /// <param name="propertyName">値が変更されたプロパティ名</param>
        /// <returns>ViewModelCommand</returns>
        private static ViewModelCommand ObservesPropertyInternal(ViewModelCommand self, INotifyPropertyChanged notifier, string propertyName)
        {
            if (_Cache == null)
                _Cache = new Dictionary<string, LivetWeakEventListener<PropertyChangedEventHandler, PropertyChangedEventArgs>>();
            
            // 唯一のコマンド、唯一のViewModel or Model, 唯一のプロパティ、の粒度に分けて登録して使う
            var key = $"{self.GetType().FullName}.{notifier.GetType().FullName}.{propertyName}";
            if (!_Cache.ContainsKey(key))
            {
                var listener = new LivetWeakEventListener<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    h => new PropertyChangedEventHandler(h),
                    h => notifier.PropertyChanged += h,
                    h => notifier.PropertyChanged -= h,
                    (s, e) =>
                    {
                        if (e.PropertyName == propertyName)
                            self.RaiseCanExecuteChanged();
                    });

                _Cache.Add(key, listener);
            }

            return self;

        }

        #endregion

        #region ListenerCommand<T>

        /// <summary>
        /// 指定したプロパティを監視します。状態変更時に追従して、コマンドの活性状態を切り替えます。
        /// </summary>
        /// <typeparam name="T">コマンドのジェネリック型</typeparam>
        /// <typeparam name="TProperty">プロパティの型</typeparam>
        /// <param name="self">ListenerCommand<T></param>
        /// <param name="propertyExpression">指定したプロパティ</param>
        /// <returns>ListenerCommand<T></returns>
        public static ListenerCommand<T> ObservesProperty<T, TProperty>(this ListenerCommand<T> self, Expression<Func<TProperty>> propertyExpression)
        {
            // Expression を追って、プロパティ名と親クラス（INotifyPropertyChanged を持つもの）を取得する
            var propertyName = ((MemberExpression)propertyExpression.Body).Member.Name;
            var notifier = (INotifyPropertyChanged)((ConstantExpression)((MemberExpression)propertyExpression.Body).Expression).Value;
            return ObservesPropertyInternal(self, notifier, propertyName);
        }

        /// <summary>
        /// 指定したプロパティを状態変更処理（CanExecute）として取得、さらに監視します。状態変更時に追従して、コマンドの活性状態を切り替えます。
        /// </summary>
        /// <typeparam name="T">コマンドのジェネリック型</typeparam>
        /// <param name="self">ListenerCommand<T></param>
        /// <param name="propertyExpression">bool 型のプロパティ</param>
        /// <returns>ListenerCommand<T></returns>
        public static ListenerCommand<T> ObservesCanExecute<T>(this ListenerCommand<T> self, Expression<Func<bool>> propertyExpression)
        {
            // （注意）ViewModelCommand の private フィールド: _canExecute は内部変数のため、仕様変更の可能性あり
            var canExecute = propertyExpression.Compile();
            var _canExecuteType = self.GetType().GetField("_canExecute", BindingFlags.NonPublic | BindingFlags.Instance);
            _canExecuteType.SetValue(self, canExecute);

            var propertyName = ((MemberExpression)propertyExpression.Body).Member.Name;
            var notifier = (INotifyPropertyChanged)((ConstantExpression)((MemberExpression)propertyExpression.Body).Expression).Value;
            return ObservesPropertyInternal(self, notifier, propertyName);
        }

        /// <summary>
        /// 指定されたプロパティの PropertyChanged イベント発生を受けて、コマンドの RaiseCanExecuteChanged() を実行します。
        /// </summary>
        /// <typeparam name="T">コマンドのジェネリック型</typeparam>
        /// <param name="self">ListenerCommand<T></param>
        /// <param name="notifier">INotifyPropertyChanged 型の ViewModel or Model</param>
        /// <param name="propertyName">値が変更されたプロパティ名</param>
        /// <returns>ListenerCommand<T></returns>
        private static ListenerCommand<T> ObservesPropertyInternal<T>(ListenerCommand<T> self, INotifyPropertyChanged notifier, string propertyName)
        {
            if (_Cache == null)
                _Cache = new Dictionary<string, LivetWeakEventListener<PropertyChangedEventHandler, PropertyChangedEventArgs>>();

            // 唯一のコマンド、唯一のViewModel or Model, 唯一のプロパティ、の粒度に分けて登録して使う
            var key = $"{self.GetType().FullName}.{notifier.GetType().FullName}.{propertyName}";
            if (!_Cache.ContainsKey(key))
            {
                var listener = new LivetWeakEventListener<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    h => new PropertyChangedEventHandler(h),
                    h => notifier.PropertyChanged += h,
                    h => notifier.PropertyChanged -= h,
                    (s, e) =>
                    {
                        if (e.PropertyName == propertyName)
                            self.RaiseCanExecuteChanged();
                    });

                _Cache.Add(key, listener);
            }

            return self;

        }

        #endregion


    }
}
