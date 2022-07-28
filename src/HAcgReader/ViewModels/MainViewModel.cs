// <copyright file="MainViewModel.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace HAcgReader.ViewModels
{
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;

    /// <summary>
    /// 主窗口视图模型
    /// </summary>
    public class MainViewModel : BaseViewModel
    {
        /// <summary>
        /// 拉取事件
        /// </summary>
        private Task? fetchingTask;

        /// <summary>
        /// 拉取事件 <see cref="CancellationToken"/> 源
        /// </summary>
        private CancellationTokenSource? fetchingCancellationTokenSource;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="articleListViewModel">文章列表视图模型</param>
        /// <param name="fetchButtonViewModel">拉取按钮视图模型</param>
        /// <param name="detailPageViewModel">详情页视图模型</param>
        /// <param name="progressBarViewModel">滚动条视图模型</param>
        public MainViewModel(
            ArticleListViewModel articleListViewModel,
            FetchButtonViewModel fetchButtonViewModel,
            DetailPageViewModel detailPageViewModel,
            ProgressBarViewModel progressBarViewModel)
        {
            this.ArticleListViewModel = articleListViewModel;
            this.FetchButtonViewModel = fetchButtonViewModel;
            this.DetailPageViewModel = detailPageViewModel;
            this.ProgressBarViewModel = progressBarViewModel;

            this.InitializeViewModelEventHandlers();
        }

        /// <summary>
        /// 文章列表视图模型
        /// </summary>
        public ArticleListViewModel ArticleListViewModel { get; private set; }

        /// <summary>
        /// 拉取按钮视图模型
        /// </summary>
        public FetchButtonViewModel FetchButtonViewModel { get; private set; }

        /// <summary>
        /// 详情页视图模型
        /// </summary>
        public DetailPageViewModel DetailPageViewModel { get; private set; }

        /// <summary>
        /// 滚动条视图模型
        /// </summary>
        public ProgressBarViewModel ProgressBarViewModel { get; private set; }

        /// <summary>
        /// 初始化视图模型事件监听器
        /// </summary>
        private void InitializeViewModelEventHandlers()
        {
            this.ArticleListViewModel.ArticleSelected += (sender, e) =>
            {
                this.DetailPageViewModel.SelectedArticle = e.SelectedArticle;
                this.DetailPageViewModel.Visibility = Visibility.Visible;
            };
            this.ArticleListViewModel.FetchStarted += (sender, e) =>
            {
                this.ProgressBarViewModel.IsIndeterminate = true;
            };
            this.ArticleListViewModel.RssFeedFetched += (sender, e) =>
            {
                this.ProgressBarViewModel.IsIndeterminate = false;
                this.ProgressBarViewModel.Maximum = e.Total;
                this.ProgressBarViewModel.Value = 0;
            };
            this.ArticleListViewModel.PageAnalysisCompleted += (sender, e) =>
            {
                this.ProgressBarViewModel.Value = e.Progress;
            };
            this.ArticleListViewModel.FetchCompleted += (sender, e) =>
            {
                this.ProgressBarViewModel.Value = 0;
                this.FetchButtonViewModel.IsFetching = false;

                this.fetchingCancellationTokenSource?.Dispose();
                this.fetchingCancellationTokenSource = null;
                this.fetchingTask = null;
            };
            this.ArticleListViewModel.FetchCancelled += (sender, e) =>
            {
                this.ProgressBarViewModel.Value = 0;
                this.ProgressBarViewModel.IsIndeterminate = false;
            };

            this.FetchButtonViewModel.Started += (sender, e) =>
            {
                this.fetchingCancellationTokenSource = new();
                this.fetchingTask = Task.Run(() => this.ArticleListViewModel.Fetch(this.fetchingCancellationTokenSource.Token));
            };
            this.FetchButtonViewModel.Cancelled += (sender, e) =>
            {
                this.FetchButtonViewModel.IsEnabled = false;
                this.fetchingCancellationTokenSource?.Cancel();
                this.fetchingCancellationTokenSource?.Dispose();
                Task.Run(() =>
                {
                    this.fetchingTask?.Wait();
                    this.fetchingTask = null;
                    this.FetchButtonViewModel.IsEnabled = true;
                });
            };
        }
    }
}