// <copyright file="MainViewModel.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace HAcgReader.ViewModels
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using HAcgReader.Core.Models;
    using HAcgReader.Core.Services;
    using HAcgReader.Resources;

    /// <summary>
    /// 主窗口视图模型
    /// </summary>
    public class MainViewModel : BaseViewModel
    {
        /// <summary>
        /// 获取神社 RSS Feed
        /// </summary>
        private readonly IRssFeedService rssFeedService;

        /// <summary>
        /// 分析页面内容，寻找磁链
        /// </summary>
        private readonly IPageAnalyzerService pageAnalyzerService;

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
        /// <param name="domain">神社域名</param>
        public MainViewModel(string domain)
            : this(new RssFeedService(domain), new PageAnalyzerService())
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="rssFeedService">获取神社 RSS Feed</param>
        /// <param name="pageAnalyzerService">分析页面内容，寻找磁链</param>
        public MainViewModel(
            IRssFeedService rssFeedService,
            IPageAnalyzerService pageAnalyzerService)
        {
            this.rssFeedService = rssFeedService;
            this.pageAnalyzerService = pageAnalyzerService;

            this.InitializeViewModelEventHandlers();
        }

        /// <summary>
        /// 文章列表视图模型
        /// </summary>
        public ArticleListViewModel ArticleListViewModel { get; private set; } = new();

        /// <summary>
        /// 拉取按钮视图模型
        /// </summary>
        public FetchButtonViewModel FetchButtonViewModel { get; private set; } = new();

        /// <summary>
        /// 详情页视图模型
        /// </summary>
        public DetailPageViewModel DetailPageViewModel { get; private set; } = new();

        /// <summary>
        /// 滚动条视图模型
        /// </summary>
        public ProgressBarViewModel ProgressBarViewModel { get; private set; } = new();

        /// <summary>
        /// 拉取并分析文章
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        public void Fetch(CancellationToken cancellationToken)
        {
            this.OnFetchStarted();

            try
            {
                this.FetchInternal(cancellationToken);
            }
            catch (TaskCanceledException)
            {
            }
            catch (HttpRequestException)
            {
                MessageBox.Show(Strings.FetchFailedText, Strings.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.OnFetchCompleted();
            }
        }

        /// <summary>
        /// 文章点击事件处理
        /// </summary>
        /// <param name="article">被点击的文章</param>
        public void OnArticleClicked(ArticleModel article)
        {
            this.DetailPageViewModel.Article = article;
            this.DetailPageViewModel.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 拉取开始
        /// </summary>
        protected void OnFetchStarted()
        {
            this.ProgressBarViewModel.IsIndeterminate = true;
        }

        /// <summary>
        /// RSS Feed 拉取完毕
        /// </summary>
        /// <param name="total">总页面个数</param>
        protected void OnRssFeedFetched(int total)
        {
            this.ProgressBarViewModel.IsIndeterminate = false;
            this.ProgressBarViewModel.Maximum = total;
            this.ProgressBarViewModel.Value = 0;
        }

        /// <summary>
        /// 页面分析结束
        /// </summary>
        /// <param name="progress">已处理的页面数量</param>
        protected void OnPageAnalysisCompleted(int progress)
        {
            this.ProgressBarViewModel.Value = progress;
        }

        /// <summary>
        /// 拉取完毕
        /// </summary>
        protected void OnFetchCompleted()
        {
            this.ProgressBarViewModel.Value = 0;
            this.ProgressBarViewModel.Maximum = 10;
            this.ProgressBarViewModel.IsIndeterminate = false;

            this.FetchButtonViewModel.IsFetching = false;

            this.fetchingCancellationTokenSource?.Dispose();
            this.fetchingCancellationTokenSource = null;
            this.fetchingTask = null;
        }

        /// <summary>
        /// 拉取并分析文章
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <exception cref="TaskCanceledException">在任务取消时抛出</exception>
        private void FetchInternal(CancellationToken cancellationToken)
        {
            // 如果未分析完毕的文章缓存为空则拉取新文章，否则继续处理
            if (!this.ArticleListViewModel.HasCachedArticles)
            {
                var fetchedArticles = this.rssFeedService.FetchNext(cancellationToken);
                this.ArticleListViewModel.AddDistinctArticlesToCache(fetchedArticles);
            }

            var processed = 0;
            var total = this.ArticleListViewModel.CacheCount;
            this.OnRssFeedFetched(total);

            while (this.ArticleListViewModel.HasCachedArticles)
            {
                var article = this.ArticleListViewModel.PopFirstCachedArticle();
                var analyzedArticle = this.pageAnalyzerService.Analyze(article, cancellationToken);

                this.ArticleListViewModel.AddArticle(analyzedArticle);
                processed++;
                this.OnPageAnalysisCompleted(processed);
            }
        }

        /// <summary>
        /// 初始化视图模型事件监听器
        /// </summary>
        private void InitializeViewModelEventHandlers()
        {
            this.FetchButtonViewModel.Started += (sender, e) =>
            {
                this.fetchingCancellationTokenSource = new();
                this.fetchingTask = Task.Run(() => this.Fetch(this.fetchingCancellationTokenSource.Token));
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