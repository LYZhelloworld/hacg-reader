// <copyright file="MainWindow.xaml.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace HAcgReader.Windows
{
    using System;
    using System.Windows;
    using HAcgReader.Core.Services;
    using HAcgReader.Resources;
    using HAcgReader.ViewModels;

    /// <summary>
    /// MainWindow.xaml 交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public MainWindow()
        {
            var domainService = new DomainService();
            var domain = domainService.GetDomain();
            if (string.IsNullOrEmpty(domain))
            {
                var domainDialog = new DomainDialog();
                var result = domainDialog.ShowDialog();
                if (result.GetValueOrDefault(false))
                {
                    domain = domainDialog.DomainDialogViewModel.Domain;
                }
            }

            if (string.IsNullOrEmpty(domain))
            {
                MessageBox.Show(
                    Strings.ErrorCannotRetrieveDomain,
                    Strings.Title,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Environment.Exit(-1);
            }

            this.MainViewModel = new MainViewModel(domain);

            this.InitializeComponent();
            this.MainViewModel.FetchButtonViewModel.Command.Execute(null);
        }

        /// <summary>
        /// 主窗口视图模型
        /// </summary>
        public MainViewModel MainViewModel { get; private set; }

        /// <summary>
        /// 文章点击事件处理
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件参数</param>
        private void ArticleListControl_ArticleClicked(object sender, Controls.ArticleClickedEventArgs e)
        {
            this.MainViewModel.OnArticleClicked(e.Article);
        }
    }
}
