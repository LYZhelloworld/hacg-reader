// <copyright file="DomainDialog.xaml.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace HAcgReader
{
    using System.Windows;
    using HAcgReader.ViewModels;

    /// <summary>
    /// DomainDialog.xaml 的交互逻辑
    /// </summary>
    public partial class DomainDialog : Window
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public DomainDialog()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// 神社域名对话框视图模型
        /// </summary>
        public DomainDialogViewModel DomainDialogViewModel { get; private set; } = new();

        /// <summary>
        /// 确定按钮点击事件
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件参数</param>
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
