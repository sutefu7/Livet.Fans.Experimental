using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Livet;

/*
 * 以下の移植です。
 * Prism
 * BindableBase.cs
 * https://github.com/PrismLibrary/Prism/blob/master/Source/Prism/Mvvm/BindableBase.cs
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

    }
}
