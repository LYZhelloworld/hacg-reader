using HAcgReader.Models;
using HAcgReader.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HAcgReader.ViewModels;

/// <summary>
/// 文章列表视图模型
/// </summary>
public class ArticleListViewModel : BaseViewModel
{
    #region View Model Properties
    /// <summary>
    /// 文章列表
    /// </summary>
    public IEnumerable<ArticleModel> Articles
    {
        get => _articles;
        set
        {
            _articles = value.ToList();
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// 文章列表
    /// </summary>
    private List<ArticleModel> _articles = new();

    /// <summary>
    /// 被选中文章的下标
    /// </summary>
    public int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            _selectedIndex = value;
            if (_selectedIndex >= 0)
            {
                ArticleSelected?.Invoke(this, new() { SelectedArticle = _articles[SelectedIndex] });
            }

            OnPropertyChanged();
        }
    }

    /// <summary>
    /// 被选中文章的下标
    /// </summary>
    private int _selectedIndex = -1;
    #endregion

    #region Events
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
    #endregion

    #region Fields
    /// <summary>
    /// 获取神社 RSS Feed
    /// </summary>
    private readonly IRssFeedService _rssFeedService;

    /// <summary>
    /// 分析页面内容，寻找磁链
    /// </summary>
    private readonly IPageAnalyzerService _pageAnalyzerService;

    /// <summary>
    /// 未分析文章的缓存
    /// </summary>
    private readonly List<ArticleModel> _articleCache = new();
    #endregion

    #region Constructors
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="domain">神社域名</param>
    public ArticleListViewModel(string domain)
        : this(rssFeedService: new RssFeedService(domain),
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
        _rssFeedService = rssFeedService;
        _pageAnalyzerService = pageAnalyzerService;
    }
    #endregion

    #region Methods
    /// <summary>
    /// 拉取并分析文章
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    public void Fetch(CancellationToken cancellationToken)
    {
        FetchStarted?.Invoke(this, EventArgs.Empty);

        try
        {
            FetchInternal(cancellationToken);
        }
        catch (TaskCanceledException)
        {
            FetchCancelled?.Invoke(this, EventArgs.Empty);
        }

        FetchCompleted?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// 拉取并分析文章
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <exception cref="TaskCanceledException">在任务取消时抛出</exception>
    private void FetchInternal(CancellationToken cancellationToken)
    {
        // 如果未分析完毕的文章缓存为空则拉取新文章，否则继续处理
        if (_articleCache.Count == 0)
        {
            // 有时会出现获取到的内容重复的现象，暂时先采用这种办法过滤
            var fetchedArticles = _rssFeedService.FetchNext(cancellationToken)
                .Where(newArticle => !_articles.Exists(article => article.Link == newArticle.Link));
            _articleCache.AddRange(fetchedArticles);
        }

        var processed = 0;
        var total = _articleCache.Count;
        RssFeedFetched?.Invoke(this, new() { Total = total });

        while (_articleCache.Count > 0)
        {
            var article = _articleCache[0];
            var analyzedArticle = _pageAnalyzerService.Analyze(article, cancellationToken);

            // 需要创建新的 List 对象才能更新绑定
            var newArticles = _articles.ToList();
            newArticles.Add(analyzedArticle);
            Articles = newArticles;

            _articleCache.RemoveAt(0);
            processed++;
            PageAnalysisCompleted?.Invoke(this, new() { Progress = processed });
        }
    }
    #endregion
}