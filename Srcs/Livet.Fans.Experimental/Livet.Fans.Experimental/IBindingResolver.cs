using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Livet.Fans.Experimental
{
    /// <summary>
    /// WPF コントロールが持つメンバーに対して、適切な戻り値や、関連処理をおこなうための共通インターフェースです。
    /// 実際の処理は、各継承先クラスを参照してください。
    /// </summary>
    interface IBindingResolver
    {
        object GetReturnValue(BindingControlObject controlInfo, BindingOptionObject option);
    }
}
