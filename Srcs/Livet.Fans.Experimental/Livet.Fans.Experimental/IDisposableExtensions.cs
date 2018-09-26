using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Livet;

/*
 * 以下の移植です。
 * ReactiveProperty
 * IDisposableExtensions.cs
 * https://github.com/runceel/ReactiveProperty/blob/master/Source/ReactiveProperty.NETStandard/Extensions/IDisposableExtensions.cs
 * 
 * ただし、ViewModel 内限定仕様として、使える範囲を縮小しています。
 * これは、LivetCompositeDisposable を含んでいる ViewModel への、スムーズな登録を想定しているためです。
 * また、ReactiveProperty の Reactive.Bindings.Extensions.AddTo() を同時利用した際の、コンフリクトによる名前解決の妨げにさせないためでもあります。
 * 後者は後付けの言い訳です。
 * 
 */


namespace Livet
{
    /// <summary>
    /// IDisposable に対する拡張クラスです。
    /// </summary>
    public static class IDisposableExtensions
    {
        public static T AddTo<T>(this T disposable, ViewModel container) where T : IDisposable
        {
            container.CompositeDisposable.Add(disposable);
            return disposable;
        }
    }
}
