using HAcgReader.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HAcgReader.Models;

/// <summary>
/// <see cref="IMainWindowModel"/> 的实现
/// </summary>
public class MainWindowModel : IMainWindowModel
{
    /// <summary>
    /// 空文章，仅做占位符用
    /// </summary>
    private static readonly ArticleModel s_emptyArticle = new();

    /// <summary>
    /// 文章列表
    /// </summary>
    private readonly List<ArticleModel> _articles = new();

    /// <summary>
    /// 获取神社 RSS Feed
    /// </summary>
    private readonly IRssFeedService _rssFeedService;

    /// <summary>
    /// 分析页面内容，寻找磁链
    /// </summary>
    private readonly IPageAnalyzerService _pageAnalyzerService;

    /// <summary>
    /// 选中的列表项
    /// </summary>
    private int _selectedIndex = -1;

    /// <inheritdoc/>
    public IEnumerable<ArticleModel> Articles => _articles;

    /// <inheritdoc/>
    public int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            _selectedIndex = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(SelectedArticle));
        }
    }

    /// <inheritdoc/>
    public bool ShowDetails => SelectedIndex >= 0 && SelectedIndex < _articles.Count;

    /// <inheritdoc/>
    public ArticleModel SelectedArticle
    {
        get
        {
            if (ShowDetails)
            {
                return _articles[SelectedIndex];
            }
            else
            {
                return s_emptyArticle;
            }
        }
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="domain">神社域名</param>
    public MainWindowModel(string domain)
        : this(rssFeedService: new RssFeedService(domain),
               pageAnalyzerService: new PageAnalyzerService())
    {
    }

    /// <summary>
    /// 初始化所有依赖的构造函数
    /// </summary>
    /// <param name="rssFeedService">获取神社 RSS Feed</param>
    /// <param name="pageAnalyzerService">分析页面内容，寻找磁链</param>
    public MainWindowModel(IRssFeedService rssFeedService, IPageAnalyzerService pageAnalyzerService)
    {
        _rssFeedService = rssFeedService;
        _pageAnalyzerService = pageAnalyzerService;
    }

    /// <inheritdoc/>
    public void AddArticles(params ArticleModel[] articles)
    {
        foreach (var article in articles)
        {
            _articles.Add(article);
        }
    }

    /// <inheritdoc/>
    public async Task FetchNewArticlesAsync()
    {
        var feedArticles = (await _rssFeedService.FetchNextAsync().ConfigureAwait(false)).ToArray();
        foreach (var article in feedArticles)
        {
            var analyzedArticle = await _pageAnalyzerService.AnalyzeAsync(article).ConfigureAwait(false);
            AddArticles(analyzedArticle);
        }

        OnPropertyChanged(nameof(Articles));
    }

    /// <summary>
    /// 属性改变事件处理
    /// </summary>
    /// <param name="propertyName">属性名</param>
    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new(propertyName));
    }
}
