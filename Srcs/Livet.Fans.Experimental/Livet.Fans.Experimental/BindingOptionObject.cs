using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Livet.Fans.Experimental
{
    /// <summary>
    /// View 側で指定されたオプション情報を保持するデータクラスです。
    /// </summary>
    class BindingOptionObject
    {
        // 共通
        // System.Windows.Data.Binding クラスから引用
        public UpdateSourceTrigger UpdateSourceTrigger { get; set; }
        public bool NotifyOnSourceUpdated { get; set; }
        public bool NotifyOnTargetUpdated { get; set; }
        public bool NotifyOnValidationError { get; set; }
        public IValueConverter Converter { get; set; }
        public object ConverterParameter { get; set; }
        public CultureInfo ConverterCulture { get; set; }
        public object Source { get; set; }
        public RelativeSource RelativeSource { get; set; }
        public string ElementName { get; set; }
        public bool IsAsync { get; set; }
        public object AsyncState { get; set; }
        public BindingMode Mode { get; set; }
        public string XPath { get; set; }
        public bool ValidatesOnDataErrors { get; set; }
        public bool ValidatesOnNotifyDataErrors { get; set; }
        public bool BindsDirectlyToSource { get; set; }
        public bool ValidatesOnExceptions { get; set; }
        //public Collection<ValidationRule> ValidationRules { get; }
        public PropertyPath Path { get; set; }
        public UpdateSourceExceptionFilterCallback UpdateSourceExceptionFilter { get; set; }

        // コマンド、メソッド共通
        public bool UseEventArgs { get; set; }

    }
}
