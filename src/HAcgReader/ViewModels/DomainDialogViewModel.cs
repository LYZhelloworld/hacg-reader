// <copyright file="DomainDialogViewModel.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace HAcgReader.ViewModels
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// 神社域名对话框视图模型
    /// </summary>
    public class DomainDialogViewModel : BaseViewModel
    {
        /// <summary>
        /// 域名正则表达式
        /// </summary>
        private static readonly Regex DomainNameRegex = new(
            @"^((?!-))(xn--)?[a-z0-9][a-z0-9-_]{0,61}[a-z0-9]{0,1}\.(xn--)?([a-z0-9\-]{1,61}|[a-z0-9-]{1,30}\.[a-z]{2,})$",
            RegexOptions.Compiled);

        /// <summary>
        /// 神社域名
        /// </summary>
        private string domain = string.Empty;

        /// <summary>
        /// 确定按钮是否可用
        /// </summary>
        private bool isOKButtonEnabled;

        /// <summary>
        /// 神社域名
        /// </summary>
        public string Domain
        {
            get => this.domain;
            set
            {
                this.domain = value;
                this.OnPropertyChanged();
                this.IsOKButtonEnabled = DomainNameRegex.IsMatch(value);
            }
        }

        /// <summary>
        /// 确定按钮是否可用
        /// </summary>
        public bool IsOKButtonEnabled
        {
            get => this.isOKButtonEnabled;
            set
            {
                this.isOKButtonEnabled = value;
                this.OnPropertyChanged();
            }
        }
    }
}
