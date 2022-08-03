// <copyright file="ArticleListControl.xaml.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace HAcgReader.Controls
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using HAcgReader.Core.Models;

    /// <summary>
    /// ArticleListControl.xaml 的交互逻辑
    /// </summary>
    public partial class ArticleListControl : UserControl
    {
        /// <summary>
        /// <see cref="Articles"/> 的 <see cref="DependencyProperty"/>
        /// </summary>
        public static readonly DependencyProperty ArticlesProperty =
            DependencyProperty.Register(
                nameof(Articles),
                typeof(IEnumerable<ArticleModel>),
                typeof(ArticleListControl));

        /// <summary>
        /// 构造函数
        /// </summary>
        public ArticleListControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// 文章列表
        /// </summary>
        public IEnumerable<ArticleModel> Articles
        {
            get { return (IEnumerable<ArticleModel>)this.GetValue(ArticlesProperty); }
            set { this.SetValue(ArticlesProperty, value); }
        }
    }
}
