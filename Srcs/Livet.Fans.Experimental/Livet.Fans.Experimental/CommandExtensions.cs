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
 * Expression を扱う際の、インスタンス取得問題に対する解決策だった
 * Prism
 * PropertyObserver.cs
 * https://github.com/PrismLibrary/Prism/blob/master/Source/Prism/Commands/PropertyObserver.cs
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
            var member = propertyExpression.Body as MemberExpression;
            var item = GetVariousData(member);
            return ObservesPropertyInternal(self, item.Item1, item.Item2);
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

            var member = propertyExpression.Body as MemberExpression;
            var item = GetVariousData(member);
            return ObservesPropertyInternal(self, item.Item1, item.Item2);
        }

        /// <summary>
        /// Expression を元に、プロパティ名とプロパティをメンバーに持つクラスを返却します。
        /// </summary>
        /// <param name="member">MemberExpression</param>
        /// <returns>プロパティ名とプロパティをメンバーに持つクラスインスタンス</returns>
        /// <remarks>ReactiveProperty や階層が深い変更通知プロパティの場合、機能しないバグの対応</remarks>
        private static Tuple<INotifyPropertyChanged, string> GetVariousData(MemberExpression member)
        {
            // Expression を追って、プロパティ名と親クラス（INotifyPropertyChanged を持つもの）を取得する
            // () => PropertyName ・・・親(ViewModel)が INotifyPropertyChanged, 子がプロパティ
            // () => ReactiveProperty.Value ・・・親(ReactiveProperty 自身の INotifyPropertyChanged)プロパティ、子がプロパティ
            // () => GrandParent.Parent.Child.Value ・・・親(Child 自身の INotifyPropertyChanged)プロパティ、子がプロパティ
            // ※ GrandParent, Parent, Child はそれぞれ INotifyPropertyChanged を継承したクラスであり、プロパティ名でもある。という場合

            // どうやってプロパティからインスタンスを準備して INotifyPropertyChanged に変えるか？
            // Prism では、上へ上へ遡れるだけ遡って、ViewModel の INotifyPropertyChanged を使っているみたい？
            // それだと、ReactiveProperty に対応できない（ReactiveProperty 自身の INotifyPropertyChanged を取得できないとダメ）
            // → 一番上まで遡りインスタンスを取得する(ConstantExpression)、そのプロパティのインスタンスを取得する、を繰り返して戻ってくるのは？

            // 1. 最下層のプロパティを開始地点として、MemberExpression 型の間（ConstantExpression 型になるまで）上に遡るのを繰り返す（プロパティ→プロパティをメンバーに持つクラスがプロパティ、プロパティ・・・）、ボトムアップ
            // 2. その時プロパティ名を控えておく
            // 3. 一番上層（ViewModelCommand や渡されたプロパティをメンバーに持つ ViewModel の継承先クラス）まで行ったら、
            //    インスタンス取得を繰り返しながら戻れるところまで戻って来る、トップダウン

            var cacheStack = new Stack<string>();
            var propertyName = member.Member.Name;

            while (member.Expression is MemberExpression me)
            {
                member = me;
                cacheStack.Push(member.Member.Name);
            }

            if (!(member.Expression is ConstantExpression ce))
                throw new InvalidOperationException("() => プロパティ名、という指定をしてください。それ以外は未対応です。");

            // ここから戻れるところまで戻る、を繰り返す
            var instance = ce.Value;
            var notifier = default(INotifyPropertyChanged);
            if (cacheStack.Count == 0)
            {
                notifier = instance as INotifyPropertyChanged;
            }
            else
            {
                var propertyType = instance.GetType().GetProperty(cacheStack.Pop());
                var propertyInstance = propertyType.GetValue(instance);

                while (0 < cacheStack.Count)
                {
                    propertyType = propertyInstance.GetType().GetProperty(cacheStack.Pop());
                    propertyInstance = propertyType.GetValue(propertyInstance);
                }
                notifier = propertyInstance as INotifyPropertyChanged;
            }

            return Tuple.Create(notifier, propertyName);

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
            var member = propertyExpression.Body as MemberExpression;
            var item = GetVariousData(member);
            return ObservesPropertyInternal(self, item.Item1, item.Item2);
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

            var member = propertyExpression.Body as MemberExpression;
            var item = GetVariousData(member);
            return ObservesPropertyInternal(self, item.Item1, item.Item2);
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
