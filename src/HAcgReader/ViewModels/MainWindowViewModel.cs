using HAcgReader.Models;
using HAcgReader.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace HAcgReader.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// 空文章，仅做占位符用
    /// </summary>
    private static readonly ArticleModel s_emptyArticle = new();

    public ICommand FetchCommand { get; }

    /// <summary>
    /// 是否正在拉取新的文章
    /// </summary>
    public bool IsFetching
    {
        get => _isFetching;
        set
        {
            _isFetching = value;
            OnPropertyChanged();
        }
    }

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

    public int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            _selectedIndex = value;
            SelectedArticle = _selectedIndex >= 0 ? _articles[_selectedIndex] : s_emptyArticle;
            OnPropertyChanged();
        }
    }

    public ArticleModel SelectedArticle
    {
        get => _selectedArticle;
        set
        {
            _selectedArticle = value;
            OnPropertyChanged();
            DetailPanelVisibility = value == null ? Visibility.Hidden : Visibility.Visible;
        }
    }

    public Visibility DetailPanelVisibility
    {
        get => _detailPanelVisibility;
        set
        {
            _detailPanelVisibility = value;
            OnPropertyChanged();
        }
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    public event EventHandler<StartedArticleProcessingEventArgs> StartedArticleProcessing;

    public event EventHandler<ArticleProcessedEventArgs>? ArticleProcessed;

    public event EventHandler AllArticlesProcessed;

    /// <summary>
    /// 获取神社 RSS Feed
    /// </summary>
    private readonly IRssFeedService _rssFeedService;

    /// <summary>
    /// 分析页面内容，寻找磁链
    /// </summary>
    private readonly IPageAnalyzerService _pageAnalyzerService;

    /// <summary>
    /// 是否正在拉取新的文章
    /// </summary>
    private bool _isFetching;

    /// <summary>
    /// 文章列表
    /// </summary>
    private List<ArticleModel> _articles = new();

    private int _selectedIndex = -1;

    private ArticleModel _selectedArticle = s_emptyArticle;

    private Visibility _detailPanelVisibility = Visibility.Hidden;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="domain">神社域名</param>
    public MainWindowViewModel(string domain)
        : this(rssFeedService: new RssFeedService(domain), pageAnalyzerService: new PageAnalyzerService())
    {
    }

    /// <summary>
    /// 初始化所有依赖的构造函数
    /// </summary>
    /// <param name="rssFeedService">获取神社 RSS Feed</param>
    /// <param name="pageAnalyzerService">分析页面内容，寻找磁链</param>
    public MainWindowViewModel(IRssFeedService rssFeedService, IPageAnalyzerService pageAnalyzerService)
    {
        _rssFeedService = rssFeedService;
        _pageAnalyzerService = pageAnalyzerService;
        FetchCommand = new FetchCommand(this);
    }

    public void Fetch()
    {
        if (IsFetching)
        {
            return;
        }

        IsFetching = true;
        Task.Run(async () =>
        {
            var feedArticles = (await _rssFeedService.FetchNextAsync().ConfigureAwait(false)).ToArray();

            var processed = 0;
            var total = feedArticles.Length;
            StartedArticleProcessing?.Invoke(this, new() { Total = total });

            foreach (var article in feedArticles)
            {
                var analyzedArticle = await _pageAnalyzerService.AnalyzeAsync(article).ConfigureAwait(false);
                _articles.Add(analyzedArticle);
                _articles = _articles.ToList();
                OnPropertyChanged(nameof(Articles));
                processed++;
                ArticleProcessed?.Invoke(this, new() { Processed = processed });
            }

            IsFetching = false;
            AllArticlesProcessed?.Invoke(this, EventArgs.Empty);
        });
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

public class FetchCommand : ICommand
{
    private readonly MainWindowViewModel _viewModel;

    public event EventHandler? CanExecuteChanged;

    public FetchCommand(MainWindowViewModel viewModel)
    {
        _viewModel = viewModel;
        _viewModel.PropertyChanged += _viewModel_PropertyChanged;
    }

    private void _viewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_viewModel.IsFetching))
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        _viewModel.Fetch();
    }
}

public class StartedArticleProcessingEventArgs : EventArgs
{
    public int Total { get; init; }
}

public class ArticleProcessedEventArgs : EventArgs
{
    public int Processed { get; init; }
}
