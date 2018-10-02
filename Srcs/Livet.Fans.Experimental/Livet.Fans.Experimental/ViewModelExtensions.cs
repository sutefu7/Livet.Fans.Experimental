using Livet.Messaging;
using Livet.Messaging.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Livet
{
    /// <summary>
    /// ViewModel に対する拡張クラスです。
    /// </summary>
    public static class ViewModelExtensions
    {
        /// <summary>
        /// 情報メッセージを表示します。
        /// </summary>
        /// <param name="self">ViewModel</param>
        /// <param name="messageBoxText">メッセージ本文</param>
        /// <param name="title">タイトル</param>
        /// <param name="messageKey">メッセージキー</param>
        public static void ShowInformationMessage(this ViewModel self, string messageBoxText, string title = "情報", string messageKey = "ShowInformationMessage") =>
            ShowTargetMessage(self, messageBoxText, title, MessageBoxImage.Information, messageKey);

        /// <summary>
        /// 警告メッセージを表示します。
        /// </summary>
        /// <param name="self">ViewModel</param>
        /// <param name="messageBoxText">メッセージ本文</param>
        /// <param name="title">タイトル</param>
        /// <param name="messageKey">メッセージキー</param>
        public static void ShowWarningMessage(this ViewModel self, string messageBoxText, string title = "警告", string messageKey = "ShowWarningMessage") =>
            ShowTargetMessage(self, messageBoxText, title, MessageBoxImage.Warning, messageKey);

        /// <summary>
        /// エラーメッセージを表示します。
        /// </summary>
        /// <param name="self">ViewModel</param>
        /// <param name="messageBoxText">メッセージ本文</param>
        /// <param name="title">タイトル</param>
        /// <param name="messageKey">メッセージキー</param>
        public static void ShowErrorMessage(this ViewModel self, string messageBoxText, string title = "エラー", string messageKey = "ShowErrorMessage") =>
            ShowTargetMessage(self, messageBoxText, title, MessageBoxImage.Error, messageKey);

        /// <summary>
        /// 任意の種類のメッセージを表示します。
        /// </summary>
        /// <param name="self">ViewModel</param>
        /// <param name="messageBoxText">メッセージ本文</param>
        /// <param name="title">タイトル</param>
        /// <param name="image">メッセージの種類別アイコン</param>
        /// <param name="messageKey">メッセージキー</param>
        private static void ShowTargetMessage(ViewModel self, string messageBoxText, string title, MessageBoxImage image, string messageKey)
        {
            var mes = new InformationMessage(messageBoxText, title, image, messageKey);
            self.Messenger.Raise(mes);
        }




        /// <summary>
        /// OK, Cancel 形式の確認メッセージを表示します。
        /// </summary>
        /// <param name="self">ViewModel</param>
        /// <param name="messageBoxText">メッセージ本文</param>
        /// <param name="title">タイトル</param>
        /// <param name="messageKey">メッセージキー</param>
        /// <param name="defaultResult">初期フォーカスをあてるボタン</param>
        /// <returns>結果</returns>
        public static ConfirmationMessage ShowConfirmationOKCancelMessage(this ViewModel self, string messageBoxText, string title = "確認", string messageKey = "ShowConfirmationOKCancelMessage", MessageBoxResult defaultResult = MessageBoxResult.OK) =>
            ShowConfirmationTargetMessage(self, messageBoxText, title, MessageBoxButton.OKCancel, defaultResult, messageKey);

        /// <summary>
        /// Yes, No 形式の確認メッセージを表示します。
        /// </summary>
        /// <param name="self">ViewModel</param>
        /// <param name="messageBoxText">メッセージ本文</param>
        /// <param name="title">タイトル</param>
        /// <param name="messageKey">メッセージキー</param>
        /// <param name="defaultResult">初期フォーカスをあてるボタン</param>
        /// <returns>結果</returns>
        public static ConfirmationMessage ShowConfirmationYesNoMessage(this ViewModel self, string messageBoxText, string title = "確認", string messageKey = "ShowConfirmationYesNoMessage", MessageBoxResult defaultResult = MessageBoxResult.Yes) =>
            ShowConfirmationTargetMessage(self, messageBoxText, title, MessageBoxButton.YesNo, defaultResult, messageKey);

        /// <summary>
        /// Yes, No, Cancel 形式の確認メッセージを表示します。
        /// </summary>
        /// <param name="self">ViewModel</param>
        /// <param name="messageBoxText">メッセージ本文</param>
        /// <param name="title">タイトル</param>
        /// <param name="messageKey">メッセージキー</param>
        /// <param name="defaultResult">初期フォーカスをあてるボタン</param>
        /// <returns>結果</returns>
        public static ConfirmationMessage ShowConfirmationYesNoCancelMessage(this ViewModel self, string messageBoxText, string title = "確認", string messageKey = "ShowConfirmationYesNoCancelMessage", MessageBoxResult defaultResult = MessageBoxResult.Yes) =>
            ShowConfirmationTargetMessage(self, messageBoxText, title, MessageBoxButton.YesNoCancel, defaultResult, messageKey);

        /// <summary>
        /// 任意の形式の確認メッセージを表示します。
        /// </summary>
        /// <param name="self">ViewModel</param>
        /// <param name="messageBoxText">メッセージ本文</param>
        /// <param name="title">タイトル</param>
        /// <param name="buttonKinds">表示するボタンの種類</param>
        /// <param name="defaultResult">初期フォーカスをあてるボタン</param>
        /// <param name="messageKey">メッセージキー</param>
        /// <returns>結果</returns>
        private static ConfirmationMessage ShowConfirmationTargetMessage(ViewModel self, string messageBoxText, string title, MessageBoxButton buttonKinds, MessageBoxResult defaultResult, string messageKey)
        {
            var mes = new ConfirmationMessage(messageBoxText, title, MessageBoxImage.Question, buttonKinds, defaultResult, messageKey);
            return self.Messenger.GetResponse(mes);
        }




        /// <summary>
        /// 情報メッセージを非同期で表示します。
        /// </summary>
        /// <param name="self">ViewModel</param>
        /// <param name="messageBoxText">メッセージ本文</param>
        /// <param name="title">タイトル</param>
        /// <param name="messageKey">メッセージキー</param>
        /// <returns></returns>
        public static async Task ShowInformationMessageAsync(this ViewModel self, string messageBoxText, string title = "情報", string messageKey = "ShowInformationMessageAsync") =>
            await ShowTargetMessageAsync(self, messageBoxText, title, MessageBoxImage.Information, messageKey);

        /// <summary>
        /// 警告メッセージを非同期で表示します。
        /// </summary>
        /// <param name="self">ViewModel</param>
        /// <param name="messageBoxText">メッセージ本文</param>
        /// <param name="title">タイトル</param>
        /// <param name="messageKey">メッセージキー</param>
        /// <returns></returns>
        public static async Task ShowWarningMessageAsync(this ViewModel self, string messageBoxText, string title = "警告", string messageKey = "ShowWarningMessageAsync") =>
            await ShowTargetMessageAsync(self, messageBoxText, title, MessageBoxImage.Warning, messageKey);

        /// <summary>
        /// エラーメッセージを非同期で表示します。
        /// </summary>
        /// <param name="self">ViewModel</param>
        /// <param name="messageBoxText">メッセージ本文</param>
        /// <param name="title">タイトル</param>
        /// <param name="messageKey">メッセージキー</param>
        /// <returns></returns>
        public static async Task ShowErrorMessageAsync(this ViewModel self, string messageBoxText, string title = "エラー", string messageKey = "ShowErrorMessageAsync") =>
            await ShowTargetMessageAsync(self, messageBoxText, title, MessageBoxImage.Error, messageKey);

        /// <summary>
        /// 任意の種類のメッセージを非同期で表示します。
        /// </summary>
        /// <param name="self">ViewModel</param>
        /// <param name="messageBoxText">メッセージ本文</param>
        /// <param name="title">タイトル</param>
        /// <param name="image">メッセージの種類別アイコン</param>
        /// <param name="messageKey">メッセージキー</param>
        /// <returns></returns>
        private static async Task ShowTargetMessageAsync(ViewModel self, string messageBoxText, string title, MessageBoxImage image, string messageKey)
        {
            var mes = new InformationMessage(messageBoxText, title, image, messageKey);
            await self.Messenger.RaiseAsync(mes);
        }




        /// <summary>
        /// OK, Cancel 形式の確認メッセージを非同期で表示します。
        /// </summary>
        /// <param name="self">ViewModel</param>
        /// <param name="messageBoxText">メッセージ本文</param>
        /// <param name="title">タイトル</param>
        /// <param name="messageKey">メッセージキー</param>
        /// <param name="defaultResult">初期フォーカスをあてるボタン</param>
        /// <returns>結果</returns>
        public async static Task<ConfirmationMessage> ShowConfirmationOKCancelMessageAsync(this ViewModel self, string messageBoxText, string title = "確認", string messageKey = "ShowConfirmationOKCancelMessageAsync", MessageBoxResult defaultResult = MessageBoxResult.OK) =>
            await ShowConfirmationTargetMessageAsync(self, messageBoxText, title, MessageBoxButton.OKCancel, defaultResult, messageKey);

        /// <summary>
        /// Yes, No 形式の確認メッセージを非同期で表示します。
        /// </summary>
        /// <param name="self">ViewModel</param>
        /// <param name="messageBoxText">メッセージ本文</param>
        /// <param name="title">タイトル</param>
        /// <param name="messageKey">メッセージキー</param>
        /// <param name="defaultResult">初期フォーカスをあてるボタン</param>
        /// <returns>結果</returns>
        public async static Task<ConfirmationMessage> ShowConfirmationYesNoMessageAsync(this ViewModel self, string messageBoxText, string title = "確認", string messageKey = "ShowConfirmationYesNoMessageAsync", MessageBoxResult defaultResult = MessageBoxResult.Yes) =>
            await ShowConfirmationTargetMessageAsync(self, messageBoxText, title, MessageBoxButton.YesNo, defaultResult, messageKey);

        /// <summary>
        /// Yes, No, Cancel 形式の確認メッセージを非同期で表示します。
        /// </summary>
        /// <param name="self">ViewModel</param>
        /// <param name="messageBoxText">メッセージ本文</param>
        /// <param name="title">タイトル</param>
        /// <param name="messageKey">メッセージキー</param>
        /// <param name="defaultResult">初期フォーカスをあてるボタン</param>
        /// <returns>結果</returns>
        public async static Task<ConfirmationMessage> ShowConfirmationYesNoCancelMessageAsync(this ViewModel self, string messageBoxText, string title = "確認", string messageKey = "ShowConfirmationYesNoCancelMessageAsync", MessageBoxResult defaultResult = MessageBoxResult.Yes) =>
            await ShowConfirmationTargetMessageAsync(self, messageBoxText, title, MessageBoxButton.YesNoCancel, defaultResult, messageKey);

        /// <summary>
        /// 任意の形式の確認メッセージを非同期で表示します。
        /// </summary>
        /// <param name="self">ViewModel</param>
        /// <param name="messageBoxText">メッセージ本文</param>
        /// <param name="title">タイトル</param>
        /// <param name="buttonKinds">表示するボタンの種類</param>
        /// <param name="defaultResult">初期フォーカスをあてるボタン</param>
        /// <param name="messageKey">メッセージキー</param>
        /// <returns>結果</returns>
        private async static Task<ConfirmationMessage> ShowConfirmationTargetMessageAsync(ViewModel self, string messageBoxText, string title, MessageBoxButton buttonKinds, MessageBoxResult defaultResult, string messageKey)
        {
            var mes = new ConfirmationMessage(messageBoxText, title, MessageBoxImage.Question, buttonKinds, defaultResult, messageKey);
            return await self.Messenger.GetResponseAsync(mes);
        }



        /// <summary>
        /// バインド先の Window を閉じます。
        /// </summary>
        /// <param name="self">ViewModel</param>
        public static void Close(this ViewModel self)
        {
            var mes = new WindowActionMessage(WindowAction.Close, "Close");
            self.Messenger.Raise(mes);
        }

        /// <summary>
        /// バインド先の Window を非同期で閉じます。
        /// </summary>
        /// <param name="self">ViewModel</param>
        /// <returns></returns>
        public async static Task CloseAsync(this ViewModel self)
        {
            var mes = new WindowActionMessage(WindowAction.Close, "CloseAsync");
            await self.Messenger.RaiseAsync(mes);
        }



        /// <summary>
        /// バインド先の Window を最大化します。
        /// </summary>
        /// <param name="self">ViewModel</param>
        public static void Maximize(this ViewModel self)
        {
            var mes = new WindowActionMessage(WindowAction.Maximize, "Maximize");
            self.Messenger.Raise(mes);
        }

        /// <summary>
        /// バインド先の Window を非同期で最大化します。
        /// </summary>
        /// <param name="self">ViewModel</param>
        /// <returns></returns>
        public async static Task MaximizeAsync(this ViewModel self)
        {
            var mes = new WindowActionMessage(WindowAction.Maximize, "MaximizeAsync");
            await self.Messenger.RaiseAsync(mes);
        }



        /// <summary>
        /// バインド先の Window を最小化します。
        /// </summary>
        /// <param name="self">ViewModel</param>
        public static void Minimize(this ViewModel self)
        {
            var mes = new WindowActionMessage(WindowAction.Minimize, "Minimize");
            self.Messenger.Raise(mes);
        }

        /// <summary>
        /// バインド先の Window を非同期で最小化します。
        /// </summary>
        /// <param name="self">ViewModel</param>
        /// <returns></returns>
        public async static Task MinimizeAsync(this ViewModel self)
        {
            var mes = new WindowActionMessage(WindowAction.Minimize, "MinimizeAsync");
            await self.Messenger.RaiseAsync(mes);
        }



        /// <summary>
        /// バインド先の Window をアクティブにします。
        /// </summary>
        /// <param name="self">ViewModel</param>
        public static void Active(this ViewModel self)
        {
            var mes = new WindowActionMessage(WindowAction.Active, "Active");
            self.Messenger.Raise(mes);
        }

        /// <summary>
        /// バインド先の Window を非同期でアクティブにします。
        /// </summary>
        /// <param name="self">ViewModel</param>
        /// <returns></returns>
        public async static Task ActiveAsync(this ViewModel self)
        {
            var mes = new WindowActionMessage(WindowAction.Active, "ActiveAsync");
            await self.Messenger.RaiseAsync(mes);
        }

    }
}
