using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using Livet.Fans.Experimental.EventListeners.WeakEvents;

/* 
 * .NET - Get default value for a reflected PropertyInfo
 * https://stackoverflow.com/questions/407337/net-get-default-value-for-a-reflected-propertyinfo
 * default(T) のリフレクション版を取得する答え。イメージ: default(xxx.GetType())
 * 
 */

namespace Livet.Fans.Experimental
{
    /// <summary>
    /// WPF コントロールが持つイベントへの、直接バインディングを提供するクラスです。
    /// System.Windows.Data.Binding の機能も含めていますが、テスト段階です。
    /// </summary>
    public class BindingExtension : MarkupExtension
    {
        #region System.Windows.Data.Binding クラスから引用
        
        //
        // 概要:
        //     バインディング ソースを更新するタイミングを決定する値を取得または設定します。
        //
        // 戻り値:
        //     System.Windows.Data.UpdateSourceTrigger 値のいずれか。既定値は System.Windows.Data.UpdateSourceTrigger.Default
        //     で、これは対象となる依存関係プロパティの System.Windows.Data.UpdateSourceTrigger の既定値を返します。ただし、ほとんどの依存関係プロパティの既定値は
        //     System.Windows.Data.UpdateSourceTrigger.PropertyChanged であるのに対し、System.Windows.Controls.TextBox.Text
        //     プロパティの既定値は System.Windows.Data.UpdateSourceTrigger.LostFocus です。依存関係プロパティの既定の
        //     System.Windows.Data.Binding.UpdateSourceTrigger 値をプログラムで判断する方法の 1 つは、System.Windows.DependencyProperty.GetMetadata(System.Type)
        //     を使用してプロパティのプロパティ メタデータを取得してから、System.Windows.FrameworkPropertyMetadata.DefaultUpdateSourceTrigger
        //     プロパティの値をチェックすることです。
        [DefaultValue(UpdateSourceTrigger.Default)]
        public UpdateSourceTrigger UpdateSourceTrigger { get; set; }
        //
        // 概要:
        //     バインディング ターゲットからバインディング ソースへ値が転送されたときに System.Windows.Data.Binding.SourceUpdated
        //     イベントを発生させるかどうかを示す値を取得または設定します。
        //
        // 戻り値:
        //     バインディング ソースの値が更新されたときに System.Windows.Data.Binding.SourceUpdated イベントを発生させる必要がある場合は
        //     true。それ以外の場合は false。既定値は、false です。
        [DefaultValue(false)]
        public bool NotifyOnSourceUpdated { get; set; }
        //
        // 概要:
        //     バインディング ソースからバインディング ターゲットへ値が転送されたときに System.Windows.Data.Binding.TargetUpdated
        //     イベントを発生させるかどうかを示す値を取得または設定します。
        //
        // 戻り値:
        //     バインディング ターゲットの値が更新されたときに System.Windows.Data.Binding.TargetUpdated イベントを発生させる必要がある場合は
        //     true。それ以外の場合は false。既定値は、false です。
        [DefaultValue(false)]
        public bool NotifyOnTargetUpdated { get; set; }
        //
        // 概要:
        //     バインドされているオブジェクトで System.Windows.Controls.Validation.Error 添付イベントを発生させるかどうかを示す値を取得または設定します。
        //
        // 戻り値:
        //     ソースの更新時に検証エラーが見つかったときに、バインドされているオブジェクトに関する System.Windows.Controls.Validation.Error
        //     添付イベントを発生させる必要がある場合は true。それ以外の場合は false。既定値は、false です。
        [DefaultValue(false)]
        public bool NotifyOnValidationError { get; set; }
        //
        // 概要:
        //     使用するコンバーターを取得または設定します。
        //
        // 戻り値:
        //     System.Windows.Data.IValueConverter 型の値。既定値は、null です。
        [DefaultValue(null)]
        public IValueConverter Converter { get; set; }
        //
        // 概要:
        //     System.Windows.Data.Binding.Converter に渡すパラメーターを取得または設定します。
        //
        // 戻り値:
        //     System.Windows.Data.Binding.Converter に渡すパラメーター。既定値は、null です。
        [DefaultValue(null)]
        public object ConverterParameter { get; set; }
        //
        // 概要:
        //     コンバーターを評価する際に使用されるカルチャを取得または設定します。
        //
        // 戻り値:
        //     既定値は、null です。
        [DefaultValue(null)]
        [TypeConverter(typeof(CultureInfoIetfLanguageTagConverter))]
        public CultureInfo ConverterCulture { get; set; }
        //
        // 概要:
        //     バインディング ソース オブジェクトとして使用するオブジェクトを取得または設定します。
        //
        // 戻り値:
        //     バインディング ソース オブジェクトとして使用するオブジェクト。
        public object Source { get; set; }
        //
        // 概要:
        //     バインディング ターゲットの位置に対して相対的な位置を指定することにより、バインディング ソースを取得または設定します。
        //
        // 戻り値:
        //     使用するバインディング ソースの相対的な位置を指定する System.Windows.Data.RelativeSource オブジェクト。既定値は、null
        //     です。
        [DefaultValue(null)]
        public RelativeSource RelativeSource { get; set; }
        //
        // 概要:
        //     バインディング ソース オブジェクトとして使用する要素の名前を取得または設定します。
        //
        // 戻り値:
        //     目的の要素の Name プロパティまたは x:Name ディレクティブ の値。コード内で要素を参照できるのは、目的の要素が RegisterName を使用して適切な
        //     System.Windows.NameScope に登録されている場合に限ります。詳細については、「WPF XAML 名前スコープ」を参照してください。既定値は、null
        //     です。
        [DefaultValue(null)]
        public string ElementName { get; set; }
        //
        // 概要:
        //     System.Windows.Data.Binding が非同期的に値を取得および設定する必要があるかどうかを示す値を取得または設定します。
        //
        // 戻り値:
        //     既定値は、null です。
        [DefaultValue(false)]
        public bool IsAsync { get; set; }
        //
        // 概要:
        //     非同期データ ディスパッチャーに渡される非透過データを取得または設定します。
        //
        // 戻り値:
        //     非同期データ ディスパッチャーに渡されるデータ。
        [DefaultValue(null)]
        public object AsyncState { get; set; }
        //
        // 概要:
        //     バインディングのデータ フローの方向を示す値を取得または設定します。
        //
        // 戻り値:
        //     System.Windows.Data.BindingMode 値のいずれか。既定値は System.Windows.Data.BindingMode.Default
        //     で、これは対象となる依存関係プロパティの既定のバインディング モード値を返します。ただし、既定値は、各依存関係プロパティによって異なります。一般に、ユーザーが編集できる、テキスト
        //     ボックスやチェック ボックスなどのコントロール プロパティは既定で双方向のバインディングであり、それ以外のほとんどのプロパティは既定で一方向のバインドになります。依存関係プロパティが既定で一方向または双方向のいずれであるかをプログラムにより判断する
        //     1 つの方法は、System.Windows.DependencyProperty.GetMetadata(System.Type) を使用してプロパティのプロパティ
        //     メタデータを取得してから、System.Windows.FrameworkPropertyMetadata.BindsTwoWayByDefault プロパティのブール値を確認することです。
        [DefaultValue(BindingMode.Default)]
        public BindingMode Mode { get; set; }
        //
        // 概要:
        //     使用する XML バインディング ソースの値を返す XPath クエリを取得または設定します。
        //
        // 戻り値:
        //     XPath クエリ。既定値は、null です。
        [DefaultValue(null)]
        public string XPath { get; set; }
        //
        // 概要:
        //     System.Windows.Controls.DataErrorValidationRule を含めるかどうかを示す値を取得または設定します。
        //
        // 戻り値:
        //     System.Windows.Controls.DataErrorValidationRule を含める場合は true。それ以外の場合は false。
        [DefaultValue(false)]
        public bool ValidatesOnDataErrors { get; set; }
        //
        // 概要:
        //     System.Windows.Controls.NotifyDataErrorValidationRule を含めるかどうかを示す値を取得または設定します。
        //
        // 戻り値:
        //     System.Windows.Controls.NotifyDataErrorValidationRule を含める場合は true。それ以外の場合は false。既定値は、true
        //     です。
        [DefaultValue(true)]
        public bool ValidatesOnNotifyDataErrors { get; set; }
        //
        // 概要:
        //     System.Windows.Data.Binding.Path を評価するときに、データ項目を基準とするか、System.Windows.Data.DataSourceProvider
        //     オブジェクトを基準とするかを示す値を取得または設定します。
        //
        // 戻り値:
        //     データ項目自体を基準としてパスを評価する場合は false。それ以外の場合は true。既定値は、false です。
        [DefaultValue(false)]
        public bool BindsDirectlyToSource { get; set; }
        //
        // 概要:
        //     System.Windows.Controls.ExceptionValidationRule を含めるかどうかを示す値を取得または設定します。
        //
        // 戻り値:
        //     System.Windows.Controls.ExceptionValidationRule を含める場合は true。それ以外の場合は false。
        [DefaultValue(false)]
        public bool ValidatesOnExceptions { get; set; }
        //
        // 概要:
        //     ユーザー入力の有効性を確認する規則のコレクションを取得します。
        //
        // 戻り値:
        //     System.Windows.Controls.ValidationRule オブジェクトのコレクション。
        //public Collection<ValidationRule> ValidationRules { get; }
        //
        // 概要:
        //     バインディング ソース プロパティへのパスを取得または設定します。
        //
        // 戻り値:
        //     バインディング ソースへのパス。既定値は、null です。
        [ConstructorArgument("path")]
        public PropertyPath Path { get; set; }
        //
        // 概要:
        //     バインディング ソースの値の更新時にバインディング エンジンが検出した例外を処理するためのカスタム ロジックを提供するハンドラーを取得または設定します。これは、バインディングに
        //     System.Windows.Controls.ExceptionValidationRule を関連付けている場合にのみ適用できます。
        //
        // 戻り値:
        //     バインディング ソースの値の更新時にバインディング エンジンが検出した例外を処理するためのカスタム ロジックを提供するメソッド。
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public UpdateSourceExceptionFilterCallback UpdateSourceExceptionFilter { get; set; }

        #endregion

        // イベント引数を受け取るかどうか
        [DefaultValue(false)]
        public bool UseEventArgs { get; set; }

        // BindingICommandResolver クラス内で発生した LivetWeakEventListener の扱いミスによるバグの類似修正。生存期間を長引かせるためクラス変数にセットしておく
        private LivetWeakEventListener<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs> _Listener;

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="path">バインド先メンバーのパスがセットされたパス情報</param>
        public BindingExtension(PropertyPath path)
        {
            this.Path = path;
        }

        /// <summary>
        /// バインド先の WPF コントロールメンバーに合わせて、対応する値を返却します。
        /// </summary>
        /// <param name="serviceProvider">WPF コントロール情報</param>
        /// <returns>対応する値</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var provider = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            if (provider == null)
                return null;

            var item = new BindingControlObject() { Path = this.Path, WpfMember = provider.TargetProperty };
            item.SetWpfControl(provider.TargetObject);
            
            // DataContext 自体が変更された場合は、追従して更新する
            var selector = item.WpfControl.Item1;
            if (selector == 0)
            {
                var element = item.WpfControl.Item2;
                var listener = new LivetWeakEventListener<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(
                    h => new DependencyPropertyChangedEventHandler(h),
                    h => element.DataContextChanged += h,
                    h => element.DataContextChanged -= h,
                    (s, e) => element_DataContextChanged(item, e));

                _Listener = listener;

            }
            else
            {
                var element = item.WpfControl.Item3;
                var listener = new LivetWeakEventListener<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(
                    h => new DependencyPropertyChangedEventHandler(h),
                    h => element.DataContextChanged += h,
                    h => element.DataContextChanged -= h,
                    (s, e) => element_DataContextChanged(item, e));

                _Listener = listener;

            }

            return ProvideValueInternal(item);
            
        }

        /// <summary>
        /// DataContext が更新されたときのイベントです。
        /// </summary>
        /// <param name="item">作業用データクラス</param>
        /// <param name="e">新しいデータコンテキスト</param>
        /// <remarks>
        /// DataContext をバインドしているのに、コントロールから DataContext が取得できないバグの対応。ProvideValue() の準備完了後に、後から DataContext がセットされる場合の対応。
        /// 任意のタイミングでソースコード経由で更新された場合の他、以下のように、内側で DataContext をセットする記述の場合にも DataContextChanged イベントが発生します。
        /// ＜Window {livetfans:Binding IsEnabled}＞
        ///     ＜Window.DataContext＞
        ///         ＜local:MainWindowViewModel /＞
        ///     ＜/Window.DataContext＞
        /// ＜/Window＞
        /// ※XML コメント内のため、全角文字のタグで囲って書いています。
        /// </remarks>
        private void element_DataContextChanged(BindingControlObject item, DependencyPropertyChangedEventArgs e)
        {
            // DataContext(=ViewModel) と バインドしている ViewModel のメンバーを再取得
            item.WpfDataContext = e.NewValue;
            item.GetViewModelMember();
            
            // 再度、コントロールのメンバーに値をセット
            var returnValue = ProvideValueInternal(item);
            item.WpfMember = returnValue;            
        }

        /// <summary>
        /// 渡された情報を元に、対応する値を返却します。
        /// </summary>
        /// <param name="item">作業用データクラス</param>
        /// <returns>対応する値</returns>
        private object ProvideValueInternal(BindingControlObject item)
        {
            var resolver = default(IBindingResolver);
            var wpfMemberType = item.WpfMember.GetType();

            // WPF コントロールのメンバーがイベント、またはメソッドかどうか
            if (typeof(EventInfo).IsAssignableFrom(wpfMemberType) || typeof(MethodInfo).IsAssignableFrom(wpfMemberType))
            {
                // ViewModel がバインドされていない場合、空のデリゲートを返却する
                if (item.ViewModel == null)
                {
                    return new RoutedEventHandler((s, e) => { });
                }

                var vmMemberType = item.ViewModelMember?.GetType();
                if (typeof(ICommand).IsAssignableFrom(vmMemberType))
                {
                    // コマンドをバインドしている
                    resolver = new BindingICommandResolver();
                }
                else
                {
                    // メソッドをバインドしている
                    resolver = new BindingMethodResolver();
                }
            }
            else
            {
                // その他のプロパティをバインドしている
                // ViewModel がバインドされていない場合、デフォルト値を返却する
                if (item.ViewModel == null)
                {
                    var defaultValue = wpfMemberType.IsValueType ? Activator.CreateInstance(wpfMemberType) : null;
                    return defaultValue;
                }

                // ReactiveProperty をバインドしている場合、Value プロパティをバインドするように調整する
                if (item.ViewModelMember != null && item.ViewModelMember.GetType().FullName.StartsWith("Reactive.Bindings.ReactiveProperty"))
                {
                    this.Path.Path += ".Value";
                    var repair = new BindingControlObject() { Path = this.Path, WpfMember = item.WpfMember };
                    repair.SetWpfControl(item.GetWpfControl());
                    item = repair;
                }

                resolver = new BindingPropertyResolver();

            }

            // 主に BindingPropertyResolver で使うものが大半だが、各オプション情報を渡す
            var option = new BindingOptionObject()
            {
                UpdateSourceTrigger = this.UpdateSourceTrigger,
                NotifyOnSourceUpdated = this.NotifyOnSourceUpdated,
                NotifyOnTargetUpdated = this.NotifyOnTargetUpdated,
                NotifyOnValidationError = this.NotifyOnValidationError,
                Converter = this.Converter,
                ConverterParameter = this.ConverterParameter,
                ConverterCulture = this.ConverterCulture,
                Source = this.Source,
                RelativeSource = this.RelativeSource,
                ElementName = this.ElementName,
                IsAsync = this.IsAsync,
                AsyncState = this.AsyncState,
                Mode = this.Mode,
                XPath = this.XPath,
                ValidatesOnDataErrors = this.ValidatesOnDataErrors,
                ValidatesOnNotifyDataErrors = this.ValidatesOnNotifyDataErrors,
                BindsDirectlyToSource = this.BindsDirectlyToSource,
                ValidatesOnExceptions = this.ValidatesOnExceptions,
                // ValidationRules = this.ValidationRules,
                Path = this.Path,
                UpdateSourceExceptionFilter = this.UpdateSourceExceptionFilter,
                UseEventArgs = this.UseEventArgs
            };

            return resolver?.GetReturnValue(item, option);
            
        }

    }
}
