// <copyright file="DetailPageViewModel.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace HAcgReader.ViewModels
{
    using System.Windows;
    using HAcgReader.Models;

    /// <summary>
    /// 详情页视图模型
    /// </summary>
    public class DetailPageViewModel : BaseViewModel
    {
        /// <summary>
        /// 被选中的文章
        /// </summary>
        private ArticleModel selectedArticle = new();

        /// <summary>
        /// 详情页可见性
        /// </summary>
        private Visibility visibility = Visibility.Hidden;

        /// <summary>
        /// 被选中的文章
        /// </summary>
        public ArticleModel SelectedArticle
        {
            get => this.selectedArticle;
            set
            {
                this.selectedArticle = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// 详情页可见性
        /// </summary>
        public Visibility Visibility
        {
            get => this.visibility;
            set
            {
                this.visibility = value;
                this.OnPropertyChanged();
            }
        }
    }
}