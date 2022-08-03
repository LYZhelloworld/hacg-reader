// <copyright file="ArticleControl.xaml.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace HAcgReader.Controls
{
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Navigation;
    using HAcgReader.Core.Models;
    using HAcgReader.Resources;

    /// <summary>
    /// ArticleControl.xaml 的交互逻辑
    /// </summary>
    public partial class ArticleControl : UserControl
    {
        /// <summary>
        /// <see cref="Article"/> 的 <see cref="DependencyProperty"/>
        /// </summary>
        public static readonly DependencyProperty ArticleProperty =
            DependencyProperty.Register(
                nameof(Article),
                typeof(ArticleModel),
                typeof(ArticleListControl));

        /// <summary>
        /// 构造函数
        /// </summary>
        public ArticleControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// 当前文章
        /// </summary>
        public ArticleModel Article
        {
            get { return (ArticleModel)this.GetValue(ArticleProperty); }
            set { this.SetValue(ArticleProperty, value); }
        }

        /// <summary>
        /// 文章链接、文章评论链接点击事件处理
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件参数</param>
        private void Link_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            var link = ((Hyperlink)sender).NavigateUri.AbsoluteUri;
            Process.Start(new ProcessStartInfo()
            {
                FileName = link,
                UseShellExecute = true,
            });
            e.Handled = true;
        }

        /// <summary>
        /// 磁力链接点击事件处理
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件参数</param>
        private void MagnetLink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            var magnetLink = ((Hyperlink)sender).NavigateUri.AbsoluteUri;
            Clipboard.SetText(magnetLink);
            MessageBox.Show(string.Format(CultureInfo.CurrentCulture, Strings.MagnetLinkCopied, magnetLink), Strings.Title, MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
