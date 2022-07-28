// <copyright file="ArticleListViewModel.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace HAcgReader.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using HAcgReader.Core.Models;
    using HAcgReader.Core.Services;

    /// <summary>
    /// 文章列表视图模型
    /// </summary>
    public class ArticleListViewModel : BaseViewModel
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
        /// 未分析文章的缓存
        /// </summary>
        private readonly List<ArticleModel> articleCache = new();

        /// <summary>
        /// 文章列表
        /// </summary>
        private List<ArticleModel> articles = new();

        /// <summary>
        /// 被选中文章的下标
        /// </summary>
        private int selectedIndex = -1;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="domain">神社域名</param>
        public ArticleListViewModel(string domain)
            : this(
                rssFeedService: new RssFeedService(domain),
                pageAnalyzerService: new PageAnalyzerService())
        {
        }

        /// <summary>
        /// 初始化所有依赖的构造函数
        /// </summary>
        /// <param name="rssFeedService">获取神社 RSS Feed</param>
        /// <param name="pageAnalyzerService">分析页面内容，寻找磁链</param>
        public ArticleListViewModel(IRssFeedService rssFeedService, IPageAnalyzerService pageAnalyzerService)
        {
            this.rssFeedService = rssFeedService;
            this.pageAnalyzerService = pageAnalyzerService;
        }

        /// <summary>
        /// 选中文章事件
        /// </summary>
        public event EventHandler<ArticleSelectedEventArgs>? ArticleSelected;

        /// <summary>
        /// 拉取开始事件
        /// </summary>
        public event EventHandler? FetchStarted;

        /// <summary>
        /// RSS Feed 拉取完毕事件
        /// </summary>
        public event EventHandler<RssFeedFetchedEventArgs>? RssFeedFetched;

        /// <summary>
        /// 页面分析结束事件
        /// </summary>
        public event EventHandler<PageAnalysisCompletedEventArgs>? PageAnalysisCompleted;

        /// <summary>
        /// 拉取完毕事件
        /// </summary>
        public event EventHandler? FetchCompleted;

        /// <summary>
        /// 拉取取消事件
        /// </summary>
        public event EventHandler? FetchCancelled;

        /// <summary>
        /// 文章列表
        /// </summary>
        public IEnumerable<ArticleModel> Articles
        {
            get => this.articles;
            set
            {
                this.articles = value.ToList();
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// 被选中文章的下标
        /// </summary>
        public int SelectedIndex
        {
            get => this.selectedIndex;
            set
            {
                this.selectedIndex = value;
                if (this.selectedIndex >= 0)
                {
                    this.ArticleSelected?.Invoke(this, new() { SelectedArticle = this.articles[this.SelectedIndex] });
                }

                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// 拉取并分析文章
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        public void Fetch(CancellationToken cancellationToken)
        {
            this.FetchStarted?.Invoke(this, EventArgs.Empty);

            try
            {
                this.FetchInternal(cancellationToken);
            }
            catch (TaskCanceledException)
            {
                this.FetchCancelled?.Invoke(this, EventArgs.Empty);
            }

            this.FetchCompleted?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 拉取并分析文章
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <exception cref="TaskCanceledException">在任务取消时抛出</exception>
        private void FetchInternal(CancellationToken cancellationToken)
        {
            // 如果未分析完毕的文章缓存为空则拉取新文章，否则继续处理
            if (this.articleCache.Count == 0)
            {
                // 有时会出现获取到的内容重复的现象，暂时先采用这种办法过滤
                var fetchedArticles = this.rssFeedService.FetchNext(cancellationToken)
                    .Where(newArticle => !this.articles.Exists(article => article.Link == newArticle.Link));
                this.articleCache.AddRange(fetchedArticles);
            }

            var processed = 0;
            var total = this.articleCache.Count;
            this.RssFeedFetched?.Invoke(this, new() { Total = total });

            while (this.articleCache.Count > 0)
            {
                var article = this.articleCache[0];
                var analyzedArticle = this.pageAnalyzerService.Analyze(article, cancellationToken);

                // 需要创建新的 List 对象才能更新绑定
                var newArticles = this.articles.ToList();
                newArticles.Add(analyzedArticle);
                this.Articles = newArticles;

                this.articleCache.RemoveAt(0);
                processed++;
                this.PageAnalysisCompleted?.Invoke(this, new() { Progress = processed });
            }
        }
    }
}