﻿using HAcgReader.Models;
using HAcgReader.Services;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace HAcgReader.ViewModels;

/// <summary>
/// <see cref="MainWindow"/> 的视图模型
/// </summary>
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
    /// 拉取按钮是否有效
    /// </summary>
    private bool _isFetchingButtonEnabled = true;

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
            SelectedArticle = _selectedIndex >= 0 ? _articles[_selectedIndex] : s_emptyArticle;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// 被选中文章的下标
    /// </summary>
    private int _selectedIndex = -1;

    /// <summary>
    /// 被选中的文章
    /// </summary>
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

    /// <summary>
    /// 被选中的文章
    /// </summary>
    private ArticleModel _selectedArticle = s_emptyArticle;

    /// <summary>
    /// 详情页可见性
    /// </summary>
    public Visibility DetailPanelVisibility
    {
        get => _detailPanelVisibility;
        set
        {
            _detailPanelVisibility = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// 详情页可见性
    /// </summary>
    private Visibility _detailPanelVisibility = Visibility.Hidden;

    /// <summary>
    /// 滚动条当前值
    /// </summary>
    public int ProgressBarValue
    {
        get => _progressBarValue;
        set
        {
            _progressBarValue = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// 滚动条当前值
    /// </summary>
    private int _progressBarValue;

    /// <summary>
    /// 滚动条最大值
    /// </summary>
    public int ProgressBarMaximum
    {
        get => _progressBarMaximum;
        set
        {
            _progressBarMaximum = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// 滚动条最大值
    /// </summary>
    private int _progressBarMaximum = 10;

    /// <summary>
    /// 滚动条处于不确定状态
    /// </summary>
    public bool ProgressBarIsIndeterminate
    {
        get => _progressBarIsIndeterminate;
        set
        {
            _progressBarIsIndeterminate = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// 滚动条处于不确定状态
    /// </summary>
    private bool _progressBarIsIndeterminate;

    /// <summary>
    /// 拉取命令
    /// </summary>
    public ICommand FetchCommand => _fetchCommand;

    /// <summary>
    /// 拉取命令
    /// </summary>
    private readonly RelayCommand _fetchCommand;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// 拉取完毕事件
    /// </summary>
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

        _fetchCommand = new RelayCommand(() => Task.Run(FetchAsync), () => IsFetchingButtonEnabled);
    }

    /// <summary>
    /// 异步拉取文章
    /// </summary>
    /// <returns>当前异步操作的任务</returns>
    public async Task FetchAsync()
    {
        if (!IsFetchingButtonEnabled)
        {
            return;
        }

        IsFetchingButtonEnabled = false;
        _fetchCommand.NotifyCanExecuteChanged();
        ProgressBarIsIndeterminate = true;

        // 有时会出现获取到的内容重复的现象，暂时先采用这种办法过滤
        var feedArticles = (await _rssFeedService.FetchNextAsync().ConfigureAwait(false))
            .Where(newArticle => !_articles.Exists(article => article.Link == newArticle.Link))
            .ToArray();

        var processed = 0;
        var total = feedArticles.Length;
        ProgressBarValue = 0;
        ProgressBarMaximum = total;
        ProgressBarIsIndeterminate = false;

        foreach (var article in feedArticles)
        {
            var analyzedArticle = await _pageAnalyzerService.AnalyzeAsync(article).ConfigureAwait(false);

            // 需要创建新的 List 对象才能更新绑定
            var newArticles = _articles.ToList();
            newArticles.Add(analyzedArticle);
            Articles = newArticles;

            processed++;
            ProgressBarValue = processed;
        }

        IsFetchingButtonEnabled = true;
        _fetchCommand.NotifyCanExecuteChanged();
        _progressBarValue = 0;
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
