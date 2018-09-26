using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

/*
 * 【WPF】最小のBindingExtensionを実装
 * http://pro.art55.jp/?eid=1068546
 * 
 */

namespace Livet.Fans.Experimental
{
    /// <summary>
    /// バインド先メンバーが任意のプロパティの場合の、戻り値を返却するクラスです。
    /// </summary>
    class BindingPropertyResolver : IBindingResolver
    {
        /// <summary>
        /// 戻り値を返却します。
        /// </summary>
        /// <param name="controlInfo">View と ViewModel の関連情報</param>
        /// <param name="option">View のオプション指定情報</param>
        /// <returns>戻り値</returns>
        public object GetReturnValue(BindingControlObject controlInfo, BindingOptionObject option)
        {
            var binding = new Binding()
            {
                UpdateSourceTrigger = option.UpdateSourceTrigger,
                NotifyOnSourceUpdated = option.NotifyOnSourceUpdated,
                NotifyOnTargetUpdated = option.NotifyOnTargetUpdated,
                NotifyOnValidationError = option.NotifyOnValidationError,
                Converter = option.Converter,
                ConverterParameter = option.ConverterParameter,
                ConverterCulture = option.ConverterCulture,
                //Source = option.Source,    // Source, RelativeSource は排他的関係なので同時セットができないため
                //RelativeSource = option.RelativeSource,
                ElementName = option.ElementName,
                IsAsync = option.IsAsync,
                AsyncState = option.AsyncState,
                Mode = option.Mode,
                XPath = option.XPath,
                ValidatesOnDataErrors = option.ValidatesOnDataErrors,
                ValidatesOnNotifyDataErrors = option.ValidatesOnNotifyDataErrors,
                BindsDirectlyToSource = option.BindsDirectlyToSource,
                ValidatesOnExceptions = option.ValidatesOnExceptions,
                // ValidationRules = option.ValidationRules,     get のみのため
                Path = option.Path,
                UpdateSourceExceptionFilter = option.UpdateSourceExceptionFilter
            };

            if (option.Source != null)
            {
                binding.Source = option.Source;
            }

            if (binding.Source == null && option.RelativeSource != null)
            {
                binding.RelativeSource = option.RelativeSource;
            }

            var selector = controlInfo.WpfControl.Item1;
            if (selector == 0)
            {
                var element = controlInfo.WpfControl.Item2;
                var member = controlInfo.WpfMember as DependencyProperty;
                BindingOperations.SetBinding(element, member, binding);
                return element.GetValue(member);
            }
            else
            {
                var element = controlInfo.WpfControl.Item3;
                var member = controlInfo.WpfMember as DependencyProperty;
                BindingOperations.SetBinding(element, member, binding);
                return element.GetValue(member);
            }

        }

    }
}
