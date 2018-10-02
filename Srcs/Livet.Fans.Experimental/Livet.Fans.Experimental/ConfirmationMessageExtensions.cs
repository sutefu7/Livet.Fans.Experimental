﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Livet.Messaging
{
    /// <summary>
    /// ConfirmationMessage に対する拡張クラスです。
    /// </summary>
    public static class ConfirmationMessageExtensions
    {
        /// <summary>
        /// MessageBox を閉じた際、どのボタンを押したのか分かるように、クリックしたボタンを返却します。
        /// </summary>
        /// <param name="self">ConfirmationMessage</param>
        /// <returns>MessageBoxResult</returns>
        public static MessageBoxResult ClickedButton(this ConfirmationMessage self)
        {
            if (self.Response == null)
            {
                // Cancel 押した
                return MessageBoxResult.Cancel;
            }

            if (self.Response.HasValue)
            {
                if (self.Response.Value)
                {
                    // Yes/OK 押した
                    if (self.Button == MessageBoxButton.OK || self.Button == MessageBoxButton.OKCancel)
                        return MessageBoxResult.OK;

                    if (self.Button == MessageBoxButton.YesNo || self.Button == MessageBoxButton.YesNoCancel)
                        return MessageBoxResult.Yes;
                }
                else
                {
                    // No 押した
                    return MessageBoxResult.No;
                }

            }

            return MessageBoxResult.None;

        }


    }
}
