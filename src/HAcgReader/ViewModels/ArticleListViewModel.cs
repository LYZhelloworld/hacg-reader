// <copyright file="ArticleListViewModel.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace HAcgReader.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using HAcgReader.Core.Models;

    /// <summary>
    /// 文章列表视图模型
    /// </summary>
    public class ArticleListViewModel : BaseViewModel
    {
        /// <summary>
        /// 未分析文章的缓存
        /// </summary>
        private readonly Queue<ArticleModel> articleCache = new();

        /// <summary>
        /// 文章列表
        /// </summary>
        private List<ArticleModel> articles = new();

        /// <summary>
        /// 被选中文章的下标
        /// </summary>
        private int selectedIndex = -1;

        /// <summary>
        /// 选中文章事件
        /// </summary>
        public event EventHandler<ArticleSelectedEventArgs>? ArticleSelected;

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
        /// 未分析文章的缓存
        /// </summary>
        public IEnumerable<ArticleModel> ArticleCache
        {
            get => this.articleCache;
        }

        /// <summary>
        /// 文章缓存是否存有文章
        /// </summary>
        public bool HasCachedArticles => this.articleCache.Any();

        /// <summary>
        /// 缓存中的文章个数
        /// </summary>
        public int CacheCount => this.articleCache.Count;

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
        /// 添加新文章到缓存，同时过滤掉链接相同的重复文章
        /// </summary>
        /// <param name="fetchedArticles">拉取到的新文章</param>
        public void AddDistinctArticlesToCache(IEnumerable<ArticleModel> fetchedArticles)
        {
            // 有时会出现获取到的内容重复的现象，暂时先采用这种办法过滤
            fetchedArticles
                .Where(newArticle => !this.articles.Exists(article => article.Link == newArticle.Link))
                .ToList()
                .ForEach(this.articleCache.Enqueue);
        }

        /// <summary>
        /// 弹出缓存中第一个文章对象
        /// </summary>
        /// <returns>缓存中第一个文章</returns>
        public ArticleModel PopFirstCachedArticle()
        {
            return this.articleCache.Dequeue();
        }

        /// <summary>
        /// 添加新文章
        /// </summary>
        /// <param name="article">新文章</param>
        /// <remarks>如果直接向 <see cref="Articles"/> 添加的话，可能不会更新绑定</remarks>
        public void AddArticle(ArticleModel article)
        {
            // 需要创建新的 List 对象才能更新绑定
            var newArticles = this.articles.ToList();
            newArticles.Add(article);
            this.Articles = newArticles;
        }
    }
}