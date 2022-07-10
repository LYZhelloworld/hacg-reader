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

namespace HAcgReader.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// 空文章，仅做占位符用
    /// </summary>
    private static readonly ArticleModel s_emptyArticle = new();

    /// <summary>
    /// 拉取按钮是否有效
    /// </summary>
    public bool IsFetchingButtonEnabled
    {
        get => _isFetchingButtonEnabled;
        set
        {
            _isFetchingButtonEnabled = value;
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

    public int ProgressBarValue
    {
        get => progressBarValue;
        set
        {
            progressBarValue = value;
            OnPropertyChanged();
        }
    }
    public int ProgressBarMaximum
    {
        get => progressBarMaximum;
        set
        {
            progressBarMaximum = value;
            OnPropertyChanged();
        }
    }

    public ICommand FetchCommand { get; }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    public event EventHandler? FetchCompleted;

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
    private bool _isFetchingButtonEnabled = true;

    /// <summary>
    /// 文章列表
    /// </summary>
    private List<ArticleModel> _articles = new();

    private int _selectedIndex = -1;

    private ArticleModel _selectedArticle = s_emptyArticle;

    private Visibility _detailPanelVisibility = Visibility.Hidden;

    private int progressBarValue;

    private int progressBarMaximum = 10;

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

    public async void FetchAsync()
    {
        if (!IsFetchingButtonEnabled)
        {
            return;
        }

        IsFetchingButtonEnabled = false;

        var feedArticles = (await _rssFeedService.FetchNextAsync().ConfigureAwait(false)).ToArray();

        var processed = 0;
        var total = feedArticles.Length;
        ProgressBarValue = 0;
        ProgressBarMaximum = total;

        foreach (var article in feedArticles)
        {
            var analyzedArticle = await _pageAnalyzerService.AnalyzeAsync(article).ConfigureAwait(false);
            _articles.Add(analyzedArticle);
            _articles = _articles.ToList();
            OnPropertyChanged(nameof(Articles));
            processed++;
            ProgressBarValue = processed;
        }

        IsFetchingButtonEnabled = true;
        progressBarValue = 0;
        FetchCompleted?.Invoke(this, EventArgs.Empty);
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

    public event EventHandler? CanExecuteChanged
    {
        add
        {
            CommandManager.RequerySuggested += value;
        }
        remove
        {
            CommandManager.RequerySuggested -= value;
        }
    }

    public FetchCommand(MainWindowViewModel viewModel)
    {
        _viewModel = viewModel;
    }

    public bool CanExecute(object? parameter)
    {
        return _viewModel.IsFetchingButtonEnabled;
    }

    public void Execute(object? parameter)
    {
        _viewModel.FetchAsync();
    }
}
